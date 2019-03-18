﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Persuit : EnemyState
{
    EnemyClass _enemy;
    float _speed;
    Vector3 _dir;

    public S_Persuit(StateMachine sm, EnemyClass e, Model player, float speed) : base(sm, e)
    {
        player.CombatState();
        _enemy = e;
        _speed = speed;
    }

    public override void Awake()
    {

        base.Awake();
    }

    public override void Execute()
    {
        base.Execute();

        if (_enemy.target != null)
        {

            if (_enemy.pathToTarget == null || _enemy.currentIndex == _enemy.pathToTarget.Count)
            {
                return;

            }
            float d = Vector3.Distance(_enemy.pathToTarget[_enemy.currentIndex].transform.position, _enemy.transform.position);
            if (d >= 1)
            {
                Quaternion targetRotation;
                _dir = (_enemy.pathToTarget[_enemy.currentIndex].transform.position - _enemy.transform.position).normalized;
                _dir.y = 0;
                var forwardDir = (_enemy.target.transform.position - _enemy.transform.position).normalized;
                forwardDir.y = 0;
                targetRotation = Quaternion.LookRotation(forwardDir, Vector3.up);
                _enemy.transform.rotation = Quaternion.Slerp(_enemy.transform.rotation, targetRotation, 7 * Time.deltaTime);
                _enemy.rb.MovePosition(_enemy.rb.position + _dir * _speed * Time.deltaTime);

            }
            else
                _enemy.currentIndex++;

        }
    }

    public override void Sleep()
    {

        base.Sleep();

        //buscar
    }

}
