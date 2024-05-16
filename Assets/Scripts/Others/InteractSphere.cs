using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class InteractSphere : MonoBehaviour, IInteractable
{
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material yellowMaterial;
    [SerializeField] private Material redMaterial;

    [SerializeField] private MeshRenderer meshRendererSphere;

    public static event EventHandler OnAnyDestroyed;
    public static event EventHandler OnAnyUpdated;


    private GridPosition gridPosition;
    private Action onInteractComplete;

    private bool isActive;
    private bool sphereAlive;
    private float timer;

    public static void ResetStaticData() {
        OnAnyDestroyed = null;
        OnAnyUpdated = null;
    }

    private enum State {
        POWER_RED,
        POWER_YELLOW,
        POWER_GREEN,
    }

    private State state;

    private void Start() {
        SetSpherePower();
        sphereAlive = true;

        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);

        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
    }

    private void Update() {
        if (!isActive) {
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f) {
            isActive = false;
            onInteractComplete();
            if (ShouldDestroySphere()) {
                LevelGrid.Instance.ClearInteractableAtGridPosition(gridPosition);
                Destroy(gameObject);
                OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public GridPosition GetGridPosition() {
        return gridPosition;
    }

    private void SetSpherePower() {
        float rndNumber = Random.Range(-1f, +1f);
        if (rndNumber > 0f) {
            UpdateSpherePower(State.POWER_GREEN, greenMaterial);
        } else {
            UpdateSpherePower(State.POWER_YELLOW, yellowMaterial);
        }
    }

    private void UpdateSpherePower(State newSphereState, Material newMaterial) {
        meshRendererSphere.material = newMaterial;
        state = newSphereState;
        OnAnyUpdated?.Invoke(this, EventArgs.Empty);
    }

    public void Interact(Action onInteractionComplete) {
        this.onInteractComplete = onInteractionComplete;

        if (state == State.POWER_GREEN) {
            UpdateSpherePower(State.POWER_YELLOW, yellowMaterial);
            state = State.POWER_YELLOW;
        } else if (state == State.POWER_YELLOW) {
            UpdateSpherePower(State.POWER_RED, redMaterial);
        } else {
            // It is Red Sphere
            sphereAlive = false;
        }
        
        isActive = true;
        timer = .5f;
    }

    public bool ShouldDestroySphere() {
        return !sphereAlive;
    }

    public int GetPriority(int priorityMultiplier = 1) {
        return 3 * priorityMultiplier;
    }
}
