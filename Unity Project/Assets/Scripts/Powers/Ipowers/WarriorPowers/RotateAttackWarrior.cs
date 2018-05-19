using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAttackWarrior : Ipower {

    Transform _player;
    float _radius=2;
    float _force=7;
    float _damage = 10;
    Rigidbody _rb;
  

    public void Ipower()
    {
        Collider[] col = Physics.OverlapSphere(_player.transform.position, _radius);
        foreach (var item in col)
        {
            if (item.GetComponent<EnemyClass>())
            {
                _rb = item.GetComponent<Rigidbody>();
                _rb.AddExplosionForce(_force, _player.transform.position, _radius, 2, ForceMode.Impulse);
                item.GetComponent<EnemyClass>().GetDamage(_damage);

            }
        }


    }

    public void Ipower2()
    {
        throw new System.NotImplementedException();
    }

    public RotateAttackWarrior(Transform player)
    {
        _player = player;
    }
    
}
