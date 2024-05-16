using System;
using UnityEngine;

public class Unit : MonoBehaviour {

    private const int ACTION_POINTS_MAX = 3;

    [SerializeField] private bool isEnemy;

    private HealthSystem healthSystem;

    public static event EventHandler OnAnyActionPointsChanged;

    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;


    private GridPosition gridPosition;
    private BaseAction[] baseActionArray;
    private int actionPoints = ACTION_POINTS_MAX;

    public static void ResetStaticData() {
        OnAnyActionPointsChanged = null;
        OnAnyUnitSpawned = null;
        OnAnyUnitDead = null;
    }

    private void Awake() {
        healthSystem = GetComponent<HealthSystem>();
        baseActionArray = GetComponents<BaseAction>();
    }

    private void HealthSystem_OnDead(object sender, EventArgs e) {
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        Destroy(gameObject);
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    private void Start() {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update() {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition) {
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;

            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition);
        }
    }

    public T GetAction<T>() where T : BaseAction {
        foreach (BaseAction baseAction in baseActionArray) {
            if (baseAction is T) {
                return baseAction as T;
            }
        }

        return null;
    }

    public GridPosition GetGridPosition() {
        return gridPosition;
    }

    public Vector3 GetWorldPosition() {
        return transform.position;
    }

    public BaseAction[] GetBaseActionArray() {
        return baseActionArray;
    }

    public int GetActionPoints() {
        return actionPoints;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction) {
        if (CanSpendActionPointsToTakeAction(baseAction)) {
            SpendActionPoints(baseAction.GetActionPointsCost());
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
            return true;
        } else {
            return false;
        }
    }

    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e) {
        if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) ||
            (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn())) {
            actionPoints = ACTION_POINTS_MAX;
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
       
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction) {
        if (actionPoints >= baseAction.GetActionPointsCost()) {
            return true;
        } else {
            return false;
        }        
    }

    private void SpendActionPoints(int amount) {
        actionPoints -= amount;
    }

    public void IncrementActionPoints() {
        actionPoints += 1;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsEnemy() {
        return isEnemy;
    }

    public void Damage(int damageAmount, Unit shooterUnit, Unit targetUnit, BaseAction baseAction) {
        healthSystem.Damage(damageAmount, shooterUnit, targetUnit, baseAction);
    }

    public bool IsDead() {
        return healthSystem.GetHealthNormalized() <= 0;
    }

    public float GetHealthNormalized() {
        return healthSystem.GetHealthNormalized();
    }

    public bool IsFullHealth() {
        return healthSystem.GetHealth() == 100;
    }

    public void HealUp(int amount) {
        healthSystem.AddHealth(amount);
    }
}

