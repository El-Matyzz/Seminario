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
    public bool isDead;
    public bool isOcuped;

    public SpatialGrid grid;

    public List<ModelEnemy> myFriends = new List<ModelEnemy>();

    public GameObject target;

    public Rigidbody rb;

    public float attackForce;
    public float dileyToAttack;
    public float viewAngleFollow;
    public float viewDistanceFollow;
    public float distanceToTraget;
    public float viewDistanceAttack;
    public float viewAngleAttack;
    public float speed;
    public float life;
    public float bleedingDamage;
    
    public ESMovemnt currentMovement;

    public IEnumerator FillFriends()
    {
        grid.aux = false;
        yield return new WaitForSeconds(0.25f);
        myFriends.Clear();
        StartCoroutine(FillFriends());
    }


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
        StartCoroutine(FillFriends());
    }

    // Update is called once per frame
    void Update()
    {
        WrapperStates();

        if(target !=null && !isOcuped) distanceToTraget = Vector3.Distance(transform.position, target.transform.position);

        if (currentMovement != null && !isOcuped) currentMovement.ESMove();

        if (!isAttack && target != null && !isOcuped) isFollow = SearchForTarget.SearchTarget(target, viewDistanceFollow, viewAngleFollow, gameObject, true);
        
        else if (isAttack && target != null && !isOcuped) isFollow = false;
        
        if (target != null && !isOcuped) isAttack = SearchForTarget.SearchTarget(target, viewDistanceAttack, viewAngleAttack, gameObject, true);

        if (isBleeding && !isOcuped) life -= bleedingDamage * Time.deltaTime;

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

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDistanceFollow);

        Vector3 rightLimit = Quaternion.AngleAxis(viewAngleFollow, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit * viewDistanceFollow));

        Vector3 leftLimit = Quaternion.AngleAxis(-viewAngleFollow, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit * viewDistanceFollow));


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistanceAttack);

        
    }

    public override void GetMeleDamage(float damage, Transform player)
    {
        life -= damage;
        rb.AddForce(player.forward * 5, ForceMode.Impulse);
        if (life <= 0) isDead = true;
    }

    public override void GetDamage(float damage)
    {
        dileyToAttack += 0.8f;
        life -= damage;
        if (life <= 0) isDead = true;
    }

    public void WrapperStates()
    {
        if (isDead || isKnocked || isStuned) isOcuped = true;
        else isOcuped = false;
    }

   
}
