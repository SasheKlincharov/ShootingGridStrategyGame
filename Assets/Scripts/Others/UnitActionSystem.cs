using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class UnitActionSystem : MonoBehaviour {

    public static UnitActionSystem Instance { get; private set; }
    public event EventHandler OnSelectedUnitChange;
    public event EventHandler OnSelectedActionChange;
    public event EventHandler OnActionStarted;
    public event EventHandler<bool> OnBusyChange;
    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;
    private BaseAction selectedAction;
    private bool isBusy;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There's more than one UnitActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start() {
        SetSelectedUnit(selectedUnit);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e) {
        if (selectedUnit.IsDead()) {
            List<Unit> friendlyUnitList = UnitManager.Instance.GetFriendlyUnitList();
            if (friendlyUnitList.Count > 0) {
                SetSelectedUnit(GetNearestUnit(selectedUnit, friendlyUnitList));
            } else {
                // Game Over!
            }
        }
    }

    public Unit GetNearestUnit(Unit selectedUnit, List<Unit> friendlyUnitList) {
        Unit nearestUnit = null;
        GridPosition nearestGridPosition = new GridPosition(999, 999, 0);
        foreach (Unit friendlyUnit in friendlyUnitList) {
            GridPosition distanceGridPosition;
            if (friendlyUnit.GetGridPosition() > selectedUnit.GetGridPosition()) {
                distanceGridPosition = friendlyUnit.GetGridPosition() - selectedUnit.GetGridPosition();
            } else {
                distanceGridPosition = selectedUnit.GetGridPosition() - friendlyUnit.GetGridPosition();
            }

            if (distanceGridPosition < nearestGridPosition) {
                nearestGridPosition = distanceGridPosition;
                nearestUnit = friendlyUnit;
            }

        }

        return nearestUnit;
    }

    private void Update() {
        if (!GameHandler.Instance.IsGamePlaying()) {
            return;
        }

        if (isBusy) {
            return;
        }

        if (!TurnSystem.Instance.IsPlayerTurn()) {
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }

        if (TryHandleUnitSelection()) {
            return; 
        }

        HandleSelectedAction();
    }

    private void HandleSelectedAction() {
        if (InputManager.Instance.IsMouseButtonDownThisFrame()) {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPositionOnlyHitVisible());

            if (!selectedAction.IsValidActionGridPosition(mouseGridPosition)) {
                return;
            }

            bool isSphere = LevelGrid.Instance.GetInteractableAtGridPosition(mouseGridPosition) is InteractSphere;
            if (!isSphere) {
                if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction)) {
                    return;
                }
            } 

            SetBusy();
            selectedAction.TakeAction(mouseGridPosition, ClearBusy);
            if (isSphere) {
                selectedUnit.IncrementActionPoints();
            }

            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    private void SetBusy() {
        isBusy = true;
        OnBusyChange?.Invoke(this, isBusy);
    }

    private void ClearBusy() {
        isBusy = false;
        OnBusyChange?.Invoke(this, isBusy);
    }


    private bool TryHandleUnitSelection() {
        if (InputManager.Instance.IsMouseButtonDownThisFrame()) {
            Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());

            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask)) {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit)) {
                    if (unit == selectedUnit) {
                        return false;
                    }

                    if (unit.IsEnemy()) {
                        return false;
                    }
                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }

        return false;
    }
    public void SetSelectedUnit(Unit unit) {
        selectedUnit = unit;
        SetSelectedAction(unit.GetAction<MoveAction>());
        OnSelectedUnitChange?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction) {
        this.selectedAction = baseAction;
        OnSelectedActionChange?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit() {
        return selectedUnit;
    }

    public BaseAction GetSelectedAction() {
        return selectedAction;
    }
}
