using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private GridPosition gridPosition;
    private int gCost;
    private int hCost;
    private int fCost;
    private bool isWalkable = true;

    private PathNode predecessorPathNode;
    public PathNode(GridPosition gridPosition) {
        this.gridPosition = gridPosition;
    }

    public PathNode GetPredecessorPathNode() {
        return predecessorPathNode;
    }
    
    public void SetPredecessorPathNode(PathNode predecessorPathNode) {
        this.predecessorPathNode = predecessorPathNode;
    }

    public void ResetPredecessorPathNode() {
        this.predecessorPathNode = null;
    }

    public override string ToString() {
        return gridPosition.ToString();
    }

    public void SetGCost(int gCost) {
        this.gCost = gCost;
    }

    public void SetHCost(int hCost) {
        this.hCost = hCost;
    }

    public void CalculateFCost() {
        fCost = gCost + hCost;
    }

    public int GetGCost() {
        return gCost;
    }

    public int GetHCost() {
        return hCost;
    }

    public int GetFCost() {
        return fCost;
    }

    public GridPosition GetGridPosition() {
        return gridPosition;
    }

    public void SetIsWalkable(bool isWalkable) {
        this.isWalkable = isWalkable;
    }

    public bool IsWalkable() {
        return isWalkable;
    }
}
