using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyClass: MonoBehaviour  {

    public abstract void GetDamage(float damage);
    public abstract IEnumerator Stuned(float stunedTimed);
    public abstract IEnumerator Knocked(float knockedTime);
    public abstract IEnumerator Bleeding(float bleedingTime);



}
