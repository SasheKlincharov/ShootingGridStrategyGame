using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameHandler : MonoBehaviour {
    public enum State {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }


    public event EventHandler OnGameStateChanged;
    public event EventHandler OnGameOverEvent;

    public event EventHandler OnAnyKill;

    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnPaused;


    private State state;
    private float waitingToStartTimer = 1f;
    private float countdownToStartTimer = 3f;

    public static bool ENEMY_WINS { get; set; }

    private bool isGamePaused = false;

    public static GameHandler Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There's more than one GameHandler! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        state = State.WaitingToStart;
    }

    private void Start() {
        ENEMY_WINS = false;
        InputManager.Instance.OnPauseAction += InputManager_OnPauseAction;
    }

    private void InputManager_OnPauseAction(object sender, EventArgs e) {
        TogglePauseGame();
    }


    public void EndGame(string winner) {
        if (winner == "blue") {
            ENEMY_WINS = false;
        } else {
            ENEMY_WINS = true;
        }

        state = State.GameOver;
        OnGameOverEvent?.Invoke(this, EventArgs.Empty);
    }

    private void Update() {
        if (IsGameOver()) {
            return;
        }

        switch (state) {
            case State.WaitingToStart:
                waitingToStartTimer -= Time.deltaTime;
                if (waitingToStartTimer < 0f) {
                    state = State.CountdownToStart;
                    OnGameStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f) {
                    state = State.GamePlaying;
                    OnGameStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
                break;
            case State.GameOver:
                break;
        }
    }

    public void TogglePauseGame() {
        isGamePaused = !isGamePaused;
        if (isGamePaused) {
            Time.timeScale = 0f;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        } else {
            OnGameUnPaused?.Invoke(this, EventArgs.Empty); 
            Time.timeScale = 1f;
        }
    }

    public bool IsGamePlaying() {
        return state == State.GamePlaying;
    }

    public bool IsCountdownToStartActive() {
        return state == State.CountdownToStart;
    }
    public bool IsGameOver() {
        return state == State.GameOver;
    }

    public float GetCountdownToStartTimer() {
        return countdownToStartTimer;
    }
}
