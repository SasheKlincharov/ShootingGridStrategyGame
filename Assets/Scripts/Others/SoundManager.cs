using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioClip shootAudio;
    [SerializeField] private AudioClip[] walkAudio;
    [SerializeField] private AudioClip grenadeThrowAudio;
    [SerializeField] private AudioClip grenadeExplodeAudio;
    [SerializeField] private AudioClip[] jumpUpAudio;
    [SerializeField] private AudioClip[] jumpDownAudio;

    [SerializeField] private AudioClip woodBreakingAudio;
    [SerializeField] private AudioClip impactWoodBreakingAudio;

    [SerializeField] private AudioClip healSound;
    [SerializeField] private AudioClip openCloseDoorSound;

    [SerializeField] private AudioClip swordSliceAudio;

    [SerializeField] private AudioClip interactSphereAudio;
    [SerializeField] private AudioClip destroySphereAudio;


    private float walkTimer;
    private float walkTimerMax = .3f;
    private bool unitWalkingActive = false;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There's more than one SoundManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        unitWalkingActive = false;
    }

    private void Start() {
        unitWalkingActive = false;
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
        GrenadeProjectile.OnAnyGrenadeExploded += GrenadeProjectile_OnAnyGrenadeExploded;
        MoveAction.OnAnyMovementStarted += MoveAction_OnAnyMovementStarted;
        MoveAction.OnAnyMovementStopped += MoveAction_OnAnyMovementStopped;
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        DestructableCrate.OnAnyDestroyed += DestructableCrate_OnAnyDestroyed;
        DestructableBarrel.OnAnyDestroyed += DestructableBarrel_OnAnyDestroyed;
        DestructableBarrel.OnAnyDamaged += DestructableBarrel_OnAnyDamaged;
        Door.OnDoorChange += Door_OnDoorChange;
        InteractSphere.OnAnyDestroyed += InteractSphere_OnAnyDestroyed;
        InteractSphere.OnAnyUpdated += InteractSphere_OnAnyUpdated;
    }

    private void InteractSphere_OnAnyUpdated(object sender, EventArgs e) {
        PlaySound(interactSphereAudio, Camera.main.transform.position);
    }

    private void InteractSphere_OnAnyDestroyed(object sender, EventArgs e) {
        PlaySound(destroySphereAudio, Camera.main.transform.position);
    }

    private void Door_OnDoorChange(object sender, EventArgs e) {
        PlaySound(openCloseDoorSound, Camera.main.transform.position, 10f);
    }

    private void DestructableBarrel_OnAnyDamaged(object sender, EventArgs e) {
        Vector3 barrelPosition = LevelGrid.Instance.GetWorldPosition((sender as DestructableBarrel).GetGridPosition());
        PlaySound(impactWoodBreakingAudio, barrelPosition, 10f);
    }

    private void DestructableBarrel_OnAnyDestroyed(object sender, EventArgs e) {
        Vector3 barrelPosition = LevelGrid.Instance.GetWorldPosition((sender as DestructableBarrel).GetGridPosition());
        PlaySound(woodBreakingAudio, barrelPosition, 10f);
        PlaySound(healSound, barrelPosition, 10f);
    }

    private void DestructableCrate_OnAnyDestroyed(object sender, EventArgs e) {
        Vector3 cratePosition = LevelGrid.Instance.GetWorldPosition((sender as DestructableCrate).GetGridPosition());
        PlaySound(woodBreakingAudio, cratePosition, 10f);
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e) {
        switch (sender) {
            case GrenadeAction grenadeAction:
                PlaySound(grenadeThrowAudio, grenadeAction.GetProjectileSpawnPosition(), 20f);
                break;
            case SwordAction swordAction:
                if (swordAction.GetTargetUnit() != null) {
                    PlaySound(swordSliceAudio, Camera.main.transform.position, 0.2f);
                }
                break;
            default:
                break;
        }
    }

    private void MoveAction_OnAnyMovementStarted(object sender, EventArgs e) {
        unitWalkingActive = true;
    }

    private void MoveAction_OnAnyMovementStopped(object sender, EventArgs e) {
        unitWalkingActive = false;
    }

    private void Update() {
        if (!unitWalkingActive) {
            return;
        }

        walkTimer -= Time.deltaTime;
        if (walkTimer < 0) {
            walkTimer = walkTimerMax;
            AudioClip playSound = walkAudio[UnityEngine.Random.Range(0, walkAudio.Length)];
            PlaySound(playSound, Camera.main.transform.position, 10f);
        }
    }

    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs e) {
        PlaySound(shootAudio, e.shootingUnit.GetWorldPosition());
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f) {
        if (!GameHandler.Instance.IsGamePlaying()) {
            return;
        }
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }

    public void PlayJumpUpSound() {
        PlaySound(jumpUpAudio[UnityEngine.Random.Range(0, jumpUpAudio.Length)], Camera.main.transform.position, 10f);
    }

    public void PlayJumpDownSound() {
        PlaySound(jumpDownAudio[UnityEngine.Random.Range(0, jumpDownAudio.Length)], Camera.main.transform.position, 10f);
    }

    private void GrenadeProjectile_OnAnyGrenadeExploded(object sender, GrenadeProjectile.OnAnyGrenadeExplodeEventArgs e) {
        PlaySound(grenadeExplodeAudio, e.targetGrenadePosition, 10f);
    }

    public void PlaySwordSwingSound() {
        PlaySound(swordSliceAudio, Camera.main.transform.position, 5f);
    }
}
