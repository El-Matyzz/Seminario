
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleAttack : ESMovemnt {

    Rigidbody _rb;
    float _attackMeleForce;
    EnemyClass _model;
    GameObject _player;
    Vector3 _dirToTarget;
    ModelEnemy _modelEnemy;

    public void ESMove()
    {

        _dirToTarget = (_player.transform.position - _model.transform.position).normalized;
        _dirToTarget.y = 0;
        _model.transform.forward = _dirToTarget;

        if (_model.myTimeToAttack == true) _model.dileyToAttack -= Time.deltaTime;
        if (_model.dileyToAttack < 1) _modelEnemy.view.ActiveLightAtack();
        if (_model.dileyToAttack <= 0 && _model.isAttack)
        {
            _modelEnemy.view.DesactivateLightAttack();
            _rb.AddForce(_model.transform.forward * _attackMeleForce, ForceMode.Impulse);
            _model.dileyToAttack = Random.Range(4f, 5f);
            _model.myTimeToAttack = false;
            _model.createAttack= true;
            foreach (var item in _model.myFriends)
            {
                if (item.myTimeToAttack == false && item.GetComponent<ModelEnemy>())
                {
                    item.myTimeToAttack = true;
                    break;
                }
            }
            _model.currentMovement = null;
            _model.createAttack = true;
            _modelEnemy.StartCoroutine(_modelEnemy.AttackCorrutine());
        }
        if(!_model.isAttack) _model.currentMovement = null;
    }

    public EnemyMeleAttack(Rigidbody rb , float attackforce, EnemyClass model, GameObject player)
    {
        var modelPlayer = player.GetComponent<Model>();
        modelPlayer.CombatState();
        _rb = rb;
        _attackMeleForce = attackforce;
        _model = model;
        _player = player;
        _modelEnemy = _model.GetComponent<ModelEnemy>();
    }
}
