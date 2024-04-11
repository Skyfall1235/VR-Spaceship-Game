using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class SO_TurretData : ScriptableObject
{
    //what else needs to go here, and what can we store in the SO?

    [SerializeField] 
    public float projectileSpeed; // this can probably just be puleld from the actual bllet script when we decide to make that.
    [SerializeField]
    public GameObject prefabBullet;
    [SerializeField]
    public Vector2 turretRotationSpeed = new Vector2(20, 20);
    [SerializeField]
    public bool useGimbalConstraints = false;
    [SerializeField]
    public Vector2 maximumTurretAngles = new Vector2(15, 15);

    [SerializeField]
    TurretTargetingType targetingType;
    [SerializeField]
    TurretWeaponType weaponType;

    public enum TurretTargetingType
    { 
        ManualLockOnGimbal,
        Automatic,
    }

    public enum TurretWeaponType
    {
        Ballistic,
        Laser,
        Missile//yes? no ? idk
    }

}
