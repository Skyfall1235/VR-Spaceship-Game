using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingComputations : MonoBehaviour
{
    //target leading algorithm
    // 
    //angle ABC, where B is thelink for both.
    //angle ABC is the amount of rotation the turret should attemt tp rotate to to hit its shot

    //
    //tringle in 3d
    //the speed of the bullet
    //the speed of the target
    //
    //so, to do this, we need to get the adjusted velocity of the target relative to the gun. this can be dont be subtracting the velocity from eac other\

    //ideally, this is where a unity job would go to give the angle for thr next frame projection.


    //basically, we need a way for the turret to reach out to the targeter, get back a unity job, wiat for the job to finish , and then use the result.
    //since only the targeting system knows the locationdf of the objects, the weapons will need to interact with it in a dynamic by a non modifying method
    
}
