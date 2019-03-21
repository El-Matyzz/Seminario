using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ModelEnemy :  EnemyClass
{
    public ViewerEnemy view;
    public EnemyCombatManager cm;

    public bool isPersuit;
    public bool isStuned;
    public bool isKnocked;
    public bool isBleeding;
    public bool isDead;
    public bool isOcuped;
    public bool subscribed;
    public bool timeToAttack;
    public bool isOnPatrol;
    public bool resting;
    bool OnAttack;

    bool startSearch;
    bool increaseFollowRadio;
    
    public List<Collider> obstacles;

    public Transform attackPivot;
    public LayerMask layerObst;
    public StateMachine sm;
    List<Cell> playerCells = new List<Cell>();
    public float attackDamage;
    public float radiusAttack;
    public float viewAnglePersuit;
    public float viewDistancePersuit;
    public float distanceToTraget;
    public float viewDistanceAttack;
    public float viewAngleAttack;
    public float radiusAvoid;
    public float avoidWeight;
    public float speed;
    public float knockbackForce;
    float maxDileyToAttack;
    float timeToChangePatrol;
      
    public IEnumerator Resting()
    {
        resting = true;
        OnAttack = true;
        yield return new WaitForSeconds(2);
        resting = false;
        OnAttack = false;
    }

    public IEnumerator PatrolCorrutine()
    {
        yield return new WaitForSeconds(1);
    }

    public override IEnumerator Stuned(float stunedTime)
    {
        yield return new WaitForSeconds(1);
    }

    public override IEnumerator Knocked(float knockedTime)
    {
        yield return new WaitForSeconds(1);
    }

    public override IEnumerator Bleeding(float bleedingTime)
    {
        yield return new WaitForSeconds(1);
    }

    public IEnumerator AttackCorrutine()
    {
        yield return new WaitForSeconds(1);
    }

    private void Awake()
    {
        pathToTarget = new List<Cell>();
    }

    void Start()
    {
        startRotation = transform.forward;
        startCell = Physics.OverlapSphere(transform.position, 0.1f).Where(x => x.GetComponent<Cell>()).Select(x => x.GetComponent<Cell>()).First();
        cellToPatrol = startCell;     
        sm = new StateMachine();
        sm.AddState(new S_Persuit(sm, this, target.GetComponent<Model>(), speed));
        sm.AddState(new S_Waiting(sm, this, this, target));
        sm.AddState(new S_Patrol(sm, this, speed));
        sm.AddState(new S_BackHome(sm, this, speed));
        sm.SetState<S_Patrol>();
        rb = GetComponent<Rigidbody>();
        cm = FindObjectOfType<EnemyCombatManager>();
        dileyToAttack = UnityEngine.Random.Range(2f, 3f);
        int randomCell1 = UnityEngine.Random.Range(0, myGrid.size.x);
        int randomCell2 = UnityEngine.Random.Range(0, myGrid.size.x);
        cellToPatrol = myGrid.cells[randomCell1,randomCell2];
        timeToChangePatrol = 10;
       
       
    }

    // Update is called once per frame
    void Update()
    {
        sm.Update();
        playerCells.Clear();
        playerCells.AddRange(Physics.OverlapSphere(target.transform.position, 0.1f).Where(x => x.GetComponent<Cell>()).Select(x => x.GetComponent<Cell>()));
        
        if (!isAttack && !isOcuped && playerCells.Count >0 ) isPersuit = SearchForTarget.SearchTarget(target, viewDistancePersuit, viewAnglePersuit, gameObject, true);

        if (target != null && !isOcuped && playerCells.Count > 0) isAttack = SearchForTarget.SearchTarget(target, viewDistanceAttack, viewAngleAttack, gameObject, true);

        if (isAttack)
        {
            avoidVect = avoidance() * avoidWeight;
            Attack();
        }

        if (isBackHome && !isAttack && !isPersuit && !isOcuped) BackHome();

        if (!isAttack && !isPersuit && !isOcuped && !isBackHome) isOnPatrol = true;
        else isOnPatrol = false;

    }

    public void Persuit()
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
        if (distance <= 2)
        {
            transform.forward = startRotation;
            isBackHome = false;
        }
        sm.SetState<S_BackHome>();

    }

    public void Patrol()
    {
        timeToChangePatrol -= Time.deltaTime;
        if(timeToChangePatrol<=0)
        {
            int randomCell1 = UnityEngine.Random.Range(0, myGrid.size.x);
            int randomCell2 = UnityEngine.Random.Range(0, myGrid.size.x);
            cellToPatrol = myGrid.cells[randomCell1, randomCell2];
            timeToChangePatrol = 10;
        }
        pathToTarget.Clear();
        var myCell = Physics.OverlapSphere(transform.position, 0.1f).Where(x => x.GetComponent<Cell>()).Select(x => x.GetComponent<Cell>()).First();
        pathToTarget.AddRange(myGridSearcher.Search(myCell, cellToPatrol));
        
        sm.SetState<S_Patrol>();
    }

    public void WaitTurn()
    {
        if (!resting && cm.times > 0 && !timeToAttack)
        {
            timeToAttack = true;
            cm.times--;
        }
       
        sm.SetState<S_Waiting>();
    }

    public void Attack()
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
                    rb.AddForce(-transform.forward * knockbackForce * 1.5f, ForceMode.Impulse);
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



    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewDistancePersuit);

        Vector3 rightLimit = Quaternion.AngleAxis(viewAnglePersuit, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit * viewDistancePersuit));

        Vector3 leftLimit = Quaternion.AngleAxis(-viewAnglePersuit, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit * viewDistancePersuit));

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistanceAttack);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPivot.position, radiusAttack);

    }

    public override void GetDamage(float damage)
    {
        dileyToAttack += 0.25f;
        if (dileyToAttack > maxDileyToAttack)
        {
            dileyToAttack = maxDileyToAttack;
        }
        life -= damage;
        if (life <= 0) Dead();
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
