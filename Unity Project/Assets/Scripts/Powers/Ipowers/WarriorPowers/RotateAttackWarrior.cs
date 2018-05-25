﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAttackWarrior : Ipower {

    Transform _player;
    Model _model;
    float _radius=4;
    float _force=10;
    float _damage = 10;
    float _cdTime = 5;
    Rigidbody _rb;
  

    public void Ipower()
    {
        Collider[] col = Physics.OverlapSphere(_player.transform.position, _radius);
        foreach (var item in col)
        {
            if (item.GetComponent<EnemyClass>())
            {
                var enemy = item.GetComponent<ModelEnemy>();

                _rb = item.GetComponent<Rigidbody>();
                _rb.AddForce(-item.transform.forward * _force, ForceMode.Impulse);

                enemy.StartCoroutine(enemy.Stuned(1));

                enemy.GetDamage(_damage, item.transform);

            }
        }
        if (_model.mySkills.secondRotate)
        {
            _model.StartCoroutine(_model.ActionDelay(Ipower2));
            _model.StartCoroutine(_model.InActionDelay(2f));
        }
        else _model.StartCoroutine(_model.InActionDelay(0.6f));
        _model.StartCoroutine(_model.PowerColdown(_cdTime,2));

    }

    public void Ipower2()
    {
        Collider[] col = Physics.OverlapSphere(_player.transform.position, _radius);
        foreach (var item in col) {
            if (item.GetComponent<EnemyClass>()) {
                _rb = item.GetComponent<Rigidbody>();
                _rb.AddForce(-item.transform.forward * _force*1.5f, ForceMode.Impulse);
                item.GetComponent<EnemyClass>().GetDamage(_damage, item.transform);
                if (_model.mySkills.healRotateAttack)
                {
                    _model.life += (_damage * 30) / 100;
                    if (_model.life >= _model.totalLife) _model.life = _model.totalLife;
                }
            }
        }
    }

    public RotateAttackWarrior(Transform player)
    {
        _player = player;
        _model = _player.gameObject.GetComponent<Model>();
    }
    
}
