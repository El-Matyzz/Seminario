
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleAttack : ESMovemnt {

    Rigidbody _rb;
    float _attackMeleForce;
    ModelEnemy _model;
    GameObject _player;
    Vector3 _dirToTarget;

    public void ESMove()
    {

        if (_model.myTimeToAttack == true) _model.dileyToAttack -= Time.deltaTime;
        if (_model.dileyToAttack <= 0)
        {
            _dirToTarget = (_player.transform.position - _model.transform.position).normalized;
            _dirToTarget.y = 0;
            _model.transform.forward = _dirToTarget;
            _rb.AddForce(_model.transform.forward * _attackMeleForce, ForceMode.Impulse);
            _model.dileyToAttack = Random.Range(1.5f, 2.5f);
            _model.myTimeToAttack = false;
        }
    }

    public EnemyMeleAttack(Rigidbody rb , float attackforce, ModelEnemy model, GameObject player)
    {
        _rb = rb;
        _attackMeleForce = attackforce;
        _model = model;
        _player = player;
    }
}
