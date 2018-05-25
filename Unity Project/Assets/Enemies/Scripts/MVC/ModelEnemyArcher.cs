using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelEnemyArcher : EnemyClass {

    public bool isFollow;
    public bool isStuned;
    public bool isKnocked;
    public bool isBleeding;
    public bool isDead;
    public bool isOcuped;
    public bool isReloading;
    bool startSearch;
    bool increaseFollowRadio;

    public SpatialGrid grid;
    public EnemyAmmo munition;
    public List<Collider> obstacles;

    public Transform attackPivot;
    public Collider closeObstacle;
    public LayerMask obstacle;

    public float radObst;
    public float attackForce;
    public float sightSpeed;
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


    public IEnumerator Reloading()
    {
        isReloading = true;
        yield return new WaitForSeconds(5);
        isReloading = false;
    }


    public override IEnumerator Bleeding(float bleedingTime)
    {
        isBleeding = true;
        yield return new WaitForSeconds(bleedingTime);
        isBleeding = false;
    }

    public override IEnumerator FillFriends()
    {
        grid.aux = false;
        yield return new WaitForSeconds(0.25f);
        myFriends.Clear();
        StartCoroutine(FillFriends());
    }

    public override IEnumerator Knocked(float knockedTime)
    {
        isKnocked = true;
        yield return new WaitForSeconds(knockedTime);
        isKnocked = false;
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

    public override IEnumerator Stuned(float stunedTimed)
    {
        isStuned = true;
        yield return new WaitForSeconds(stunedTimed);
        isStuned = false;
    }

    // Use this for initialization
    void Start () {

        grid = FindObjectOfType<SpatialGrid>();
        munition = FindObjectOfType<EnemyAmmo>();
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {

        WrapperStates();

        if (currentMovement != null && !isOcuped) currentMovement.ESMove();

        if (!isAttack && !isOcuped) isFollow = SearchForTarget.SearchTarget(target, viewDistanceFollow, viewAngleFollow, gameObject, true);
        else isFollow = false;

        if (target != null && !isOcuped) isAttack = SearchForTarget.SearchTarget(target, viewDistanceAttack, viewAngleAttack, gameObject, true);

        if (isBleeding && !isOcuped) life -= bleedingDamage * Time.deltaTime;
    }

    public void Attack()
    {
        currentMovement = new EnemySightFollow(this, target.transform, sightSpeed);
        if (!isReloading)
        {
            Arrow newArrow = munition.arrowsPool.GetObjectFromPool();
            newArrow.ammoAmount = munition;
            newArrow.transform.position = attackPivot.position;
            newArrow.transform.forward = transform.forward;
            Rigidbody arrowRb = newArrow.GetComponent<Rigidbody>();
            arrowRb.AddForce(new Vector3(transform.forward.x, attackPivot.forward.y + 0.3f, transform.forward.z) * 700 * Time.deltaTime, ForceMode.Impulse);

            StartCoroutine(Reloading());
        }
    }

    public void Follow()
    {
        currentMovement = new EnemyFollow(this, target, speed);
    }

    public override void GetDamage(float damage, Transform player)
    {
        life -= damage;
        dileyToAttack += 0.8f;
        rb.AddForce(player.forward * 5, ForceMode.Impulse);
        if (life <= 0) isDead = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistanceAttack);

        Vector3 rightLimit = Quaternion.AngleAxis(viewAngleAttack, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit * viewDistanceAttack));

        Vector3 leftLimit = Quaternion.AngleAxis(-viewAngleAttack, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit * viewDistanceAttack));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDistanceFollow);

        Vector3 rightLimit2 = Quaternion.AngleAxis(viewAngleFollow, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit2 * viewDistanceFollow));

        Vector3 leftLimit2 = Quaternion.AngleAxis(-viewAngleFollow, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit2 * viewDistanceFollow));

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
