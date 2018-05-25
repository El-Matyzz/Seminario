
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleAttack : ESMovemnt {

    Rigidbody _rb;
    float _attackMeleForce;
    EnemyClass _model;
    GameObject _player;
    Vector3 _dirToTarget;

    public void ESMove()
    {
        _model.transform.LookAt(_player.transform.position);
        if (_model.myTimeToAttack == true) _model.dileyToAttack -= Time.deltaTime;
        if (_model.dileyToAttack <= 0)
        {
            _dirToTarget = (_player.transform.position - _model.transform.position).normalized;
            _dirToTarget.y = 0;
            _model.transform.forward = _dirToTarget;
            _rb.AddForce(_model.transform.forward * _attackMeleForce, ForceMode.Impulse);
            _model.dileyToAttack = Random.Range(4f, 5f);
            _model.myTimeToAttack = false;
            _model.createAttack= true;
            foreach (var item in _model.myFriends)
            {
                if (item.myTimeToAttack == false)
                {
                    item.myTimeToAttack = true;
                    break;
                }
            }
            _model.currentMovement = null;
        }
    }

    public EnemyMeleAttack(Rigidbody rb , float attackforce, EnemyClass model, GameObject player)
    {
        _rb = rb;
        _attackMeleForce = attackforce;
        _model = model;
        _player = player;
    }
}
