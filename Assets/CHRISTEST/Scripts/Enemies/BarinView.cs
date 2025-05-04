using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarinView : MonoBehaviour
{


   /* public virtual IEnumerator FOVRoutime()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        // Debug.Log("busco player");

        while (true)
        {
            yield return wait;
            canSeePlayer = FieldOfViewCheck();
        }
    }
    public virtual bool FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, checkRadius, whatIsPlayer);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    // Debug.Log("veo player");
                    return true;
                }
                else
                {
                    // Debug.Log(" no veo player");

                    return false;
                }
            }
            else
            {
                //Debug.Log(" no veo player");
                return false;
            }
        }
        else
        {
            //Debug.Log(" no veo player");
            return false;
        }
    }*/
}
