using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionPointsText;

    [SerializeField] private Unit unit;

    [SerializeField] private Image healthBarImage;

    [SerializeField] private HealthSystem healthSystem;

    private void Start() {
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        healthSystem.OnHealthChange += HealthSystem_OnHealthChange;

        UpdateActionPointsText();
        UpdateHealthBar();
    }

    private void HealthSystem_OnHealthChange(object sender, System.EventArgs e) {
        UpdateHealthBar();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, System.EventArgs e) {
        UpdateActionPointsText();
    }

    private void UpdateActionPointsText() {
        actionPointsText.text = unit.GetActionPoints().ToString();
    }

    private void UpdateHealthBar() {
        float currentHealth = healthSystem.GetHealthNormalized();
        healthBarImage.fillAmount = currentHealth;
     
    }
}
