using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A_WarriorRetreat : i_EnemyActions
{
    ModelE_Melee _e;

    public void Actions()
    {

        if (_e.onRetreat && !_e._view._anim.GetBool("Attack"))
        {

            RaycastHit hit;

            if (Physics.Raycast(_e.transform.position, -_e.transform.forward, out hit, 1))
            {
                _e.onRetreat = false;
                _e.IdleEvent();
            }

            _e.rb.MovePosition(_e.rb.position - _e.transform.forward * _e.speed * Time.deltaTime);
        }

        else if (!_e.onRetreat) _e.IdleEvent();
    }

    public A_WarriorRetreat( ModelE_Melee e)
    {
        _e = e;
    }
}
