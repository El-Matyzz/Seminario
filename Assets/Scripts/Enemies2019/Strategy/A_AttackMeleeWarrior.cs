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
                Debug.Log(1);
                Quaternion targetRotation;
                var dir = (_e.target.transform.position - _e.transform.position).normalized;
                dir.y = 0;
                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                _e.transform.rotation = Quaternion.Slerp(_e.transform.rotation, targetRotation, 7 * Time.deltaTime);
                _e.rb.MovePosition(_e.rb.position + _e.transform.forward * _e.speed * Time.deltaTime);
            }

            else if(_e.onAttackArea && _e.dileyToAttack<0)
            {

                Quaternion targetRotation;
                var dir = (_e.target.transform.position - _e.transform.position).normalized;
                dir.y = 0;
                targetRotation = Quaternion.LookRotation(dir, Vector3.up);
                _e.transform.rotation = Quaternion.Slerp(_e.transform.rotation, targetRotation, 7 * Time.deltaTime);

                _e.StartCoroutine(_e.Resting());
                _e.cm.times++;
                if (_e.flank)
                {
                    _e.flank = false;
                    _e.cm.flanTicket = false;
                }
                _e.dileyToAttack = Random.Range(4f, 6f);
                _e.maxDileyToAttack = _e.dileyToAttack;

                var player = Physics.OverlapSphere(_e.attackPivot.position, _e.radiusAttack).Where(x => x.GetComponent<Model>()).Select(x => x.GetComponent<Model>()).FirstOrDefault();
                if (player != null)
                {
                    _e.AttackEvent();                 
                }
            }
        }
    }

    public A_AttackMeleeWarrior(ModelE_Melee e )
    {
        _e = e;
    }
}
