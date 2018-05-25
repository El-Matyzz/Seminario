using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySearching : ESMovemnt {

    float _speed;
    Transform _target;
    EnemyClass _enemy;
    Vector3 _dirToTarget;
    Vector3 _randomVect;

    public void ESMove()
    {
        Quaternion targetRotation;
        _dirToTarget = (_target.position - _enemy.transform.position).normalized;
        _dirToTarget += _randomVect + _enemy.vectAvoidance;
        _dirToTarget.y = 0;
        targetRotation = Quaternion.LookRotation(_dirToTarget, Vector3.up);
        _enemy.transform.rotation = Quaternion.Slerp(_enemy.transform.rotation, targetRotation, 7 * Time.deltaTime);
        _enemy.rb.MovePosition(_enemy.rb.position + _dirToTarget * _speed * Time.deltaTime);
    }

    public EnemySearching(EnemyClass enemy, Transform target, float speed)
    {
        _enemy = enemy;
        _target = target;
        _speed = speed;
        _randomVect = new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2));
    }
  
}
