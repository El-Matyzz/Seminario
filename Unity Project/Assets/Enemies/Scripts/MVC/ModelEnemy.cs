using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelEnemy :  EnemyClass
{

    public bool isFollow;
    public bool isStuned;
    public bool isKnocked;
    public bool isBleeding;
    public bool isDead;
    public bool isOcuped;
    bool startSearch;
    bool increaseFollowRadio;

    public SpatialGrid grid;
    
    public List<Collider> obstacles;

    public Transform attackPivot;
    public Collider closeObstacle;
    public LayerMask obstacle;

    public float radObst;
    public float attackForce;
    public float radiusAttack;
    public float viewAngleFollow;
    public float viewDistanceFollow;
    public float distanceToTraget;
    public float viewDistanceAttack;
    public float viewAngleAttack;
    public float speed;
    public float life;
    public float bleedingDamage;
    public float radFlock;
    public float separationWeight;
    float starDistaceToFollow;
    int countTimesForSearch;

    public override IEnumerator FillFriends()
    {
        grid.aux = false;
        yield return new WaitForSeconds(0.25f);
        myFriends.Clear();
        StartCoroutine(FillFriends());
    }

    public override IEnumerator SearchingForPlayer()
    {
        viewDistanceFollow = starDistaceToFollow;
        while (countTimesForSearch < 5)
        {
            if (isFollow) break;
            currentMovement = new EnemySearching(this, target.transform, speed);
            countTimesForSearch++;
            yield return new WaitForSeconds(2.5f);
        }
        countTimesForSearch = 0;
        currentMovement = null;
        increaseFollowRadio = true;
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
        starDistaceToFollow = viewDistanceFollow;
        increaseFollowRadio = true;
        grid = FindObjectOfType<SpatialGrid>();
    }

    // Update is called once per frame
    void Update()
    {
        WrapperStates();
        GetObstacles();
        closeObstacle = GetCloserOb();
        vectAvoidance = getObstacleAvoidance();

        if (currentMovement != null && !isOcuped) currentMovement.ESMove();

        if(!isAttack && !isOcuped) isFollow = SearchForTarget.SearchTarget(target, viewDistanceFollow, viewAngleFollow, gameObject, true);

        if (!isFollow)
        {
            if (startSearch)
            {
                StartCoroutine(SearchingForPlayer());
                startSearch = false;
            }
        }
        else
        {   
            if(increaseFollowRadio)
            {
                viewDistanceFollow *= 3;
                increaseFollowRadio = false;
            }
            startSearch = true;          
        }
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

        if (createAttack)
        {
            Collider[] col = Physics.OverlapSphere(attackPivot.position, radiusAttack);
            foreach (var item in col)
            {
                if (item.GetComponent<Model>())
                {
                    item.GetComponent<Model>().GetDamage(10);
                    createAttack = false;
                }
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


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistanceAttack);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPivot.position, radiusAttack);

    }

    public override void GetDamage(float damage, Transform player)
    {
        life -= damage;
        dileyToAttack += 0.8f;
        rb.AddForce(player.forward * 5, ForceMode.Impulse);
        if (life <= 0) isDead = true;
    }

    public void WrapperStates()
    {
        if (isDead || isKnocked || isStuned) isOcuped = true;
        else isOcuped = false;
    }

    Collider GetCloserOb()
    {
        if (obstacles.Count > 0)
        {
            Collider closer = null;
            float dist = 99999;
            foreach (var item in obstacles)
            {
                var newDist = Vector3.Distance(item.transform.position, transform.position);
                if (newDist < dist)
                {
                    dist = newDist;
                    closer = item;
                }
            }
            return closer;
        }
        else
            return null;
    }

    Vector3 getObstacleAvoidance()
    {
        if (closeObstacle)
            return transform.position - closeObstacle.transform.position;
        else return Vector3.zero;
    }

    void GetObstacles()
    {
        obstacles.Clear();
        obstacles.AddRange(Physics.OverlapSphere(transform.position, radObst, obstacle));
    }

   
}
