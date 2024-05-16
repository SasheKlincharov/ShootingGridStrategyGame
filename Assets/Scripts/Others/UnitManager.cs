using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    private List<Unit> unitList;
    private List<Unit> friendlyUnitList;
    private List<Unit> enemyUnitList;

    public static UnitManager Instance;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There's more than one UnitManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        unitList = new List<Unit>();
        friendlyUnitList = new List<Unit>();
        enemyUnitList = new List<Unit>();
    }

    private void Start() {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }

    private void Unit_OnAnyUnitSpawned(object sender, System.EventArgs e) {
        Unit unit = sender as Unit;

        if (unit.IsEnemy()) {
            enemyUnitList.Add(unit);
        } else {
            friendlyUnitList.Add(unit);
        }

        unitList.Add(unit);
    }

    private void Unit_OnAnyUnitDead(object sender, System.EventArgs e) {
        Unit unit = sender as Unit;

        if (unit.IsEnemy()) {
            enemyUnitList.Remove(unit);
        } else {
            friendlyUnitList.Remove(unit);
        }

        unitList.Remove(unit);
    }

    public List<Unit> GetUnitList() {
        return unitList;
    }

    public List<Unit> GetEnemyUnitList() {
        return enemyUnitList;
    }

    public List<Unit> GetFriendlyUnitList() {
        return friendlyUnitList;
    }

}
