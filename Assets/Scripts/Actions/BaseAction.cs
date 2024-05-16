using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour {

    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;

    protected Unit unit;
    protected bool isActive;
    protected Action onActionComplete;

    public static void ResetStaticData() {
        OnAnyActionCompleted = null;
        OnAnyActionStarted = null;
    }

    protected virtual void Awake() {
        unit = GetComponent<Unit>();
    }

    public abstract string GetActionName();

    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

    public virtual int GetActionPointsCost() {
        return 1;
    }

    public virtual bool IsValidActionGridPosition(GridPosition gridPosition) {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public abstract List<GridPosition> GetValidActionGridPositionList();

    protected void ActionStart(Action onActionComplete) {
        isActive = true;
        this.onActionComplete = onActionComplete;
        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void ActionComplete() {
        isActive = false;
        onActionComplete();
        if (TurnSystem.Instance.IsPlayerTurn() || (this is ShootAction)) {
            OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
        }
    }

    public Unit GetUnit() {
        return unit;
    }

    public EnemyAIAction GetBestEnemyAIAction() {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();
        List<GridPosition> validActionGridPositionList = GetValidActionGridPositionList();

        foreach (GridPosition gridPosition in validActionGridPositionList) {
            EnemyAIAction enemyAIAction = GetBestEnemyAIAction(gridPosition, out BaseAction baseAction);
            if (baseAction is InteractAction && LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition) is InteractSphere) {
                continue;
            }
            enemyAIActionList.Add(enemyAIAction);
        }

        foreach (EnemyAIAction enemyAIAction in enemyAIActionList) {
            //Debug.Log(enemyAIAction.baseAction.ToString());
        }

        if (enemyAIActionList.Count > 0) {
            enemyAIActionList.Sort((EnemyAIAction actionA, EnemyAIAction actionB) => actionB.actionValue - actionA.actionValue);
             EnemyAIAction enemyAIAction = enemyAIActionList[0];
             enemyAIActionList.RemoveAt(0);
             return enemyAIAction;
        } else {
            // No possible EnemyAI Actions
            return null;
        }
       
    }

    public abstract EnemyAIAction GetBestEnemyAIAction(GridPosition gridPosition, out BaseAction baseAction);
}
