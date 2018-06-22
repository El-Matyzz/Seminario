using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : ESMovemnt
{
    GameObject _player;
    Vector3 _dirToTarget;
    EnemyClass _enemy;
    float _speed;

    public void ESMove()
    {
        Quaternion targetRotation;
        if (_enemy.closeObstacle || _enemy.distanceToPatrol>=_enemy.areaToPatrol)
        {
            if (_enemy.distanceToPatrol >= _enemy.areaToPatrol) _dirToTarget = -_enemy.transform.forward;
            else
            {
                _dirToTarget = _enemy.vectTurnDirecction;
                _dirToTarget.y = 0;
            }
        }
        targetRotation = Quaternion.LookRotation(_dirToTarget, Vector3.up);
        _enemy.transform.rotation = Quaternion.Slerp(_enemy.transform.rotation, targetRotation, 7 * Time.deltaTime);
        _enemy.rb.MovePosition(_enemy.rb.position + _dirToTarget * (_speed / 2) * Time.deltaTime);
    }

    public EnemyPatrol(EnemyClass enemy, GameObject player, float speed, Vector3 dir)
    {
        _enemy = enemy;
        _player = player;
        _speed = speed;
        _dirToTarget = dir;
    }
}
