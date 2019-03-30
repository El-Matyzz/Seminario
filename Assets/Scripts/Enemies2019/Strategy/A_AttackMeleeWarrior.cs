using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class A_AttackMeleeWarrior : i_EnemyActions
{
    ModelE_Melee _e;
    int flankSpeed;

    public void Actions()
    {
        _e.target.CombatState();
        

        if (!_e.timeToAttack && _e.cm.times > 0)
        {
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
            _e.IdleEvent();

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
            _e.MoveEvent();

            var rotateSpeed = 0;

            if (flankSpeed < 1) rotateSpeed = 30;
            else rotateSpeed = -30;

            var dir = (_e.target.transform.position - _e.transform.position).normalized;
            var angle = Vector3.Angle(dir, _e.transform.forward);
            if (angle < 100)
            {
                var d = Vector3.Distance(_e.transform.position, _e.target.transform.position);

                _e.transform.RotateAround(_e.target.transform.position, Vector3.up, rotateSpeed * Time.deltaTime);

                if (_e.avoidVectObstacles != Vector3.zero && d>3) _e.transform.position += _e.transform.forward * 4 * Time.deltaTime;
                
                _e.transform.forward = dir;

            }

        }

        if (_e.timeToAttack) _e.dileyToAttack -= Time.deltaTime;
        if (_e.dileyToAttack <= 0 && !_e.onDamage)
        {

            var _dirToTarget = (_e.target.transform.position - _e.transform.position).normalized;
      
            var _distanceToTarget = Vector3.Distance(_e.transform.position, _e.target.transform.position);

            var shoulderMultiplier = 0.75f;

            RaycastHit hit;

            var leftRayPos = _e.attackPivot.transform.position - (_e.attackPivot.transform.right * shoulderMultiplier);
            var rightRayPos = _e.attackPivot.transform.position + (_e.attackPivot.transform.right * shoulderMultiplier);

            bool obstructed = false ;

            if (Physics.Raycast(leftRayPos, _e.attackPivot.transform.forward, out hit, _distanceToTarget))
            {
                if(hit.transform.name == _e.target.name) obstructed = true;
            }

            else if (Physics.Raycast(rightRayPos, _e.attackPivot.transform.forward, out hit, _distanceToTarget))
            {
                if (hit.transform.name == _e.target.name) obstructed = true;
            }

           
            else if (Physics.Raycast(_e.attackPivot.transform.position, _e.attackPivot.transform.forward, out hit, _distanceToTarget))
            {
                if (hit.transform.name == _e.target.name) obstructed = true;
            }

            if(obstructed)
            {
                flankSpeed = Random.Range(0, 2);
                _e.rb.AddForce(_e.transform.forward * _e.knockbackForce, ForceMode.Impulse);
                _e.StartCoroutine(_e.Resting());
                _e.cm.times++;
                if (_e.flank)
                {
                    _e.flank = false;
                    _e.cm.flanTicket = false;
                }
                _e.dileyToAttack = UnityEngine.Random.Range(4f, 6f);
                _e.maxDileyToAttack = _e.dileyToAttack;
            }

         
        }

        if (_e.onAttack)
        {
            _e.AttackEvent();
            var player = Physics.OverlapSphere(_e.attackPivot.position, _e.radiusAttack).Where(x => x.GetComponent<Model>()).Select(x => x.GetComponent<Model>()).FirstOrDefault();
            if (player != null)
            {
                player.GetDamage(_e.attackDamage, _e.transform, false);
                _e.rb.velocity = Vector3.zero;
                _e.rb.AddForce(-_e.transform.forward * _e.knockbackForce/1.5f  , ForceMode.Impulse);
                _e.onAttack = false;
            }
        }
    }

    public A_AttackMeleeWarrior(ModelE_Melee e)
    {
        _e = e;
    }
}
