using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public virtual void Awake()
    {

    }
    public virtual void Fire()
    {
        throw new System.NotImplementedException();
    }
    public virtual void Reload()
    {
        throw new System.NotImplementedException();
    }
}
