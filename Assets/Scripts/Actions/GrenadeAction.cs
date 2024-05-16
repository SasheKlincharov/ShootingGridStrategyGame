using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction {

    private int maxThrowDistance = 5;

    [SerializeField] private Transform grenadeProjectilePrefab;
    private void Update() {
        if (!isActive) {
            return;
        }
    }

    public override string GetActionName() {
        return "Grenade";
    }

    public override EnemyAIAction GetBestEnemyAIAction(GridPosition gridPosition, out BaseAction baseAction) {
        baseAction = this;
        return new EnemyAIAction() { baseAction = this, gridPosition = gridPosition, actionValue = 0 };
    }

    public override List<GridPosition> GetValidActionGridPositionList() {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxThrowDistance; x <= maxThrowDistance; x++) {
            for (int z = -maxThrowDistance; z <= maxThrowDistance; z++) {
                for (int floor = -maxThrowDistance; floor <= maxThrowDistance; floor++) {
                    GridPosition offsetGridPosition = new GridPosition(x, z, floor);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                        continue;
                    }

                    if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition)) {
                        continue;
                    }


                    if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition, out int pathLength)) {
                        continue;
                    }

                    int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                    if (testDistance > maxThrowDistance) {
                        continue;
                    }

                    validGridPositionList.Add(testGridPosition);
                }
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete) {
        Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, unit.GetWorldPosition(), Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(unit.GetGridPosition(), gridPosition, this, OnGrenadeBehaviourComplete);


        ActionStart(onActionComplete);
    }

    private void OnGrenadeBehaviourComplete() {
        ActionComplete();
    }

    public int GetMaxThrowDistance() {
        return maxThrowDistance;
    }

    public override int GetActionPointsCost() {
        return 2;
    }

    public Vector3 GetProjectileSpawnPosition() {
        if (grenadeProjectilePrefab != null) { 
            return grenadeProjectilePrefab.transform.position;
        }

        return Vector3.zero;
    }
}
