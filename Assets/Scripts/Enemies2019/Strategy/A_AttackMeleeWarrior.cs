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

        if (!_e.onDamage)
        {
            if (!_e.onAttackArea && !_e.onAttack)
            {
                _e.viewDistanceAttack = 3.79f;
                Quaternion targetRotation;
                var dir = (_e.target.transform.position - _e.transform.position).normalized;
                dir.y = 0;
                var avoid = _e.warriorVectAvoidance.normalized;
                avoid.y = 0;
                targetRotation = Quaternion.LookRotation(dir + avoid, Vector3.up);
                _e.transform.rotation = Quaternion.Slerp(_e.transform.rotation, targetRotation, 7 * Time.deltaTime);
                _e.rb.MovePosition(_e.rb.position + _e.transform.forward * _e.speed * Time.deltaTime);
                _e.MoveEvent();
            }

            else if(_e.onAttackArea && _e.delayToAttack<0 && !_e.onRetreat)
            {
                Quaternion targetRotation;
                var dir = (_e.target.transform.position - _e.transform.position).normalized;
                dir.y = 0;
                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                _e.transform.rotation = Quaternion.Slerp(_e.transform.rotation, targetRotation, 7 * Time.deltaTime);

                _e.StartCoroutine(_e.Resting());
                if(_e.cm.times<2) _e.cm.times++;
                if (_e.flank)
                {
                    _e.flank = false;
                    _e.cm.flanTicket = false;
                }
                var player = Physics.OverlapSphere(_e.attackPivot.position, _e.radiusAttack).Where(x => x.GetComponent<Model>()).Select(x => x.GetComponent<Model>()).FirstOrDefault();

                if (player != null && !_e.firstAttack)
                {
                    _e.AttackEvent();
                    _e.StartCoroutine(_e.Delay(1.25f));
                    _e.firstAttack = true;
                }

            }      
        }
    }

    public A_AttackMeleeWarrior(ModelE_Melee e )
    {
        _e = e;
    }
}
