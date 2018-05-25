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
       // _vectSep = getSep() * _enemy.separationWeight;

        if (_enemy.isAttack == false)
        {
            Quaternion targetRotation;
            _dirToTarget = (_player.transform.position - _enemy.transform.position).normalized;
            _dirToTarget += _enemy.vectAvoidance;
            _dirToTarget.y = 0;
            targetRotation = Quaternion.LookRotation(_dirToTarget, Vector3.up);
            _enemy.transform.rotation = Quaternion.Slerp(_enemy.transform.rotation, targetRotation, 7 * Time.deltaTime);
            _enemy.rb.MovePosition(_enemy.rb.position + _dirToTarget * _speed * Time.deltaTime);
     
        }
        else _enemy.currentMovement = null;
    }

    public EnemyFollow(EnemyClass enemy, GameObject player, float speed)
    {
        _enemy = enemy;
        _player = player;
        _speed = speed;       
    }

  /*  Vector3 getSep()
    {
        Vector3 sep = new Vector3();
        foreach (var item in _enemy.myFriends)
        {
            Vector3 f = new Vector3();
            f = _enemy.transform.position - item.transform.position;
            float mag = _enemy.radFlock - f.magnitude;
            f.Normalize();
            f *= mag;
            sep += f;
        }
        sep.y = 0;
        return sep /= _enemy.myFriends.Count;
    }
    */
}
