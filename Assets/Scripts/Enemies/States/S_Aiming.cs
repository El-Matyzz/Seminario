﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Aiming : EnemyState
{
    EnemyClass _enemy;
    Transform _target;
    float _sightSpeed;
    Vector3 _dirToTarget;

    public S_Aiming(StateMachine sm, EnemyClass e, Transform target, float sightSpeed) : base(sm, e)
    {
        var model = target.GetComponent<Model>();
        model.CombatState();
        _enemy = e;
        _target = target;
        _sightSpeed = sightSpeed;
    }

    public override void Awake()
    {

        base.Awake();
    }

    public override void Execute()
    {
        base.Execute();

        Quaternion targetRotation;
        _dirToTarget = (_target.transform.position - _enemy.transform.position).normalized;
        _dirToTarget.y = 0;
        targetRotation = Quaternion.LookRotation(_dirToTarget, Vector3.up);
        _enemy.transform.rotation = Quaternion.Slerp(_enemy.transform.rotation, targetRotation, _sightSpeed * Time.deltaTime);

    }

    public override void Sleep()
    {

        base.Sleep();

    }
}
