using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CreateAssetMenu(menuName = "Project VRS/New Projectile Data Recursive")]
[System.Serializable]
public class SO_ProjectileRecursive : ScriptableObject
{
    [System.Serializable]
    class ProjectileData
    {
        [SerializeField] float speed;
        [SerializeField] uint damage;
        [SerializeField] uint armorPenetration;
        [SerializeField] float lifetime;
        [SerializeField] Vector2 spread;
        [SerializeField] GameObject prefab;
        [SerializeField] bool spawnSubProjectiles = false;
        [SerializeField] uint subProjectileCount;
        [SerializeReference] public ProjectileData subProjectileData = null;
    }
    [SerializeField]ProjectileData projectileData;
}
public class SO_ProjectileRecursiveEditor : Editor
{

}