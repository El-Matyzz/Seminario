using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollow : ESMovemnt {

    GameObject _player;
    Vector3 _dirToTarget;
    ModelEnemy _enemy;
    float _speed;

    public void ESMove()
    {

        if (_enemy.isAttack == false)
        {
            _dirToTarget = (_player.transform.position - _enemy.transform.position).normalized;
            _dirToTarget.y = 0;
            _enemy.transform.forward = _dirToTarget;
            _enemy.transform.position += _enemy.transform.forward * _speed * Time.deltaTime;
        }
        else _enemy.currentMovement = null;
    }

    public EnemyFollow(ModelEnemy enemy, GameObject player, float speed)
    {
        _enemy = enemy;
        _player = player;
        _speed = speed;
        
    }
}
