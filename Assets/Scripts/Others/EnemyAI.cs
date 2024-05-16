using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public static event EventHandler<OnAnyEnemyAIActionStartedEventArgs> OnAnyEnemyAIActionStarted;

    public static void ResetStaticData() {
        OnAnyEnemyAIActionStarted = null;
    }

    public class OnAnyEnemyAIActionStartedEventArgs : EventArgs {
        public BaseAction action;
        public Unit enemyUnit;
        public EnemyAIAction enemyAIAction;
    }

    private enum State {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy
    }

    private State state;

    private float timer;


    private void Awake() {
        state = State.WaitingForEnemyTurn;
    }

    private void Start() {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, System.EventArgs e) {
        if (!TurnSystem.Instance.IsPlayerTurn()) {
            state = State.TakingTurn;
            timer = 2f;
        }
    }

    private void Update() {
        if (!GameHandler.Instance.IsGamePlaying()) {
            return;
        }

        if (TurnSystem.Instance.IsPlayerTurn()) {
            return;
        }

        switch (state) {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer < 0) {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn)) {
                        state = State.Busy;
                    } else {
                        // No more enemies that can take action, so we end the enemy turn
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:
                break;
        }
    }

    private void SetStateTakingTurn() {
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete) {
        if (UnitManager.Instance.GetFriendlyUnitList().Count == 0) {
            return false;
        }

        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList()) {
            if (TryTakeEnemyAIAction(enemyUnit, onEnemyAIActionComplete)) {
                return true;
            }
        }
        return false;
    }

    public bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete) {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;     

        foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray()) {
            if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction)) {
                // Enemy cannot afford this action
                continue;
            }


            if (bestEnemyAIAction == null) {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            } else {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue) {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
        }

        if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction)) {
            GridPosition actionUnitPosition = enemyUnit.GetGridPosition();
           
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);

            OnAnyEnemyAIActionStarted?.Invoke(this, new OnAnyEnemyAIActionStartedEventArgs { action = bestBaseAction, enemyAIAction = bestEnemyAIAction, enemyUnit = enemyUnit });

            return true;
        } else {
            return false;
        }
    }   
}
