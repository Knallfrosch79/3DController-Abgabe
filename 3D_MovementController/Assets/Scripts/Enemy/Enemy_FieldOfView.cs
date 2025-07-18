using System;
using System.Collections;
using UnityEngine;

public class Enemy_FieldOfView : MonoBehaviour
{
    [Header("Field of View Settings")]
    [SerializeField]public float radius;
    [Range(0,360)]
    [SerializeField] float angle;
    [SerializeField] GameObject playerRef;

    [Header("Debug Settings")]
    [SerializeField] LayerMask targetMask;
    [SerializeField] LayerMask obstacleMask;

    [Header("State")]
    [SerializeField] bool canSeePlayer = false;

    private void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    /// <summary>
    /// Starts the field of view check routine.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FOVRoutine()
    {
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    /// <summary>
    /// Checks if the player is within the enemy's field of view.
    /// </summary>
    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if(Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                {
                    canSeePlayer = true;
                    Debug.Log("Player detected!");
                }
                else
                {
                    canSeePlayer = false;
                }
            }
            else
            {
                canSeePlayer = false;
            }
        }
        else if(canSeePlayer)
        {
            canSeePlayer = false;
            Debug.Log("Player lost!");
        }
    }
}
