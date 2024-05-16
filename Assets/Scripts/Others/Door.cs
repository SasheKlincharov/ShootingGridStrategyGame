using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    public static event EventHandler OnDoorChange;
    public static event EventHandler OnAnyDoorOpened;
    public event EventHandler OnDoorOpened;

    private GridPosition gridPosition;
    
    private bool isOpen;

    private Animator animator;
    private Action onInteractComplete;
    private float timer;
    private bool isActive;

    public static void ResetStaticData() {
        OnDoorChange = null;
        OnAnyDoorOpened = null;
    }

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(gridPosition, this);

        if (isOpen) {
            OpenDoor();
        } else {
            CloseDoor();
        }
    }

    private void Update() {
        if (!isActive) {
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f) {
            isActive = false;
            onInteractComplete();
        }
    }

    public void Interact(Action onInteractComplete) {
        this.onInteractComplete = onInteractComplete;
        isActive = true;
        timer = .5f;
        if (isOpen) {
            CloseDoor();
        } else {
            OpenDoor();
        }
    }

    private void OpenDoor() {
        isOpen = true;
        animator.SetBool("IsOpen", isOpen);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);

        OnDoorOpened?.Invoke(this, EventArgs.Empty);
        OnAnyDoorOpened?.Invoke(this, EventArgs.Empty);
        OnDoorChange?.Invoke(this, EventArgs.Empty);
    }

    private void CloseDoor() {
        isOpen = false;
        animator.SetBool("IsOpen", isOpen);
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, false);
        OnDoorChange?.Invoke(this, EventArgs.Empty);
    }

    public int GetPriority(int priorityMultiplier = 100) {
        if (isOpen) {
            return -1;
        } else {
            return 1 * priorityMultiplier;
        }
    }

    public GridPosition GetGridPosition() {
        return gridPosition;
    }
}
