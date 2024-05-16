using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject actionCameraGameObject;
    [SerializeField] private LayerMask ceilingLayerMask;
    private bool followEnemyUnit;
    private Unit enemyUnit;
    private bool smoothMoveCamera;
    private Vector3 actionCameraPosition;

    private void Start() {
        actionCameraPosition = actionCameraGameObject.transform.position;
        followEnemyUnit = false;
        smoothMoveCamera = false;
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
        EnemyAI.OnAnyEnemyAIActionStarted += EnemyAI_OnAnyEnemyAIActionStarted;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        HideActionCamera();
    }

    private void Update() {
        if (followEnemyUnit && enemyUnit != null) {
             actionCameraGameObject.transform.LookAt(enemyUnit.GetWorldPosition());
        }

        if (smoothMoveCamera) {
            float t = Mathf.Clamp01(Time.deltaTime * 2f);
            actionCameraGameObject.transform.position = Vector3.Lerp(actionCameraGameObject.transform.position, actionCameraPosition, t);

            if (Vector3.Distance(actionCameraGameObject.transform.position, actionCameraPosition) < 0.05) {
                smoothMoveCamera = false;
            }
        }

    }

    private void EnemyAI_OnAnyEnemyAIActionStarted(object sender, EnemyAI.OnAnyEnemyAIActionStartedEventArgs enemyAIActionStartedEventArgs) {
        if (enemyAIActionStartedEventArgs.action is ShootAction) {
            return;
        }
        bool changedUnit = true;
        if (enemyUnit == enemyAIActionStartedEventArgs.enemyUnit) {
            changedUnit = false;
        }

        enemyUnit = enemyAIActionStartedEventArgs.enemyUnit;
        // TODO: Refactor this code, it's chaos.

        float raycastDistance = 6f;
        EnemyAIAction enemyAction = enemyAIActionStartedEventArgs.enemyAIAction;
        float scalingFactor = 1.0f / (float)(enemyAction.gridPosition.floor + 1);
        Vector3 cameraCharacterHeight = Vector3.up * (enemyAction.gridPosition.floor + 1) * 3 * scalingFactor;
        if (Physics.Raycast(enemyUnit.GetWorldPosition() + Vector3.up + new Vector3(0, 0.05f, 0), Vector3.up, out RaycastHit raycastHit, raycastDistance, ceilingLayerMask)) {
            if (raycastHit.collider.gameObject.GetComponent<Plane>()) {
                CameraController.Instance.SetLowCameraHeight();
            } else {
                CameraController.Instance.SetHighCameraHeight();
            }

            float cameraOffsetAmount = 5.5f;
            Vector3 actionCameraPosition = enemyUnit.GetWorldPosition() +
                                           cameraCharacterHeight *
                                           cameraOffsetAmount;
            float shoulderOffsetAmount = 0.6f;
            Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * Vector3.forward * shoulderOffsetAmount;
            actionCameraPosition = actionCameraPosition + shoulderOffset + new Vector3(0, 0, -1) * 10;
            if (changedUnit || Vector3.Distance(actionCameraPosition, actionCameraGameObject.transform.position) > 7f) {
                this.actionCameraPosition = actionCameraPosition;
                smoothMoveCamera = true;
            }
        } else {
            CameraController.Instance.SetHighCameraHeight();
            float cameraOffsetAmount = 5.5f;
            Vector3 actionCameraPosition = enemyUnit.GetWorldPosition() +
                                           cameraCharacterHeight *
                                           cameraOffsetAmount;
            float shoulderOffsetAmount = 0.6f;
            Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * Vector3.forward * shoulderOffsetAmount;
            actionCameraPosition = actionCameraPosition + shoulderOffset + new Vector3(0, 0, -1) * 10;
            if (changedUnit || Vector3.Distance(actionCameraPosition, actionCameraGameObject.transform.position) > 7f) {
                this.actionCameraPosition = actionCameraPosition;
                smoothMoveCamera = true;
            }
        }
        // TODO: To here

        actionCameraGameObject.transform.LookAt(enemyUnit.GetWorldPosition() + cameraCharacterHeight);
        followEnemyUnit = true;
        ShowActionCamera();
    }


    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e) {
        HideActionCamera();
        CameraController.Instance.SetHighCameraHeight();
    }

    private void BaseAction_OnAnyActionCompleted(object sender, System.EventArgs e) {
        switch (sender) {
            case ShootAction shootAction:
                HideActionCamera();
                break;
            case SwordAction swordAction:
                HideActionCamera();
                break;
            default:
                HideActionCamera();
                break;
        }
    }

    private void BaseAction_OnAnyActionStarted(object sender, System.EventArgs e) {
        switch (sender) {
            case ShootAction shootAction:
                smoothMoveCamera = false;
                followEnemyUnit = false;
                Unit shootingUnit = shootAction.GetUnit();
                Unit targetUnit = shootAction.GetTargetUnit();

                Vector3 cameraCharacterHeight = Vector3.up * 1.7f;
                Vector3 shootDirection = (targetUnit.GetWorldPosition() - shootingUnit.GetWorldPosition()).normalized;
                
                float shoulderOffsetAmount = 0.6f;
                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDirection * shoulderOffsetAmount;

                Vector3 actionCameraPosition = 
                    shootingUnit.GetWorldPosition() +
                    cameraCharacterHeight +
                    shoulderOffset +
                    (shootDirection * -1);

                actionCameraGameObject.transform.position = actionCameraPosition;
                actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);
                ShowActionCamera();
                break;
            case SwordAction swordAction:
                // Uncomment this section if want camera move when doing the sword action
                //Unit swordUnit = swordAction.GetUnit();
                //Unit swordTargetUnit = swordAction.GetTargetUnit();
              
                //Vector3 cameraCharacterSwordHeight = Vector3.up * 1.7f;
                //Vector3 swordDir = (swordTargetUnit.GetWorldPosition() - swordUnit.GetWorldPosition()).normalized;
                //float shoulderOffsetSwordAmount = 0.5f;
                //Vector3 shoulderSwordOffset = Quaternion.Euler(0, 90, 0) * swordDir * shoulderOffsetSwordAmount;


                //Vector3 actionCameraSwordPosition =
                //   swordUnit.GetWorldPosition() +
                //   cameraCharacterSwordHeight +
                //   shoulderSwordOffset +
                //   (swordDir * -1);

                //actionCameraGameObject.transform.position = actionCameraSwordPosition;
                //actionCameraGameObject.transform.LookAt(swordUnit.GetWorldPosition() + cameraCharacterSwordHeight);

                //ShowActionCamera();
                break;
        }
    }

    private void ShowActionCamera() {
        actionCameraGameObject.SetActive(true);
    }

    private void HideActionCamera() {
        followEnemyUnit = false;
        actionCameraGameObject.SetActive(false);
    }
}
