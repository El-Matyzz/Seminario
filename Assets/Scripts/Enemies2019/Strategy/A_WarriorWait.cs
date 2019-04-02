using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class A_WarriorWait : i_EnemyActions
{
    ModelE_Melee _e;
    int flankSpeed;

    public void Actions()
    {
        _e.target.CombatState();


        if (!_e.timeToAttack && _e.cm.times > 0)
        {
            flankSpeed = Random.Range(0, 2);
            _e.cm.times--;
            _e.timeToAttack = true;
            if (!_e.cm.flanTicket)
            {
                _e.flank = true;
                _e.cm.flanTicket = true;
            }
        }

        if (!_e.onAttack && !_e.flank)
        {
            if (!_e.onDamage) _e.IdleEvent();

            Quaternion targetRotation;
            var _dir = (_e.target.transform.position - _e.transform.position).normalized;
            _dir.y = 0;
            targetRotation = Quaternion.LookRotation(_dir, Vector3.up);
            _e.transform.rotation = Quaternion.Slerp(_e.transform.rotation, targetRotation, 7 * Time.deltaTime);
            var _avoid = _e.warriorVectAvoidance.normalized;
            _avoid.y = 0;
            _e.transform.position += _avoid * _e.speed * Time.deltaTime;

        }

        if (!_e.onAttack && _e.flank)
        {
            if (!_e.onDamage) _e.MoveEvent();

            var rotateSpeed = 0;

            if (flankSpeed < 1) rotateSpeed = 30;
            else rotateSpeed = -30;

            var dir = (_e.target.transform.position - _e.transform.position).normalized;
            var angle = Vector3.Angle(dir, _e.transform.forward);
            if (angle < 100)
            {
                var d = Vector3.Distance(_e.transform.position, _e.target.transform.position);

                _e.transform.RotateAround(_e.target.transform.position, Vector3.up, rotateSpeed * Time.deltaTime);

                if (_e.avoidVectObstacles != Vector3.zero && d > 3) _e.transform.position += _e.transform.forward * 4 * Time.deltaTime;

                _e.transform.forward = dir;

            }

        }

        if (_e.timeToAttack) _e.dileyToAttack -= Time.deltaTime;
     
    }

    public A_WarriorWait(ModelE_Melee e)
    {
        _e = e;
    }
}
