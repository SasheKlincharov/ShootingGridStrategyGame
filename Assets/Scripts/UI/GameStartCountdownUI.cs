using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI countdownText;

    private void Update() {
        countdownText.text = Mathf.CeilToInt(GameHandler.Instance.GetCountdownToStartTimer()).ToString();
    }

    private void Start() {
        GameHandler.Instance.OnGameStateChanged += GameHandler_OnStateChanged;
        Hide();
    }

    private void GameHandler_OnStateChanged(object sender, System.EventArgs e) {
        if (GameHandler.Instance.IsCountdownToStartActive()) {
            Show();
        } else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
