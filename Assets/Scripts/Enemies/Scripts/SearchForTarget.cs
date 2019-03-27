using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchForTarget : MonoBehaviour {

  

    public static bool SearchTarget(GameObject target, float viewDistance, float viewAngle, GameObject follower, bool detector)
    {
        var _viewAngle = viewAngle;
        var _viewDistance = viewDistance;
        var _enemy = follower;

        if (target == null) return false;

        var _dirToTarget = (target.transform.position - _enemy.transform.position).normalized;

        var _angleToTarget = Vector3.Angle(_enemy.transform.forward, _dirToTarget);

        var _distanceToTarget = Vector3.Distance(_enemy.transform.position, target.transform.position);

        if (_angleToTarget <= _viewAngle && _distanceToTarget <= _viewDistance)
        {
            RaycastHit rch;
            bool obstaclesBetween = false;

            if (detector)
            {
                if (Physics.Raycast(_enemy.transform.position, _dirToTarget, out rch, _distanceToTarget))
                    if (rch.collider.gameObject.layer == 8)
                        obstaclesBetween = true;
            }
            if (!obstaclesBetween)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}

