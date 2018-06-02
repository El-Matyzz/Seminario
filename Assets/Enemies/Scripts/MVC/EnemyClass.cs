using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyClass: MonoBehaviour  {

    public abstract void GetDamage(float damage);
    public abstract IEnumerator Stuned(float stunedTimed);
    public abstract IEnumerator SearchingForPlayer();
    public abstract IEnumerator Knocked(float knockedTime);
    public abstract IEnumerator Bleeding(float bleedingTime);
    public abstract IEnumerator FillFriends();
    public bool createAttack;
    public bool isAttack;
    public bool myTimeToAttack;
    public float dileyToAttack;
    public float bleedingDamage;
    public Vector3 vectAvoidance;
    public Rigidbody rb;
    public GameObject target;
    public ESMovemnt currentMovement;
    public List<EnemyClass> myFriends = new List<EnemyClass>();

}
