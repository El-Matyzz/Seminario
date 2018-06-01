﻿using System;
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
            _dirToTarget = ((_player.transform.position - _enemy.transform.position) + _enemy.vectAvoidance).normalized;
            _dirToTarget.y = 0;
            targetRotation = Quaternion.LookRotation(_dirToTarget, Vector3.up);
            _enemy.transform.rotation = Quaternion.Slerp(_enemy.transform.rotation, targetRotation, 7 * Time.deltaTime);
            _enemy.rb.MovePosition(_enemy.rb.position + _dirToTarget * _speed * Time.deltaTime);    
        }
        else _enemy.currentMovement = null;
    }

    public EnemyFollow(EnemyClass enemy, GameObject player, float speed)
    {
        var model = player.GetComponent<Model>();
        model.StopCoroutine(model.StartStateCombat());
        model.StartCoroutine(model.StartStateCombat());
        _enemy = enemy;
        _player = player;
        _speed = speed;       
    }

}
