using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelEnemy :  EnemyClass
{

    public bool isFollow;
    public bool isAttack;
    public bool myTimeToAttack;
    public bool isStuned;
    public bool isKnocked;
    public bool isBleeding;

    public List<ModelEnemy> myFriends = new List<ModelEnemy>();

    public GameObject target;

    public Rigidbody rb;

    public float attackForce;
    public float dileyToAttack;
    public float viewAngleFollow;
    public float viewDistanceFollow;
    public float distanceToTraget;
    public float distanceToAttack;
    public float speed;
    public float life;
    public float bleedingDamage;
    
    public ESMovemnt currentMovement;

    public override IEnumerator Stuned(float stunedTime)
    {
        isStuned = true;
        yield return new WaitForSeconds(stunedTime);
        isStuned = false;
    }

    public override IEnumerator Knocked(float knockedTime)
    {
        isKnocked = true;
        yield return new WaitForSeconds(knockedTime);
        isKnocked = false;
    }

    public override IEnumerator Bleeding(float bleedingTime)
    {
        isBleeding = true;
        yield return new WaitForSeconds(bleedingTime);
        isBleeding = false;
    }

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        FillMyFriendsList();
    }

    // Update is called once per frame
    void Update()
    {
        distanceToTraget = Vector3.Distance(transform.position, target.transform.position);

        if (currentMovement != null) currentMovement.ESMove();

        if (!isAttack) isFollow = SearchForTarget.SearchTarget(target, viewDistanceFollow, viewAngleFollow, gameObject, true);
        else isFollow = false;

        if (distanceToTraget <= distanceToAttack) isAttack = true;
        else isAttack = false;

        if (isBleeding) life -= bleedingDamage * Time.deltaTime;

    }

    public void Follow()
    {
        currentMovement = new EnemyFollow(this, target, speed);
    }

    public void Attack()
    {

        int amountOfAttackers=0;

        foreach (var item in myFriends) if (item.myTimeToAttack== true) amountOfAttackers++;

        if (amountOfAttackers < 2)
        {
            myTimeToAttack = true;
            currentMovement = new EnemyMeleAttack(rb, attackForce, this, target);
        }   
    }



    public void FillMyFriendsList()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, 9999999999999999);
        foreach (Collider hit in colliders)
        {
            if (hit.gameObject.GetComponent(typeof(ModelEnemy))) 
            {
                myFriends.Add(hit.GetComponent<ModelEnemy>());
            }  
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDistanceFollow);

        Vector3 rightLimit = Quaternion.AngleAxis(viewAngleFollow, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit * viewDistanceFollow));

        Vector3 leftLimit = Quaternion.AngleAxis(-viewAngleFollow, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit * viewDistanceFollow));
    }

    public override void GetDamage(float damage)
    {
        life -= damage;
    }

    
}
