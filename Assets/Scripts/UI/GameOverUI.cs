using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    private void Start() {
        GameHandler.Instance.OnGameOverEvent += GameHandler_OnGameOverEvent;
        Hide();
    }

    private void GameHandler_OnGameOverEvent(object sender, System.EventArgs e) {
        if (GameHandler.Instance.IsGameOver()) {
            Show();
        } else {
            Hide();
        }
    }

    private void Update() {
        if (GameHandler.Instance.IsGamePlaying()) {
            return;
        }

        if (GameHandler.ENEMY_WINS) {
            textMeshProUGUI.text = "ENEMY WINS!";
            textMeshProUGUI.color = Color.red;
        } else {
            textMeshProUGUI.text = "YOU WIN!";
            textMeshProUGUI.color = Color.green;
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
