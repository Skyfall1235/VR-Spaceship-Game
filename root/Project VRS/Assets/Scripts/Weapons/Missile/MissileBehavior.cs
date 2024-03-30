using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MissileBehavior : MonoBehaviour
{
    [SerializeField] SO_MissileData behaviorParameters;
    //the target, TARGET TYPE

    //GAUDANCE STUFF :  some constat? elavated trajectory?
    //limits?
    //INTERCEPT PROPERTIES
    //like the  hit range, bool for stop guidance after intercept, destroy on hit


    //AN INTERCEPT EVENT
    //an event that gets called with each stage getting used up so we can do cool stuff with it down the road?
    //also a passed target event
}
