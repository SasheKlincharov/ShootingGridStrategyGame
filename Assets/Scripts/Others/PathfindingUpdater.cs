using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingUpdater : MonoBehaviour
{
    private void Start() {
        DestructableCrate.OnAnyDestroyed += DestructableCrate_OnAnyDestroyed;
        DestructableBarrel.OnAnyDestroyed += DestructableBarrel_OnAnyDestroyed;
        InteractSphere.OnAnyDestroyed += InteractSphere_OnAnyDestroyed;
    }

    private void InteractSphere_OnAnyDestroyed(object sender, System.EventArgs e) {
        InteractSphere sphere = sender as InteractSphere;
        Pathfinding.Instance.SetIsWalkableGridPosition(sphere.GetGridPosition(), true);
    }

    private void DestructableBarrel_OnAnyDestroyed(object sender, System.EventArgs e) {
        DestructableBarrel destructableCrate = sender as DestructableBarrel;
        Pathfinding.Instance.SetIsWalkableGridPosition(destructableCrate.GetGridPosition(), true);
    }

    private void DestructableCrate_OnAnyDestroyed(object sender, System.EventArgs e) {
        DestructableCrate destructableCrate = sender as DestructableCrate;
        Pathfinding.Instance.SetIsWalkableGridPosition(destructableCrate.GetGridPosition(), true);
    }
}
