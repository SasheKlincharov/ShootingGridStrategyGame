using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveAction : BaseAction
{
    [SerializeField] int maxMoveDistance = 5;
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    public event EventHandler<OnChangeFloorsStartedEventArgs> OnChangeFloorsStarted;
    public class OnChangeFloorsStartedEventArgs : EventArgs {
        public GridPosition unitGridPosition;
        public GridPosition targetGridPosition;
    }

    public static event EventHandler OnAnyMovementStarted;
    public static event EventHandler OnAnyMovementStopped;

    private Vector3 targetPosition;

    private List<Vector3> positionList;
    private int currentPositionIndex;

    private bool isChaningFloors;

    private float differentFloorsTeleportTimer;
    private float differentFloorsTeleportTimerMax = .5f;

    new public static void ResetStaticData() {
        OnAnyMovementStarted = null;
        OnAnyMovementStopped = null;
    }

    protected override void Awake() {
        base.Awake();
        targetPosition = transform.position;
    }

    private void Update() {
        if (!isActive) {
            return;
        }
        Vector3 targetPosition = positionList[currentPositionIndex];
        float rotateSpeed = 10f;

        if (isChaningFloors) {
            // Stop and teleport to position logic, rotate towards that floor position direction

            Vector3 targetSameFloorPosition = targetPosition;
            targetSameFloorPosition.y = transform.position.y;

            Vector3 rotateDirection = (targetSameFloorPosition - transform.position).normalized;
            transform.forward = Vector3.Slerp(transform.forward, rotateDirection, Time.deltaTime * rotateSpeed);

            differentFloorsTeleportTimer -= Time.deltaTime;
            if (differentFloorsTeleportTimer < 0f) {
                //Teleport unit
                isChaningFloors = false;
                transform.position = targetPosition;
            }
        } else {
            // Regular move logic
            Vector3 moveDirection = (targetPosition - transform.position).normalized;

            transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

            float moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }

        float stoppingDistance = .1f;
        if (Vector3.Distance(transform.position, targetPosition) < stoppingDistance) {
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count) {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                OnAnyMovementStopped?.Invoke(this, EventArgs.Empty);

                ActionComplete();
            } else {
                GridPosition unitGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
                GridPosition targetGridPosition = LevelGrid.Instance.GetGridPosition(positionList[currentPositionIndex]);

                if (targetGridPosition.floor != unitGridPosition.floor) {
                    // Going to different floor
                    isChaningFloors = true;
                    differentFloorsTeleportTimer = differentFloorsTeleportTimerMax;
                    OnChangeFloorsStarted?.Invoke(this, new OnChangeFloorsStartedEventArgs { 
                        unitGridPosition = unitGridPosition,
                        targetGridPosition = targetGridPosition
                    });
                }
            }
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete) {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);
        currentPositionIndex = 0;
        positionList = new List<Vector3>();

        foreach (GridPosition pathGridPosition in pathGridPositionList) {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        OnStartMoving?.Invoke(this, EventArgs.Empty);
        OnAnyMovementStarted?.Invoke(this, EventArgs.Empty); 
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList() {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++) {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++) {
                for (int floor = -maxMoveDistance; floor <= maxMoveDistance; floor++) {
                    GridPosition offsetGridPosition = new GridPosition(x, z, floor);
                    GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                    if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                        continue;
                    }
                    if (unitGridPosition == testGridPosition) {
                        continue;
                    }

                    if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) {
                        continue;
                    }


                    if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition)) {
                        continue;
                    }

                    if (!Pathfinding.Instance.HasPath(unitGridPosition, testGridPosition, out int pathLength)) {
                        continue;
                    }

                    int pathFindingDistanceMultiplier = 10;
                    if (pathLength > (maxMoveDistance * pathFindingDistanceMultiplier)) {
                        continue;
                    }
                    validGridPositionList.Add(testGridPosition);
                }
               
            }
        }
        return validGridPositionList;
    }

    public override string GetActionName() {
        return "Move";
    }

    public override EnemyAIAction GetBestEnemyAIAction(GridPosition gridPosition, out BaseAction baseAction) {
        baseAction = this;
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);
        return new EnemyAIAction {
            baseAction = this,
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }

}
