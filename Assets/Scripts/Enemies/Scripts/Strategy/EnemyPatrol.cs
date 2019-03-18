using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : ESMovemnt
{
    GameObject _player;
    Vector3 _dirToTarget;
    EnemyClass _enemy;
    float _speed;

    public void ESMove()
    {
    
    }

    public EnemyPatrol(EnemyClass enemy, GameObject player, float speed, Vector3 dir)
    {
        _enemy = enemy;
        _player = player;
        _speed = speed;
        _dirToTarget = dir;
    }
}
