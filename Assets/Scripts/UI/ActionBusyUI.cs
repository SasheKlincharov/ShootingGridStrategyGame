using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{

    private void Start() {
        Hide();
        UnitActionSystem.Instance.OnBusyChange += UnitActionSystem_OnBusyChange;
    }

    private void UnitActionSystem_OnBusyChange(object sender, bool isBusy) {
        if (isBusy) {
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
