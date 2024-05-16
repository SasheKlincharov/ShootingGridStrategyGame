using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction {
    public static event EventHandler OnAnySwordHit;
    public event EventHandler OnSwordActionStarted;
    public event EventHandler OnSwordActionCompleted;

    private enum State {
        SwingingSwordBeforeHit,
        SwingingSwordAfterHit,
    }

    private State state;

    private float stateTimer;

    private Unit targetUnit;
    private DestructableBarrel targetBarrel;

    private int maxSwordDistance = 1;

    new public static void ResetStaticData() {
        OnAnySwordHit = null;
    }


    private void Update() {
        if (!isActive) {
            return;
        }

        stateTimer -= Time.deltaTime;
        switch (state) {
            case State.SwingingSwordBeforeHit:
                Vector3 aimDirection;
                if (targetUnit != null) {
                    aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                } else {
                    aimDirection = (targetBarrel.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                }
                float rotateSpeed = 15f;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
                break;
            case State.SwingingSwordAfterHit:
                break;
            default:
                break;
        }

        if (stateTimer <= 0f) {
            NextState();
        }

    }

    private void NextState() {
        switch (state) {
            case State.SwingingSwordBeforeHit:
                state = State.SwingingSwordAfterHit;
                float afterHitStateTime = 0.5f;
                stateTimer = afterHitStateTime;
                if (targetUnit != null) {
                    targetUnit.Damage(100, shooterUnit: unit, targetUnit: targetUnit, this);
                } else {
                    targetBarrel.Damage();
                }
                OnAnySwordHit?.Invoke(this, EventArgs.Empty);
                break;
            case State.SwingingSwordAfterHit:
                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;  
            default:
                break;
        }
    }

    public override string GetActionName() {
        return "Sword";
    }

    public override EnemyAIAction GetBestEnemyAIAction(GridPosition gridPosition, out BaseAction baseAction) {
        baseAction = this;
        return new EnemyAIAction() {
            baseAction = this,
            gridPosition = gridPosition,
            actionValue = 300,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList() {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxSwordDistance; x <= maxSwordDistance; x++) {
            for (int z = -maxSwordDistance; z <= maxSwordDistance; z++) {
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }

                bool hasBarrel = LevelGrid.Instance.HasAnyBarrelOnGridPosition(testGridPosition);
                bool hasUnit = LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition);
                if (!hasBarrel && !hasUnit) {
                    continue;
                }
                
                if (hasUnit) {
                    Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                    if (targetUnit.IsEnemy() == unit.IsEnemy()) {
                        // Both Units on same team
                        continue;
                    }
                }

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete) {
        if (LevelGrid.Instance.GetUnitAtGridPosition(gridPosition) == null) {
            targetBarrel = LevelGrid.Instance.GetDestructableBarrelAtPosition(gridPosition);
            targetUnit = null;
        } else {
            targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            targetBarrel = null;
        }

        state = State.SwingingSwordBeforeHit;
        float beforeHitStateTime = 0.7f;
        stateTimer = beforeHitStateTime;

        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }

    public int GetMaxSwordDistance() {
        return maxSwordDistance;
    }

    public Unit GetTargetUnit() {
        return targetUnit;
    }

    public override int GetActionPointsCost() {
        return 2;
    }
}
