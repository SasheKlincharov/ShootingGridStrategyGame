using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitActionSystemUI : MonoBehaviour {
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private TextMeshProUGUI actionPointsText;
    private List<ActionButtonUI> actionButtonsUIList;

    private void Awake() {
        actionButtonsUIList = new List<ActionButtonUI>();
    }

    private void Start() {
        UnitActionSystem.Instance.OnSelectedUnitChange += UnitActionSystem_OnSelectedUnitChange;
        UnitActionSystem.Instance.OnSelectedActionChange += UnitActionSystem_OnSelectedActionChange;
        UnitActionSystem.Instance.OnActionStarted += UnityActionSystem_OnActionStarted;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, System.EventArgs e) {
        UpdateActionPoints();
    }

    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e) {
        UpdateActionPoints(); 
    }

    private void UnityActionSystem_OnActionStarted(object sender, System.EventArgs e) {
        UpdateActionPoints();
    }

    private void UnitActionSystem_OnSelectedActionChange(object sender, System.EventArgs e) {
        UpdateSelectedVisual();
    }

    private void UnitActionSystem_OnSelectedUnitChange(object sender, System.EventArgs e) {
        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void UpdateSelectedVisual() {
       foreach (ActionButtonUI actionButton in actionButtonsUIList) {
            actionButton.UpdateSelectedVisual();
        }
    }

    private void CreateUnitActionButtons() {
        foreach (Transform buttonTransform in actionButtonContainerTransform) {
            Destroy(buttonTransform.gameObject);
        }

        actionButtonsUIList.Clear();
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        foreach (BaseAction action in selectedUnit.GetBaseActionArray()) {
            Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(action);
            actionButtonsUIList.Add(actionButtonUI);
        }
    }

    private void UpdateActionPoints() {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        actionPointsText.text = "Action Points: " + selectedUnit.GetActionPoints();
        
    }
}
