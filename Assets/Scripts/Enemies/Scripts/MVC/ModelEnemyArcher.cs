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
    public bool isOnPatrol;
    public bool isAttackMelle;
    bool startSearch;
    bool increaseFollowRadio;
    public bool resting;
    public bool timeToAttack;
    bool OnAttack;

    public EnemyAmmo munition;
    public List<Collider> obstacles;

    public EnemyCombatManager cm;
    public Transform attackPivot;
    public LayerMask obstacle;
    public LayerMask player;
    public ViewerEnemy view;
    List<Cell> playerCells = new List<Cell>();

    public float radObst;
    public float sightSpeed;
    public float viewAngleFollow;
    public float viewDistanceFollow;
    public float viewDistanceAttackRange;
    public float viewAngleAttackRange;
    public float viewAngleAttackMelle;
    public float viewDistanceAttackMelle;
    public float speed;
    float timeToShoot;
    public float shootTime;
    float starDistaceToFollow;
    int countTimesForSearch;
    public float knockbackForce;
    float maxDileyToAttack;
    public float radiusAttack;
    public float attackDamage;
    public float radiusAvoid;
    public float avoidWeight;

    public StateMachine sm;  

    public override IEnumerator Bleeding(float bleedingTime)
    {
        isBleeding = true;
        yield return new WaitForSeconds(bleedingTime);
        isBleeding = false;
    }

    public override IEnumerator Knocked(float knockedTime)
    {
        isKnocked = true;
        yield return new WaitForSeconds(knockedTime);
        isKnocked = false;
    }

    public IEnumerator Resting()
    {
        resting = true;
        OnAttack = true;
        yield return new WaitForSeconds(2);
        resting = false;
        OnAttack = false;
    }

    public override IEnumerator Stuned(float stunedTimed)
    {
        isStuned = true;
        yield return new WaitForSeconds(stunedTimed);
        isStuned = false;
    }

    // Use this for initialization
    void Start () {

        startRotation = transform.forward;
        timeToShoot = shootTime;
        startCell = Physics.OverlapSphere(transform.position, 0.1f).Where(x => x.GetComponent<Cell>()).Select(x => x.GetComponent<Cell>()).First();
        sm = new StateMachine();
        sm.AddState(new S_Persuit(sm, this, target.GetComponent<Model>(), speed));
        sm.AddState(new S_Patrol(sm, this, speed));
        sm.AddState(new S_BackHome(sm, this, speed));
        sm.AddState(new S_Aiming(sm, this, target.transform, sightSpeed));
        view = GetComponent<ViewerEnemy>();
        munition = FindObjectOfType<EnemyAmmo>();
        rb = GetComponent<Rigidbody>();
        dileyToAttack = UnityEngine.Random.Range(2f, 3f);
        // ess = GetComponent<EnemyScreenSpace>();
    }
	
	// Update is called once per frame
	void Update () {

        WrapperStates();
        sm.Update();
        playerCells.Clear();
        playerCells.AddRange(Physics.OverlapSphere(target.transform.position, 0.1f).Where(x => x.GetComponent<Cell>()).Select(x => x.GetComponent<Cell>()));

        if (target != null && !isAttack && !isAttackMelle && !isOcuped && playerCells.Count>0) isPersuit = SearchForTarget.SearchTarget(target, viewDistanceFollow, viewAngleFollow, gameObject, true);
        else isPersuit = false;

        if (target != null && !isOcuped  && playerCells.Count > 0 && !isAttackMelle) isAttack = SearchForTarget.SearchTarget(target, viewDistanceAttackRange, viewAngleAttackRange, gameObject, true);
        else isAttack = false;

        if (target != null && !isOcuped && playerCells.Count > 0) isAttackMelle = SearchForTarget.SearchTarget(target, viewDistanceAttackMelle, viewAngleAttackMelle, gameObject, true);
        else isAttackMelle = false;

        if (isAttackMelle)
        {
            avoidVect = avoidance() * avoidWeight;
            AttackMelle();
        }

        if (isBleeding && !isOcuped) life -= bleedingDamage * Time.deltaTime;

        if (isBackHome && !isAttack && !isPersuit && !isOcuped && !isAttackMelle) BackHome();

        if (life <= 0)
            print(name + " is fucking dead!");

        if (!isAttack && !isPersuit && !isOcuped && !isBackHome && !isAttackMelle) isOnPatrol = true;
        else isOnPatrol = false;
    }

    public void AttackRange()
    {

        timeToShoot -= Time.deltaTime;
       // if (timeToShoot > 4) view.AttackVisorLight();
        sm.SetState<S_Aiming>();
        target.GetComponent<Model>().CombatState();
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
                timeToShoot = shootTime;               
            }
        }
    }

    public void Patrol()
    {       
        Quaternion rotateAngle = Quaternion.LookRotation(transform.forward + new Vector3(Mathf.Sin(Time.time * 0.5f),0,0), Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotateAngle, 5 * Time.deltaTime);
    }

    public void AttackMelle()
    {
        target.GetComponent<Model>().CombatState();
        var targetCell = Physics.OverlapSphere(target.transform.position, 0.1f).Where(x => x.GetComponent<Cell>()).Select(x => x.GetComponent<Cell>()).FirstOrDefault();
        var myCell = Physics.OverlapSphere(transform.position, 0.1f).Where(x => x.GetComponent<Cell>()).Select(x => x.GetComponent<Cell>()).First();

        if (targetCell != null)
        {
            if (timeToAttack) dileyToAttack -= Time.deltaTime;
            if (dileyToAttack <= 0)
            {
                rb.AddForce(transform.forward * knockbackForce, ForceMode.Impulse);
                timeToAttack = false;
                StartCoroutine(Resting());
                cm.ResetTimes();
                dileyToAttack = UnityEngine.Random.Range(4f, 6f);
                maxDileyToAttack = dileyToAttack;
            }

            if (OnAttack)
            {
                var player = Physics.OverlapSphere(attackPivot.position, radiusAttack).Where(x => x.GetComponent<Model>()).Select(x => x.GetComponent<Model>()).FirstOrDefault();
                if (player != null)
                {
                    player.GetDamage(attackDamage, transform, false);
                    rb.AddForce(-transform.forward * knockbackForce * 1.3f, ForceMode.Impulse);
                    OnAttack = false;
                }
            }
        }
        else
        {
            isAttack = false;
            pathToTarget.AddRange(myGridSearcher.Search(myCell, startCell));
            sm.SetState<S_BackHome>();
            isBackHome = true;
        }
    }

    public void Waiting()
    {
        if (!resting && cm.times > 0 && !timeToAttack)
        {
            timeToAttack = true;
            cm.times--;
        }

        sm.SetState<S_Waiting>();
    }

    public void Persuit()
    {
        Debug.Log(1);
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
        Debug.Log(2);
        pathToTarget.Clear();
        var myCell = Physics.OverlapSphere(transform.position, 0.1f).Where(x => x.GetComponent<Cell>()).Select(x => x.GetComponent<Cell>()).First();
        pathToTarget.AddRange(myGridSearcher.Search(myCell, startCell));
        var distance = Vector3.Distance(transform.position, startCell.transform.position);
        if (distance <= 2)
        {
            transform.forward = startRotation;
            isBackHome = false;
        }
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

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, viewDistanceAttackMelle);

        Vector3 rightLimit = Quaternion.AngleAxis(viewAngleAttackMelle, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit * viewDistanceAttackMelle));

        Vector3 leftLimit = Quaternion.AngleAxis(-viewAngleAttackMelle, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit * viewDistanceAttackMelle));

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistanceAttackRange);

        Vector3 rightLimit2 = Quaternion.AngleAxis(viewAngleAttackRange, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit2 * viewDistanceAttackRange));

        Vector3 leftLimit2 = Quaternion.AngleAxis(-viewAngleAttackRange, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit2 * viewDistanceAttackRange));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDistanceFollow);

        Vector3 rightLimit3 = Quaternion.AngleAxis(viewAngleFollow, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit3 * viewDistanceFollow));

        Vector3 leftLimit3 = Quaternion.AngleAxis(-viewAngleFollow, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit3 * viewDistanceFollow));

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPivot.position, radiusAttack);
    }

    public void WrapperStates()
    {
        if (isDead || isKnocked || isStuned) isOcuped = true;
        else isOcuped = false;
    }

    Vector3 avoidance()
    {
        var friends = Physics.OverlapSphere(transform.position, radiusAvoid).Where(x => x.GetComponent<ModelEnemy>() && x != this).Select(x => x.GetComponent<ModelEnemy>());
        if (friends.Count() > 0)
        {
            var dir = transform.position - friends.First().transform.position;
            return dir.normalized;
        }
        else return Vector3.zero;
    }

    public void Dead()
    {
        gameObject.SetActive(false);
        isDead = true;
    }
}
