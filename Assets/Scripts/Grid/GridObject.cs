using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject {
    private GridPosition gridPosition;
    private GridSystem<GridObject> gridSystem;
    private List<Unit> unitList;
    private IInteractable interactable;

    public GridObject(GridPosition gridPosition, GridSystem<GridObject> gridSystem) {
        this.gridPosition = gridPosition;
        this.gridSystem = gridSystem;
        this.unitList = new List<Unit>();
    }

    public void AddUnit(Unit unit) {
        this.unitList.Add(unit);
    }

    public List<Unit> GetUnitList() {
        return unitList;
    }

    public void RemoveUnit(Unit unit) {
        unitList.Remove(unit);
    }

    public GridPosition GetGridPosition() {
        return gridPosition;
    }

    public bool HasAnyUnit() {
        return unitList.Count > 0;
    }

    public bool IsBarrel() {
        return interactable is DestructableBarrel;
    }

    public Unit GetUnit() {
        if (HasAnyUnit()) {
            return unitList[0];
        }

        return null;
    }

    public IInteractable GetInteractable() {
        return interactable;
    }

    public bool TryGetInteractable(out IInteractable interactable) {
        if (GetInteractable() != null) {
            interactable = GetInteractable();
            return true;
        }

        interactable = null;
        return false;
    }

    public void SetInteractable(IInteractable interactable) {
        this.interactable = interactable;
    }
    public void ClearInteractable() {
        this.interactable = null;
    }

    public bool TryGetUnit(out Unit unit) {
        if (HasAnyUnit() && unitList.Count > 0) {
            unit = GetUnit();
            return true;
        }
        unit = null;
        return false;
    }

    public override string ToString() {
        string unitString = "";
        foreach (Unit unit in unitList) {
            unitString += unit + "\n";
        }
        return gridPosition.ToString() + "\n" + unitString;
    }
}