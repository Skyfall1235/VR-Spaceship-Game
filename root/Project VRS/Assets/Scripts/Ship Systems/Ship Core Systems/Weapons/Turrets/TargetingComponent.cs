using System;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using System.Collections;

public class TargetingComponent : MonoBehaviour
{
    //so, what external things ddoes this script need?
    //1 - the projectile info. this can be gotten from the scriptable object on the turret
    //2 - the barrels instatiation point. the calculations need the exact spawning position to be effective
    //3 - the target data from the target handler.

    [Header("Target Data")]

    [SerializeField]
    [Tooltip("Stores data about the currently targeted enemy.")]
    private TargetData m_currentTargetData;
    public TargetData CurrentTargetData
    {
        set
        {
            m_currentTargetData = value;
        }
    }

    [SerializeField]
    [Tooltip("The calculated lead position for targeting enemies in motion. Defaults to Vector3.zero.")]
    private Vector3 m_leadPosition = Vector3.zero;
    public Vector3 LeadPositionToTarget
    {
        get
        {
            return m_leadPosition;
        }
    } 

    /// <summary>
    /// 
    /// </summary>
    private SO_WeaponData m_weaponData;
    public SO_WeaponData WeaponData
    {
        set
        {
            m_weaponData = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private GameObject m_projectileInstantiationPoint;
    public GameObject ProjectileInstantiationPoint
    {
        set
        {
            m_projectileInstantiationPoint = value;
        }
    }


    #region Monobehavior Methods

    private void FixedUpdate()
    {
        if (m_currentTargetData.IsEmpty)
        {
            return;
        }
        //we dont want to schedule jobs on top of each other
        if (!CoroutineIsFinished)
        {
            return;
        }
        //NEEDS an if statement to only run if there is currently a target that is aquired
        if(m_currentTargetData.IsEmpty || m_weaponData == null)
        {
            //data is missing and cannot be ran, end now
            return; 
        }

        //Start job if there are no obstructions
        StartCoroutine(ComputeTargetLead(m_projectileInstantiationPoint.transform.position, m_currentTargetData, m_weaponData.UseProjectile? m_weaponData.ProjectileData.ProjectileSpeed : Mathf.Infinity));
    }

    private void OnDestroy()
    {
        KillTracking();
    }

    private void OnApplicationQuit()
    {
        KillTracking();
    }

    private void OnDrawGizmosSelected()
    {
        //draw a ray in direction of the guidnce command from the missile
        DrawLine(transform.position, m_leadPosition, Color.green);
        //DrawLine(transform.position, barrel.transform.up * 5, Color.red);

        void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            Debug.DrawLine(start, end, color);
        }
    }

    #endregion

    #region Target lead Calculation

    private Coroutine TargetLeadCoroutine;
    private JobHandle ComputeTargetLeadJob;
    private NativeArray<float3> targetLeadDataStorage;
    private bool CoroutineIsFinished = true;

    /// <summary>
    /// This coroutine calculates the lead position necessary to hit a moving target with a projectile,
    /// utilizing a multithreaded job for efficient calculation.
    /// </summary>
    /// <param name="gunPosition">The position of the gun firing the projectile.</param>
    /// <param name="targetData">An object containing information about the target, including its GameObject and Rigidbody.</param>
    /// <param name="projectileSpeed">The speed of the projectile.</param>
    /// <returns>An IEnumerator that can be used to yield and wait for job completion.</returns>
    protected IEnumerator ComputeTargetLead(Vector3 gunPosition, TargetData targetData, float projectileSpeed)
    {
        CoroutineIsFinished = false;
        try
        {
            //schedule the job
            Vector3 targetPosition = targetData.TargetGameObject.transform.position;
            Vector3 targetVelocity = targetData.TargetRB.velocity;

            ComputeTargetLeadJob = FindTargetLead(gunPosition, targetPosition, targetVelocity, projectileSpeed, targetLeadDataStorage);
            yield return ComputeTargetLeadJob;
        }
        finally
        {
            //dispose and force completion
            ComputeTargetLeadJob.Complete();
        }
        //save and dispose of the ram
        m_leadPosition = CalculateTargetLeadJob.Float3ToVector3(targetLeadDataStorage[0]);
        CoroutineIsFinished = true;
    }

    /// <summary>
    /// Instantly stops the coroutine and disposes of the memory and job in a safe fashion
    /// </summary>
    protected void KillTracking()
    {
        if (TargetLeadCoroutine != null)
        {
            StopCoroutine(TargetLeadCoroutine);
        }
        //force a completion of the last step, then kill the native array AFTER. (i think this avoids a memory leak?)
        ComputeTargetLeadJob.Complete();
        targetLeadDataStorage.Dispose();
    }

    /// <summary>
    /// This method schedules a job to calculate the lead needed to hit a moving target with a projectile.
    /// </summary>
    /// <param name="playerPosition">The position of the player firing the projectile.</param>
    /// <param name="targetPosition">The current position of the target.</param>
    /// <param name="targetAcceleration">The acceleration of the target (optional, defaults to Vector3.zero).</param>
    /// <param name="projectileSpeed">The speed of the projectile.</param>
    /// <param name="nativeArrayRef">A reference to a pre-allocated NativeArray<float3> to store the calculated lead position (must have a length of 1).</param>
    /// <returns>A JobHandle that can be used to check the job's completion status.</returns>
    protected JobHandle FindTargetLead(Vector3 playerPosition, Vector3 targetPosition, float3 targetAcceleration, float projectileSpeed, NativeArray<float3> nativeArrayRef)
    {
        CalculateTargetLeadJob job = new CalculateTargetLeadJob(playerPosition, targetPosition, targetAcceleration, projectileSpeed, nativeArrayRef);
        return job.Schedule();
    }

    /// <summary>
    /// Unity Job that calculates the target lead lneeded for a gun to hit its mark in 3d space.
    /// </summary>
    /// <remarks>
    /// The native array inputted into the constructor is how you get an output. you will need to schedule this job with a coroutine
    /// </remarks>
    [BurstCompile]
    protected struct CalculateTargetLeadJob : IJob
    {
        readonly float3 PlayerPosition;
        readonly float3 TargetPosition;
        readonly float3 TargetAcceleration;
        readonly float ProjectileSpeed;
        //[0] should be the output
        [WriteOnly] NativeArray<float3> NativeArray;

        public CalculateTargetLeadJob(Vector3 playerPosition, Vector3 targetPosition, float3 targetAcceleration, float projectileSpeed, NativeArray<float3> nativeArrayRef)
        {
            //convert the vector3s to  floats
            this.PlayerPosition = Vector3ToFloat3(playerPosition);
            this.TargetPosition = Vector3ToFloat3(targetPosition);
            this.TargetAcceleration = Vector3ToFloat3(targetAcceleration);
            this.ProjectileSpeed = projectileSpeed;
            this.NativeArray = nativeArrayRef;
        }

        public void Execute()
        {
            //calclulate and save
            float3 LeadPositionRelativeToPosition = CalulcateLeadPosition(PlayerPosition, TargetPosition, TargetAcceleration, ProjectileSpeed);
            NativeArray[0] = LeadPositionRelativeToPosition;
        }

        float3 CalulcateLeadPosition(float3 playerPosition, float3 targetPosition, float3 targetAcceleration, float projectileSpeed)
        {
            // Calculate lead position
            float distance = Distance(playerPosition, targetPosition);
            float travelTime = distance / projectileSpeed;
            float3 leadPosition = targetPosition + targetAcceleration * (travelTime * travelTime) / 2; // Consider constant acceleration
            return leadPosition;
        }

        #region basic math funcs

        float Distance(float3 a, float3 b)
        {
            float num = a.x - b.x;
            float num2 = a.y - b.y;
            float num3 = a.z - b.z;
            return (float)Math.Sqrt(num * num + num2 * num2 + num3 * num3);
        }

        public static float3 Vector3ToFloat3(Vector3 from)
        {
            return new float3(from.x, from.y, from.z);
        }

        public static Vector3 Float3ToVector3(float3 from)
        {
            return new Vector3(from.x, from.y, from.z);
        }

        #endregion
    }

    #endregion

}
