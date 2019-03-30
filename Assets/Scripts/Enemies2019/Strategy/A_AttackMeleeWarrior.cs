using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class A_AttackMeleeWarrior : i_EnemyActions
{
    ModelE_Melee _e;

    public void Actions()
    {
        _e.target.CombatState();

        Quaternion targetRotation;
        var _dir = (_e.target.transform.position - _e.transform.position).normalized;
        _dir.y = 0;
        targetRotation = Quaternion.LookRotation(_dir, Vector3.up);
        _e.transform.rotation = Quaternion.Slerp(_e.transform.rotation, targetRotation, 7 * Time.deltaTime);

        if (!_e.timeToAttack && _e.cm.times > 0)
        {
            _e.cm.times--;
            _e.timeToAttack = true;
        }

        if (_e.timeToAttack) _e.dileyToAttack -= Time.deltaTime;
        if (_e.dileyToAttack <= 0 && !_e.onDamage)
        {
            _e.rb.AddForce(_e.transform.forward * _e.knockbackForce, ForceMode.Impulse);
            _e.StartCoroutine(_e.Resting());
            _e.cm.times++;
            _e.dileyToAttack = UnityEngine.Random.Range(3f, 5f);         
        }

        if (_e.onAttack)
        {
            var player = Physics.OverlapSphere(_e.attackPivot.position, _e.radiusAttack).Where(x => x.GetComponent<Model>()).Select(x => x.GetComponent<Model>()).FirstOrDefault();
            if (player != null)
            {
                player.GetDamage(_e.attackDamage, _e.transform, false);
                _e.rb.velocity = Vector3.zero;
                _e.rb.AddForce(-_e.transform.forward * _e.knockbackForce/2  , ForceMode.Impulse);
                _e.onAttack = false;
            }
        }
    }

    public A_AttackMeleeWarrior(ModelE_Melee e)
    {
        _e = e;
    }
}
