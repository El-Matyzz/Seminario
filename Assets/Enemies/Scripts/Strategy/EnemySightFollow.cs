using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySightFollow : ESMovemnt {

    EnemyClass _enemy;
    Transform _target;
    float _sightSpeed;
    Vector3 _dirToTarget;

    public void ESMove()
    {

      Quaternion targetRotation;
      _dirToTarget = (_target.transform.position - _enemy.transform.position).normalized;
      _dirToTarget.y = 0;
      targetRotation = Quaternion.LookRotation(_dirToTarget, Vector3.up);
      _enemy.transform.rotation = Quaternion.Slerp(_enemy.transform.rotation, targetRotation, _sightSpeed * Time.deltaTime);

     }

    public EnemySightFollow(EnemyClass enemy, Transform target, float sightSpeed)
    {
        var model = target.GetComponent<Model>();
        model.timeOnCombat = 5;
        _enemy = enemy;
        _target = target;
        _sightSpeed = sightSpeed;
    }
}
