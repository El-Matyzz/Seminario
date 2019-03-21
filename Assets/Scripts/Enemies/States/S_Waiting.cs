using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Waiting : EnemyState
{
    GameObject _player;
    ModelEnemy _model;
    Vector3 _dirToTarget;

    public S_Waiting(StateMachine sm, EnemyClass e, ModelEnemy model, GameObject player) : base(sm, e)
    {
        var modelPlayer = player.GetComponent<Model>();
       // modelPlayer.CombatState();
        _model = model;
        _player = player;
    }

    public override void Awake()
    {

        base.Awake();
    }

    public override void Execute()
    {
        base.Execute();

        if (_model.avoidVect != Vector3.zero)
        {
            _model.transform.position += _model.avoidVect * _model.speed * Time.deltaTime;
        }
        _dirToTarget = (_player.transform.position - _model.transform.position).normalized;
        _dirToTarget.y = 0;
        _model.transform.forward = _dirToTarget;
    }

    public override void Sleep()
    {

        base.Sleep();
    }

}
