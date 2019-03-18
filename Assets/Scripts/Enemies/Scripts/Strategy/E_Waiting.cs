using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_Waiting : ESMovemnt
{
    GameObject _player;
    ModelEnemy _model;
    Vector3 _dirToTarget;

    public void ESMove()
    {
        if (_model.avoidVect != Vector3.zero)
        {
            _model.transform.position += _model.avoidVect * _model.speed * Time.deltaTime;
        }
        _dirToTarget = (_player.transform.position - _model.transform.position).normalized;
        _dirToTarget.y = 0;
        _model.transform.forward = _dirToTarget;
    }

    public E_Waiting(ModelEnemy model, GameObject player)
    {
        var modelPlayer = player.GetComponent<Model>();
        modelPlayer.CombatState();
        _model = model;
        _player = player;       
    }
}
