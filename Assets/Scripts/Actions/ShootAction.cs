using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShootAction : BaseAction {

    private int maxShootDistance = 7;
    private float stateTimer;
    private Unit targetUnit;
    private bool canShootBullet;

    [SerializeField] private LayerMask obstaclesLayerMask;

    public static event EventHandler<OnShootEventArgs> OnAnyShoot;

    new public static void ResetStaticData() {
        OnAnyShoot = null;
    }

    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs {
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    private enum State {
        Aiming,
        Shooting,
        Cooloff
    }

    private State state;

    private void Update() {
        if (!isActive) {
            return;
        }
        stateTimer -= Time.deltaTime;
        switch (state) {
            case State.Aiming:
                Vector3 aimDirection = (targetUnit.GetWorldPosition() - transform.position).normalized;
                aimDirection.y = 0f;
                float rotateSpeed = 15f;
                transform.forward = Vector3.Slerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
                break;
            case State.Shooting:
                if (canShootBullet) {
                    Shoot();
                    canShootBullet = false;
                }
                break;
            case State.Cooloff:
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
            case State.Aiming:
                state = State.Shooting;
                float shootingStateTime = 0.1f;
                stateTimer = shootingStateTime;
                break;
            case State.Shooting:
                state = State.Cooloff;
                float cooloffStateTime = 0.5f;
                stateTimer = cooloffStateTime;
                break;
            case State.Cooloff:
                ActionComplete();
                break;
            default:
                break;
        }
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition) {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++) {
            for (int z = -maxShootDistance; z <= maxShootDistance; z++) {
                for (int floor = -maxShootDistance; floor <= maxShootDistance; floor++) {
                    GridPosition offsetGridPosition = new GridPosition(x, z, floor);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                        continue;
                    }

                    int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                    if (testDistance > maxShootDistance) {
                        continue;
                    }

                    if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) {
                        // Grid position is empty, no Unit to select here
                        continue;
                    }

                    Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                    if (targetUnit.IsEnemy() == unit.IsEnemy()) {
                        // Both Units on same team
                        continue;
                    }

                    Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                    Vector3 shootDir = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;
                    float unitShoulderHeight = 1.7f;
                    if (Physics.Raycast(unitWorldPosition +
                        Vector3.up * unitShoulderHeight,
                        shootDir,
                        Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                        obstaclesLayerMask)) {
                        // Blocked by an obstacle
                        continue;
                    }

                    validGridPositionList.Add(testGridPosition);
                }
            }
        }
        return validGridPositionList;
    }

    public override List<GridPosition> GetValidActionGridPositionList() {
        return GetValidActionGridPositionList(unit.GetGridPosition());
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete) {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        canShootBullet = true;

        state = State.Aiming;
        float aimingStateTime = 1f;
        stateTimer = aimingStateTime;

        ActionStart(onActionComplete);
    }

    private void Shoot() {
        OnAnyShoot?.Invoke(this, new OnShootEventArgs { targetUnit = targetUnit, shootingUnit = unit });
        OnShoot?.Invoke(this, new OnShootEventArgs { targetUnit = targetUnit, shootingUnit = unit });

        targetUnit.Damage(30, shooterUnit: unit, targetUnit: targetUnit, this);
    }

    public override string GetActionName() {
        return "Shoot";
    }

    public Unit GetTargetUnit() {
        return targetUnit;
    }

    public int GetMaxShootDistance() {
        return maxShootDistance;
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition) {
       return GetValidActionGridPositionList(gridPosition).Count;
    }

    public override EnemyAIAction GetBestEnemyAIAction(GridPosition gridPosition, out BaseAction baseAction) {
        baseAction = this;
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        return new EnemyAIAction {
            baseAction = this,
            gridPosition = gridPosition,
            actionValue = 100 + Mathf.RoundToInt((1 - targetUnit.GetHealthNormalized()) * 100f),
        };
    }

}
