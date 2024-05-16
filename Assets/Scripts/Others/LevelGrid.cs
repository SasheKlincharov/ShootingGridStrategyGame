using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGrid : MonoBehaviour {

    public static LevelGrid Instance { get; private set; }

    public const float FLOOR_HEIGHT = 3f;

    private List<GridSystem<GridObject>> gridSystemList;

    [SerializeField] private Transform gridDebugObjectPrefab;

    public event EventHandler<OnAnyUnitMovedGridPositionEventArgs> OnAnyUnitMovedGridPosition;
    public class OnAnyUnitMovedGridPositionEventArgs : EventArgs {
        public Unit unit;
        public GridPosition fromGridPosition;
        public GridPosition toGridPosition;
    }

    [SerializeField] private int floorAmount;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;


    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There's more than one UnitActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        gridSystemList = new List<GridSystem<GridObject>>();
        for (int floor = 0; floor < floorAmount; floor++) {
            GridSystem<GridObject> gridSystem = new GridSystem<GridObject>(width, height, cellSize, floor, FLOOR_HEIGHT, (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(gridPosition, g));
            gridSystemList.Add(gridSystem);
        }
        //gridSystem.CreateDebugObjects(gridDebugObjectPrefab);
    }

    private void Start() {
        Pathfinding.Instance.Setup(width, height, cellSize, floorAmount); 
    }

    private GridSystem<GridObject> GetGridSystem(int floor) {
        return gridSystemList[floor];
    }

    public int GetWidth() {
        return GetGridSystem(0).GetWidth();
    }

    public int GetHeight() {
        return GetGridSystem(0).GetHeight();
    }

    public int GetFloor(Vector3 worldPosition) {
        return Mathf.RoundToInt(worldPosition.y / FLOOR_HEIGHT);
    }

    public int GetFloorAmount() {
        return floorAmount;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition) {
        int floor = GetFloor(worldPosition);
        return GetGridSystem(floor).GetGridPosition(worldPosition);
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition) {
        return GetGridSystem(gridPosition.floor).GetWorldPosition(gridPosition);
    }

    public List<Unit> GetUnitListAtGridPosition(GridPosition gridPosition) {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetUnitList();
    }

    public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.AddUnit(unit);
    }

    public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit) {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.RemoveUnit(unit);
    }

    public void UnitMovedGridPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition) {
        RemoveUnitAtGridPosition(fromGridPosition, unit);

        AddUnitAtGridPosition(toGridPosition, unit);

        OnAnyUnitMovedGridPosition?.Invoke(this, new OnAnyUnitMovedGridPositionEventArgs {
            unit = unit,
            fromGridPosition = fromGridPosition,
            toGridPosition = toGridPosition
        });
    }

    public void ClearInteractableAtGridPosition(GridPosition gridPosition) {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.ClearInteractable();
    }

    public Boolean IsValidGridPosition(GridPosition gridPosition) {
        if (gridPosition.floor < 0 || gridPosition.floor >= floorAmount) {
            //Invalid
            return false;
        }

        return GetGridSystem(gridPosition.floor).IsValidGridPosition(gridPosition);
    }

    public Boolean HasAnyBarrelOnGridPosition(GridPosition gridPosition) {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.IsBarrel();
    }

    public Boolean HasAnyUnitOnGridPosition(GridPosition gridPosition) {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition) {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }

    public DestructableBarrel GetDestructableBarrelAtPosition(GridPosition gridPosition) {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetInteractable() as DestructableBarrel;
    }

    public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition) {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        return gridObject.GetInteractable();
    }

    public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable interactable) {
        GridObject gridObject = GetGridSystem(gridPosition.floor).GetGridObject(gridPosition);
        gridObject.SetInteractable(interactable);
    }

    public List<IInteractable> GetInteractablesAroundGridPosition(GridPosition gridPosition) {
        Vector3 worldPosition = GetWorldPosition(gridPosition);
        List<IInteractable> interactableList = new List<IInteractable>();
        
        for (int x = -1; x <= 1; x++) {
            for (int z = 1; z >= -1; z--) {
                GridPosition testGridPosition = GetGridPosition(new Vector3(worldPosition.x + (2 * x), 0, worldPosition.z + (2 * z)));
                if (!IsValidGridPosition(testGridPosition)) { continue; }
                
                //if (testGridPosition == gridPosition) {  continue; }
                if (GetGridSystem(gridPosition.floor).GetGridObject(testGridPosition).TryGetInteractable(out IInteractable interactable)) {
                    interactableList.Add(interactable);
                }
            }
        }
        return interactableList;
    }

    public List<Unit> GetUnitsAroundGridPosition(GridPosition gridPosition) {
        Vector3 worldPosition = GetWorldPosition(gridPosition);
        List<Unit> unitsList = new List<Unit>();

        for (int x = -1; x <= 1; x++) {
            for (int z = 1; z >= -1; z--) {
                GridPosition testGridPosition = GetGridPosition(new Vector3(worldPosition.x +  (2 * x), 0, worldPosition.z + (2 * z)));
                if (!IsValidGridPosition(testGridPosition)) {
                    continue;
                }

                if (testGridPosition == gridPosition) {
                    // This is the same position as the one we're checking
                    continue;
                }

                if (GetGridSystem(gridPosition.floor).GetGridObject(testGridPosition).TryGetUnit(out Unit unit)) {
                    unitsList.Add(unit);
                }
            }
        }
        return unitsList;
    }
}
