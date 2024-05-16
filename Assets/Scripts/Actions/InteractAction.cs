using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class InteractAction : BaseAction {

    private int maxInteractDistance = 1;

    private void Update() {
        if(!isActive) {
            return;
        }
    }

    public override string GetActionName() {
        return "Interact";
    }

    public override EnemyAIAction GetBestEnemyAIAction(GridPosition gridPosition, out BaseAction baseAction) {
        baseAction = this;
        List<IInteractable> interactables = LevelGrid.Instance.GetInteractablesAroundGridPosition(gridPosition);
        
        interactables.Sort((IInteractable interactableA, IInteractable interactableB) => interactableA.GetPriority() - interactableB.GetPriority());
        return new EnemyAIAction {
            baseAction = this,
            gridPosition = gridPosition,
            actionValue = interactables.Count > 0 ? interactables[0].GetPriority(priorityMultiplier : 100) : 0,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList() {

        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxInteractDistance; x <= maxInteractDistance; x++) {
            for (int z = -maxInteractDistance; z <= maxInteractDistance; z++) {
                GridPosition offsetGridPosition = new GridPosition(x, z, 0);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;
                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) {
                    continue;
                }
                IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);
                
                if (interactable is DestructableBarrel) {
                    continue;
                }

                if (interactable == null) {
                    // There is no door on this grid position
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete) {
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);
        interactable.Interact(OnInteractComplete);

        ActionStart(onActionComplete);
    }

    private void OnInteractComplete() {
        ActionComplete();
    }

}
