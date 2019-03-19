using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ModelEnemyArcher : EnemyClass {

    public bool isPersuit;
    public bool isStuned;
    public bool isKnocked;
    public bool isBleeding;
    public bool isDead;
    public bool isOcuped;
    public bool isReloading;
    public bool isScape;
    public bool startScape;
    public bool timeToScape;
    bool startSearch;
    bool increaseFollowRadio;

    public EnemyAmmo munition;
    public List<Collider> obstacles;

    public Transform attackPivot;
    public LayerMask obstacle;
    public LayerMask player;
    public ViewerEnemy view;

    public float radObst;
    public float sightSpeed;
    public float viewAngleFollow;
    public float viewDistanceFollow;
    public float viewAngleScape;
    public float viewDistanceScape;
    public float viewDistanceAttack;
    public float viewAngleAttack;
    public float speed;
    float timeToShoot;
    public float shootTime;
    float starDistaceToFollow;
    int countTimesForSearch;

    public StateMachine sm;  

    public override IEnumerator Bleeding(float bleedingTime)
    {
        isBleeding = true;
        yield return new WaitForSeconds(bleedingTime);
        isBleeding = false;
    }

    public override IEnumerator FillFriends()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);
            myFriends.Clear();
        }              
    }

    public override IEnumerator Knocked(float knockedTime)
    {
        isKnocked = true;
        yield return new WaitForSeconds(knockedTime);
        isKnocked = false;
    }

    public override IEnumerator SearchingForPlayer()
    {
        yield return new WaitForSeconds(1f);
        /*  viewD     yield return new WaitForSeconds(knockedTime);istanceFollow = starDistaceToFollow;
          while (countTimesForSearch < 5)
          {
              Vector3 dirToTarget;
              var randomVect = new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2));

              if (isFollow) break;

              if (closeObstacle) dirToTarget = vectTurnDirecction;

              else
              {
                  dirToTarget = (target.transform.position - transform.position).normalized;
                  dirToTarget += randomVect;
              }

              dirToTarget.y = 0;
              currentMovement = new EnemySearching(this, target.transform, speed,dirToTarget);
              countTimesForSearch++;
              yield return new WaitForSeconds(2.5f);
          }
          countTimesForSearch = 0;
          currentMovement = null;
          increaseFollowRadio = true;
          */
    }

    public override IEnumerator Stuned(float stunedTimed)
    {
        isStuned = true;
        yield return new WaitForSeconds(stunedTimed);
        isStuned = false;
    }

    public IEnumerator ScapeTime()
    {
        isScape = false;
        startScape = true;
        yield return new WaitForSeconds(2);
        startScape = false;
        currentMovement = null;
        StartCoroutine(TimeToScapeAgain());
    }

    public IEnumerator TimeToScapeAgain()
    {
        timeToScape = false;
        yield return new WaitForSeconds(10);
        timeToScape = true;
    }

    // Use this for initialization
    void Start () {

        timeToShoot = shootTime;
        startCell = Physics.OverlapSphere(transform.position, 0.1f).Where(x => x.GetComponent<Cell>()).Select(x => x.GetComponent<Cell>()).First();
        sm = new StateMachine();
        sm.AddState(new S_Persuit(sm, this, target.GetComponent<Model>(), speed));
        sm.AddState(new S_Patrol(sm, this, speed));
        sm.AddState(new S_BackHome(sm, this, speed));
        sm.AddState(new S_Aiming(sm, this, target.transform, sightSpeed));
        view = GetComponent<ViewerEnemy>();
        timeToScape = true;
        munition = FindObjectOfType<EnemyAmmo>();
        rb = GetComponent<Rigidbody>();
        StartCoroutine(FillFriends());
        dileyToAttack = UnityEngine.Random.Range(2f, 3f);
        // ess = GetComponent<EnemyScreenSpace>();
    }
	
	// Update is called once per frame
	void Update () {

        WrapperStates();
        sm.Update();
               
        if (target != null && !isOcuped && timeToScape) isScape = SearchForTarget.SearchTarget(target, viewDistanceScape, viewAngleScape, gameObject, true);

        if (isScape) StartCoroutine(ScapeTime());

        if (target != null && !isAttack && !isOcuped && !startScape) isPersuit = SearchForTarget.SearchTarget(target, viewDistanceFollow, viewAngleFollow, gameObject, true);
        else isPersuit = false;

        if (target != null && !isOcuped && !startScape) isAttack = SearchForTarget.SearchTarget(target, viewDistanceAttack, viewAngleAttack, gameObject, true);
        else isAttack = false;

        if (isBleeding && !isOcuped) life -= bleedingDamage * Time.deltaTime;

        if (life <= 0)
            print(name + " is fucking dead!");
    }

    public void Attack()
    {
        timeToShoot -= Time.deltaTime;
       // if (timeToShoot > 4) view.AttackVisorLight();
        sm.SetState<S_Aiming>();

        if (timeToShoot<=0)
        {
            attackPivot.LookAt(target.transform.position);

            Vector3 localPlayerPos = transform.InverseTransformPoint(target.transform.position);
            Vector3 localEnemyPos = transform.InverseTransformPoint(attackPivot.position);
            Vector3 localPlayerDir = localPlayerPos - localEnemyPos;
            Vector3 v = localPlayerDir;
            v.y = 0f;
            localPlayerDir = Quaternion.FromToRotation(v, Vector3.forward) * localPlayerDir;
            Vector3 raycastDirection = transform.TransformDirection(localPlayerDir);

            if (Physics.Raycast(attackPivot.position, raycastDirection, Mathf.Infinity, player))
            {
                Arrow newArrow = munition.arrowsPool.GetObjectFromPool();
                newArrow.ammoAmount = munition;
                newArrow.transform.position = attackPivot.position;
                newArrow.transform.forward = transform.forward;
                Rigidbody arrowRb = newArrow.GetComponent<Rigidbody>();
                arrowRb.AddForce(new Vector3(transform.forward.x, attackPivot.forward.y + 0.2f, transform.forward.z) * 950 * Time.deltaTime, ForceMode.Impulse);
                // view.DesactivateLightAttack();
                timeToShoot = shootTime;               
            }
        }
    }

    public void Follow()
    {
        pathToTarget.Clear();
        var targetCell = Physics.OverlapSphere(target.transform.position, 0.1f).Where(x => x.GetComponent<Cell>()).Select(x => x.GetComponent<Cell>()).FirstOrDefault();
        var myCell = Physics.OverlapSphere(transform.position, 0.1f).Where(x => x.GetComponent<Cell>()).Select(x => x.GetComponent<Cell>()).First();
        if (targetCell != null)
        {
            pathToTarget.AddRange(myGridSearcher.Search(myCell, targetCell));
            sm.SetState<S_Persuit>();
        }
        else
        {
            isPersuit = false;
            pathToTarget.AddRange(myGridSearcher.Search(myCell, startCell));
            sm.SetState<S_BackHome>();
            isBackHome = true;
        }
    }

    public void BackHome()
    {

        pathToTarget.Clear();
        var myCell = Physics.OverlapSphere(transform.position, 0.1f).Where(x => x.GetComponent<Cell>()).Select(x => x.GetComponent<Cell>()).First();
        pathToTarget.AddRange(myGridSearcher.Search(myCell, startCell));
        var distance = Vector3.Distance(transform.position, startCell.transform.position);
        if (distance <= 1) isBackHome = false;
        sm.SetState<S_BackHome>();

    }

    public void Scape()
    {
       currentMovement = new EnemyScape(this, target.transform, speed);
    }

    public override void GetDamage(float damage)
    {
        life -= damage;
        //ess.UpdateLifeBar(life);
        dileyToAttack += 0.8f;
        if (life <= 0)
        {
            isDead = true;
            Destroy(gameObject);
        }
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

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, viewDistanceScape);

        Vector3 rightLimit3 = Quaternion.AngleAxis(viewAngleScape, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit3 * viewDistanceScape));

        Vector3 leftLimit3 = Quaternion.AngleAxis(-viewAngleScape, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit3 * viewDistanceScape));
    }

    public void WrapperStates()
    {
        if (isDead || isKnocked || isStuned) isOcuped = true;
        else isOcuped = false;
    }

  
}
