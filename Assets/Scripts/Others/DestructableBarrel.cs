using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class DestructableBarrel : MonoBehaviour, IInteractable {
    public static event EventHandler OnAnyDestroyed;
    public static event EventHandler OnAnyDamaged;

    [SerializeField] private GameObject visualGameObject;
    [SerializeField] private Transform barrelDestroyedPrefab;

    private Action onInteractionComplete;

    private bool isActive;
    private float timer;
    private static int defaultMaxBarrelHealth = 100;
    private int barrelHealth;

    private int countDamageTimes = 0;

    private GridPosition gridPosition;

    public static void ResetStaticData() {
        OnAnyDestroyed = null;
        OnAnyDamaged = null;
    }

    private void Start() {
        InitBarrelHealth();

        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
    }

    private void InitBarrelHealth() {
        countDamageTimes = 0;
        barrelHealth = defaultMaxBarrelHealth;

        float rndNumber = Random.Range(-1f, +1f);
        if (rndNumber < 0.5f) {
            barrelHealth = 50;
        } else {
            barrelHealth = 100;
        }
    }

    private void Update() {
        if (!isActive) {
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f) {
            isActive = false;
            if (barrelHealth <= 0) {
                LevelGrid.Instance.ClearInteractableAtGridPosition(gridPosition);

                Destroy(gameObject);
                OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void Damage() {
        barrelHealth -= 50;
        countDamageTimes += 1;

        if (barrelHealth <= 0) {
            Transform barrelDestroyedTransform = Instantiate(barrelDestroyedPrefab, transform.position, transform.rotation);
            ApplyExplosionToChildren(barrelDestroyedTransform, 150f, transform.position, 10f);
            OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
            visualGameObject.SetActive(false);

            // Regenerate health based on barrel Strength 
            List<Unit> nearbyUnitsList = LevelGrid.Instance.GetUnitsAroundGridPosition(gridPosition);

            foreach (Unit unit in nearbyUnitsList) {
                if (unit.IsFullHealth()) {
                    continue;
                } else {
                    unit.HealUp(countDamageTimes * 30);
                }
            }
            countDamageTimes = 0;
        } else {
            OnAnyDamaged?.Invoke(this, EventArgs.Empty);
        }

        isActive = true;
        timer = .5f;
    }

    private void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange) {
        foreach (Transform child in root) {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidBody)) {
                childRigidBody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRange);
        }
    }

    public void Interact(Action onInteractionComplete) {
        this.onInteractionComplete = onInteractionComplete;
    }

    public GridPosition GetGridPosition() {
        return gridPosition;
    }

    public Vector3 GetWorldPosition() {
        return LevelGrid.Instance.GetWorldPosition(gridPosition);
    }

    public int GetPriority(int priorityMultiplier = 1) {
        return 1 * priorityMultiplier;
    }
}
