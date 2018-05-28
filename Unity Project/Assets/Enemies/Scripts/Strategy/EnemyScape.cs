using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScape :ESMovemnt {

    EnemyClass _enemy;
    Transform _target;
    float _speed;
    Vector3 _dirToTarget;


    public void ESMove()
    {
        Quaternion targetRotation;
        _dirToTarget = _enemy.transform.position - _target.transform.position;
        _dirToTarget += _enemy.vectAvoidance;
        _dirToTarget.y = 0;
        targetRotation = Quaternion.LookRotation(_dirToTarget, Vector3.up);
        _enemy.transform.rotation = Quaternion.Slerp(_enemy.transform.rotation, targetRotation, 7 * Time.deltaTime);
        _enemy.rb.MovePosition(_enemy.rb.position + _dirToTarget * _speed * Time.deltaTime);
    }

    public EnemyScape(EnemyClass enemy, Transform target, float speed)
    {
        _enemy = enemy;
        _target = target;
        _speed = speed;
    }
}
