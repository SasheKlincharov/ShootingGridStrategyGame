using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact(Action onInteractionComplete);
    int GetPriority(int priorityMultiplier = 100);

    GridPosition GetGridPosition();
}
