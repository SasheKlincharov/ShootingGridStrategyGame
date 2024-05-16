using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSystem : MonoBehaviour {
    private int turnNumber = 1;
    public event EventHandler OnTurnChanged;
    private bool isPlayerTurn = true;
    public static TurnSystem Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There's more than one TurnSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void NextTurn() {
        turnNumber++;
        isPlayerTurn = !isPlayerTurn;
        OnTurnChanged?.Invoke(this, EventArgs.Empty);
        if (UnitManager.Instance.GetEnemyUnitList().Count == 0) {
            GameHandler.Instance.EndGame("blue");
            return;
        } else if (UnitManager.Instance.GetFriendlyUnitList().Count == 0) {
            GameHandler.Instance.EndGame("red");
            return;
        }

    }

    public bool IsPlayerTurn() {
        return isPlayerTurn;
    }

    public int GetTurnNumber() {
        return turnNumber;
    }
}
