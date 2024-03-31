using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    public override void Fire()
    {
        Debug.Log(gameObject.name + " Fired");
    }
    public override void Reload()
    {
        Debug.Log("Reload");
    }
}
