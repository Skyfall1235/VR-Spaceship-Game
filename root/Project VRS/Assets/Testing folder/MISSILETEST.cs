
using UnityEngine;


public class MISSILETEST : MonoBehaviour
{
    public Transform target;
    public float speed = 10f;
    public Vector3 OldRTM;

    void FixedUpdate()
    {
        //Rigidbody rb = GetComponent<Rigidbody>();
        //Rigidbody targetRB = target.GetComponent<Rigidbody>();
        //commande accelleration = N * Vc * LOSrate + (N * Nt) / 2
        Vector3 NewRTM = target.position - transform.position;
        NewRTM.Normalize();
        OldRTM.Normalize();
        Vector3 LOSDelta = NewRTM - OldRTM;
        float LOSRate = LOSDelta.magnitude;

        float N = 3;
        float Vc = -LOSRate;

        float Nt = 0;
        if(Nt == 0)
        {
            Nt = 9.8f;
        }
        //A_cmd = N * Vc * LOS_Rate + N * Nt / 2
        Vector3 latax = LOSRate * N * Vc * NewRTM + LOSDelta * Nt * (0.5f * N);


        //current position of the target
        //the missile                                           needed for newRTM
        //the navigation gain                                   N
        //the old RTM                                           oldRTM 
        //the acceleration (if 0 set to gravity / at least 1)   Nt
        //the negative LOSRate                                  Vc
        //the LOS delta                                         newrtm - oldrtm
        //LOS rate                                              delta magnitude


        OldRTM = NewRTM;
    }
}

