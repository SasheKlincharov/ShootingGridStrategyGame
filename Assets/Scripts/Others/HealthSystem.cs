using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int health = 40;

    private int healthMax;

    public event EventHandler OnDead;

    public event EventHandler OnHealthChange;

    private void Awake() {
        healthMax = health;
    }

    public void Damage(int damageAmount, Unit shooterUnit, Unit targetUnit, BaseAction baseAction) {
        health -= damageAmount;

        if (health < 0) {
            health = 0;
        }

        OnHealthChange?.Invoke(this, EventArgs.Empty);

        if (health == 0) {
            Die(shooterUnit, targetUnit, baseAction);
        }
    }

    public void AddHealth(int amount) {
        if (amount + health > healthMax) {
            health = healthMax;
        } else {
            health += amount;
        }

        OnHealthChange?.Invoke(this, EventArgs.Empty);
    }

    public int GetHealth() {
        return health;
    }

    private void Die(Unit shooterUnit, Unit targetUnit, BaseAction killedByAction) {
        OnDead?.Invoke(this, EventArgs.Empty);
        //GameHandler.Instance.AddKill(new KillDetail(shooterUnit, targetUnit, killedByAction));
    }

    public float GetHealthNormalized() {
        return (float)health / healthMax;
    }
}
