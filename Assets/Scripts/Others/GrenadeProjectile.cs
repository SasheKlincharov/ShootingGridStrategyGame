using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GrenadeProjectile : MonoBehaviour
{
    [SerializeField] private Transform grenadeExplodeVFXPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;

    private Vector3 targetPosition;
    private GridPosition unitGridPosition;

    private GrenadeAction grenadeAction;

    private Action OnGrenadeBehaviourComplete;
    public static event EventHandler<OnAnyGrenadeExplodeEventArgs> OnAnyGrenadeExploded;

    public class OnAnyGrenadeExplodeEventArgs : EventArgs {
        public Vector3 targetGrenadePosition;
    }

    private float totalDistance;
    private Vector3 positionXZ;

    public static void ResetStaticData() {
        OnAnyGrenadeExploded = null;
    }

    private void Update() {
        Vector3 moveDir = (targetPosition - positionXZ).normalized;
        float moveSpeed = 15f;
        positionXZ += moveDir * moveSpeed * Time.deltaTime;

        float distance = Vector3.Distance(positionXZ, targetPosition);
        float distanceNormalized = 1 - distance / totalDistance;
        int unitFloor = LevelGrid.Instance.GetFloor(LevelGrid.Instance.GetWorldPosition(unitGridPosition));

        float maxHeight = totalDistance / 4f + LevelGrid.Instance.GetFloor(targetPosition) * LevelGrid.FLOOR_HEIGHT;
        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
        transform.position = new Vector3(positionXZ.x, positionY + unitFloor * LevelGrid.FLOOR_HEIGHT, positionXZ.z);
        int targetFloor = LevelGrid.Instance.GetFloor(targetPosition);
        float reachedTargetDistance = .3f + targetFloor;
 
        if (Vector3.Distance(positionXZ, targetPosition) <= reachedTargetDistance) {
            float damageRadius = 3f;
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, damageRadius);
            foreach (Collider collider in colliderArray) {
                if (collider.TryGetComponent<Unit>(out Unit targetUnit)) {
                    if (targetUnit.GetGridPosition().floor != targetFloor) continue;
                    targetUnit.Damage(30, shooterUnit: LevelGrid.Instance.GetUnitAtGridPosition(unitGridPosition), targetUnit: targetUnit, grenadeAction);
                }
                if (collider.TryGetComponent<DestructableCrate>(out DestructableCrate crate)) {
                    if (crate.GetGridPosition().floor != targetFloor) continue;
                    crate.Damage();
                }
                if (collider.TryGetComponent<DestructableBarrel>(out DestructableBarrel barrel)) {
                    if (barrel.GetGridPosition().floor != targetFloor) continue;
                    barrel.Damage();
                }
            }

            OnAnyGrenadeExploded?.Invoke(this, new OnAnyGrenadeExplodeEventArgs { targetGrenadePosition = targetPosition }) ;

            trailRenderer.transform.parent = null; 
            Instantiate(grenadeExplodeVFXPrefab, targetPosition + Vector3.up * 1f, Quaternion.identity);

            Destroy(gameObject);
            OnGrenadeBehaviourComplete();
        }
    }

    public void Setup(GridPosition unitGridPosition, GridPosition targetGridPosition, GrenadeAction grenadeAction, Action OnGrenadeBehaviourComplete) {
        this.grenadeAction = grenadeAction;
        this.OnGrenadeBehaviourComplete = OnGrenadeBehaviourComplete;
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);

        this.unitGridPosition = unitGridPosition;
        positionXZ = transform.position;
        positionXZ.y = 0f;
        totalDistance = Vector3.Distance(positionXZ, targetPosition);
    }
}
