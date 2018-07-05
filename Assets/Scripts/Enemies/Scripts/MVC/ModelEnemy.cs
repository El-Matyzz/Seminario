using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ModelEnemy :  EnemyClass
{
    public ViewerEnemy view;

    public bool isFollow;
    public bool isStuned;
    public bool isKnocked;
    public bool isBleeding;
    public bool isDead;
    public bool isOcuped;
    bool startSearch;
    bool increaseFollowRadio;
    
    public List<Collider> obstacles;

    public Transform attackPivot;
    public LayerMask obstacle;

    public float radObst;
    public float attackForce;
    public float radiusAttack;
    public float viewAngleFollow;
    public float viewDistanceFollow;
    public float distanceToTraget;
    public float viewDistanceAttack;
    public float viewAngleAttack;
    public float fightingTogetherRadius;
    public float speed;
    public float radFlock;
    public float separationWeight;
    public float timeToSearch;
    float starDistaceToFollow;
    public int countTimesForSearch;
    public float knockbackForce;
   

    public override IEnumerator FillFriends()
    {
        while (true)
        {
            IEnumerable<ModelEnemy> friends = Physics.OverlapSphere(transform.position, fightingTogetherRadius)
                .Select(x => x.GetComponent<ModelEnemy>())
                .Where(x => x && x != this && !myFriends.Contains(x) && !x.isDead);
            myFriends = friends.ToList();
            yield return new WaitForSeconds(0.25f);
            myFriends.Clear();
        }
    }

    public override IEnumerator SearchingForPlayer()
    {

        var maxTime = timeToSearch;
        viewDistanceFollow = starDistaceToFollow;
        while (countTimesForSearch < 5)
        {
            Vector3 dirToTarget;
            var randomVect = new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2));

            if (isFollow) break;

            if (!firstSearch) dirToTarget = (target.transform.position - transform.position).normalized;
            else dirToTarget = randomVect.normalized;

            dirToTarget.y = 0;
            currentMovement = new EnemySearching(this, target.transform, speed, dirToTarget);
            countTimesForSearch++;
            firstSearch = true;
            yield return new WaitForSeconds(timeToSearch);
            timeToSearch = 2.5f;
        }
        countTimesForSearch = 0;
        currentMovement = null;
        increaseFollowRadio = true;
        firstSearch = false;
        timeToSearch = maxTime;
    }

    public IEnumerator PatrolCorrutine()
    {
        while (true)
        {
            Vector3 dirToTarget;
            var randomVect = new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2));
            if (isFollow) break;

            if (closeObstacle) dirToTarget = -transform.forward;
            else dirToTarget = randomVect;

            dirToTarget.y = 0;
            currentMovement = new EnemyPatrol(this, target.gameObject, speed, dirToTarget);

            yield return new WaitForSeconds(2.5f);
        }
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

    public IEnumerator AttackCorrutine()
    {
        var aux = 0;
        while (aux < 10 && createAttack)
        {
            Collider[] col = Physics.OverlapSphere(attackPivot.position, radiusAttack);
            foreach (var item in col)
            {
                if (item.GetComponent<Model>())
                {

                    item.GetComponent<Model>().GetDamage(10, transform, false);
                    createAttack = false;
                }
            }
            aux++;
            yield return new WaitForSeconds(0.15f);
        }
    }
    // Use this for initialization
    void Start()
    {
        view = GetComponent<ViewerEnemy>();
        StartCoroutine(PatrolCorrutine());
        startPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(FillFriends());
        starDistaceToFollow = viewDistanceFollow;
        increaseFollowRadio = true;
        ess = GetComponent<EnemyScreenSpace>();

        attackForce *= Random.Range(0.8f, 1.2f);
        viewDistanceFollow *= Random.Range(0.8f, 1.2f);
        viewDistanceAttack *= Random.Range(0.8f, 1.2f);
        speed *= Random.Range(0.8f, 1.2f);
        life *= Random.Range(0.8f, 1.2f);
        knockbackForce *= Random.Range(0.8f, 1.2f);
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPatrol = Vector3.Distance(transform.position, startPosition);
        WrapperStates();
        GetObstacles();
        closeObstacle = GetCloserOb();
        vectAvoidance = getObstacleAvoidance();
        vectTurnDirecction = GetTurnDireccion();

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

        if (life <= 0)
            Destroy(gameObject);
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

    public void GetBack(Vector3 src)
    {

       rb.velocity = Vector3.zero; 
       Vector3 dir = (src - transform.position).normalized;
       dir.y = 0;
       rb.AddForce(-dir * knockbackForce);
        
    }

    public void OnCollisionEnter(Collision c)
    {

        if (c.gameObject.GetComponent<Model>() && createAttack)
            GetBack(c.transform.position);
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDistanceFollow);

        Vector3 rightLimit = Quaternion.AngleAxis(viewAngleFollow, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit * viewDistanceFollow));

        Vector3 leftLimit = Quaternion.AngleAxis(-viewAngleFollow, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit * viewDistanceFollow));


        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, fightingTogetherRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistanceAttack);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPivot.position, radiusAttack);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startPosition, areaToPatrol);
    }

    public override void GetDamage(float damage)
    {
        life -= damage;
        ess.UpdateLifeBar(life);
        dileyToAttack += 0.25f;
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
            return transform.position + closeObstacle.transform.position;
        else return Vector3.zero;
    }

    Vector3 GetTurnDireccion()
    {
        if (closeObstacle) return -transform.forward;
        else return Vector3.zero;
    }

    void GetObstacles()
    {
        obstacles.Clear();
        obstacles.AddRange(Physics.OverlapSphere(transform.position, radObst, obstacle));
    }
}
