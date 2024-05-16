using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStatics : MonoBehaviour
{
    private void Awake() {
        BaseAction.ResetStaticData();
        GrenadeProjectile.ResetStaticData();
        SwordAction.ResetStaticData();
        ShootAction.ResetStaticData();
        MoveAction.ResetStaticData();
        GrenadeAction.ResetStaticData();
        InteractAction.ResetStaticData();
        SpinAction.ResetStaticData();
        EnemyAI.ResetStaticData();
        InteractSphere.ResetStaticData();
        Unit.ResetStaticData();
        DestructableBarrel.ResetStaticData();
        DestructableCrate.ResetStaticData();
        Door.ResetStaticData();
    }
}
