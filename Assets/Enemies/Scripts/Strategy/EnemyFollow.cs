using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollow : ESMovemnt {

    GameObject _player;
    Vector3 _dirToTarget;
    Vector3 _vectSep;
    EnemyClass _enemy;
    float _speed; 

    public void ESMove()
    {
    

        if (_enemy.isAttack == false)
        {
            Quaternion targetRotation;
            _dirToTarget = (_player.transform.position - _enemy.transform.position).normalized;
            _dirToTarget.y = 0;
            var avoidance = _enemy.vectAvoidance.normalized;
            avoidance.y = 0;
            targetRotation = Quaternion.LookRotation(_dirToTarget, Vector3.up);
            _enemy.transform.rotation = Quaternion.Slerp(_enemy.transform.rotation, targetRotation, 7 * Time.deltaTime);
            _enemy.rb.MovePosition(_enemy.rb.position + (_dirToTarget + avoidance) * _speed * Time.deltaTime);    
        }
        else _enemy.currentMovement = null;
    }

    public EnemyFollow(EnemyClass enemy, GameObject player, float speed)
    {
        var model = player.GetComponent<Model>();
        model.CombatState();
        _enemy = enemy;
        _player = player;
        _speed = speed;       
    }

}
