using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour {
    [SerializeField] private Unit unit;

    private MeshRenderer meshRenderer;

    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start() {
        UpdateVisual();
        UnitActionSystem.Instance.OnSelectedUnitChange += UnitActionSystem_OnSelectedUnitChange;
    }

    private void UnitActionSystem_OnSelectedUnitChange(object sender, System.EventArgs e) {
       UpdateVisual();
    }

    private void Show() {
        meshRenderer.gameObject.SetActive(true);
    }

    private void Hide() {
        meshRenderer.gameObject.SetActive(false);
    }

    private void UpdateVisual() {
        if (unit == UnitActionSystem.Instance.GetSelectedUnit()) {
            Show();
        } else {
            Hide();
        }
    }

    private void OnDestroy() {
        UnitActionSystem.Instance.OnSelectedUnitChange -= UnitActionSystem_OnSelectedUnitChange;
    }
}
