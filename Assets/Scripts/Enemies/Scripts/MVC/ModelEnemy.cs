﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ModelEnemy : EnemyClass
{
    public ViewerEnemy view;
    public EnemyCombatManager cm;

    public bool isPersuit;
    public bool isStuned;
    public bool isKnocked;
    public bool isBleeding;
    public bool isDead;
    public bool isOcuped;
    public bool timeToAttack;
    public bool isOnPatrol;
    public bool resting;
    public bool lostTarget;
    public bool flankTarget;
    bool OnAttack;
    bool startback;
    bool startSearch;
    bool increaseFollowRadio;
   

    public List<Collider> obstacles;

    public Transform attackPivot;
    public LayerMask layerObst;
    public LayerMask layerPlayer;
    public StateMachine sm;
    List<Cell> transitableCells = new List<Cell>();
    public float attackDamage;
    public float distanceToBack;
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
    public float timeToChangePatrol;
    public float timeOfLook;

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
        transitableCells.AddRange(GetTransitableCells());

        startCell = GetCloseCell(transform.position);
        sm = new StateMachine();
        sm.AddState(new S_Persuit(sm, this, target.GetComponent<Model>(), speed));
        sm.AddState(new S_LookForTarget(sm, this, speed));
        sm.AddState(new S_Waiting(sm, this, this, target, speed));
        sm.AddState(new S_Patrol(sm, this, speed));
        sm.AddState(new S_BackHome(sm, this, speed));
        sm.SetState<S_Patrol>();
        rb = GetComponent<Rigidbody>();
        cm = FindObjectOfType<EnemyCombatManager>();
        dileyToAttack = UnityEngine.Random.Range(2f, 3f);
        cellToPatrol = GetRandomCell();
        timeToChangePatrol = 0;
        timeOfLook = 10;
        

    }

    void Update()
    {
        sm.Update();
       
        avoidVectObstacles = AvoidObstacles();

        var d = Vector3.Distance(startCell.transform.position, transform.position);

        if (d > distanceToBack) isBackHome = true;

        if (!isAttack && !isOcuped && !isBackHome && SearchForTarget.SearchTarget(target, viewDistancePersuit, viewAnglePersuit, transform, true, layerPlayer)) isPersuit = true;
        else isPersuit = false;

        if (target != null && !isOcuped && !isBackHome && SearchForTarget.SearchTarget(target, viewDistanceAttack, viewAngleAttack, transform, true, layerPlayer)) isAttack = true;
        else isAttack = false;


        if (!isAttack && !isPersuit && !isOcuped && !isBackHome && !lostTarget) isOnPatrol = true;
        else isOnPatrol = false;


    }


    private void FixedUpdate()
    {

        if (lostTarget && !isPersuit && !isAttack && !isBackHome && !answerCall) LookForTarget();

        if (isAttack)
        {
            avoidVectFriends = avoidance() * avoidWeight;
            Attack();
        }

        if (isBackHome && !isAttack && !isPersuit && !isOcuped && !answerCall) BackHome();
     
        
    }

    public void Persuit()
    {
        AnswerCall();
        answerCall = false;   
        lostTarget = true;
        timeOfLook = 10;
        timeToChangePatrol = 0;
        lastTargetPosition = target.transform.position;
        sm.SetState<S_Persuit>();
    }

    public override void Founded()
    {
        pathToTarget.Clear();
        currentIndex = 2;
        cellToPatrol = GetCloseCell(target.position);   
        var myCell = GetCloseCell(transform.position);
        pathToTarget.AddRange(myGridSearcher.Search(myCell, cellToPatrol));
    }

    public void LookForTarget()
    {
        answerCall = false;
        timeOfLook -= Time.deltaTime;
        isOnPatrol = false;
        if (timeOfLook <= 0)
        {
            lostTarget = false;
            isBackHome = true;
        }
        else
            sm.SetState<S_LookForTarget>();

    }

    public void BackHome()
    {
        answerCall = false;
        if (!startback)
        {
            pathToTarget.Clear();
            var myCell = GetCloseCell(transform.position);
            pathToTarget.AddRange(myGridSearcher.Search(myCell, startCell));
            startback = true;
        }
        
        transitableCells.Clear();
        transitableCells.AddRange(GetTransitableCells());
           
        var distance = Vector3.Distance(transform.position, startCell.transform.position);

        if (distance <= 1)
        {
            transform.forward = startRotation;
            isBackHome = false;
            lostTarget = false;
            isOnPatrol = true;
        }
        sm.SetState<S_BackHome>();

    }

    public void Patrol()
    {
        if (!lostTarget)
        {
             
             transitableCells.Clear();
             transitableCells.AddRange(GetTransitableCells());

             timeToChangePatrol -= Time.deltaTime;
             if (timeToChangePatrol <= 0)
             {
                pathToTarget.Clear();
                currentIndex = 0;
                var myCell = GetCloseCell(transform.position);


                cellToPatrol = GetRandomCell();

                if (cellToPatrol == null) cellToPatrol = myCell;    
               
                timeToChangePatrol = UnityEngine.Random.Range(6,10);
                pathToTarget.AddRange(myGridSearcher.Search(myCell, cellToPatrol));
             }

             sm.SetState<S_Patrol>();
        }
      
    }

    public void WaitTurn()
    {
        answerCall = false;
        target.GetComponent<Model>().CombatState();

        bool aux = false;
        foreach (var item in myFriends)  if (item.flankTarget) aux = true;
        
        if (!resting && cm.times > 0 && !timeToAttack && !aux)
        {
            var speed = UnityEngine.Random.Range(0, 1);
            sm.AddState(new S_Waiting(sm, this, this, target, speed));
            timeToAttack = true;
            cm.times--;
            if (cm.times <= 0)
            {
                flankTarget = true;
            }
        }
       
        sm.SetState<S_Waiting>();
    }

    public void Attack()
    {
        answerCall = false;
        AnswerCall();

        if (timeToAttack) dileyToAttack -= Time.deltaTime;
        if (dileyToAttack <= 0)
        {
            rb.AddForce(transform.forward * knockbackForce, ForceMode.Impulse);
            timeToAttack = false;
            StartCoroutine(Resting());
            cm.times++;
            dileyToAttack = UnityEngine.Random.Range(4f, 6f);
            maxDileyToAttack = dileyToAttack;
            flankTarget = false;
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

    public void AnswerCall()
    {

        int friendsOnBatle = 1;
        foreach (var item in myFriends)
        {
            if (item.isPersuit || item.isAttack) friendsOnBatle++;
        }

        if (friendsOnBatle <= myFriends.Count + 1)
        {
            foreach (var item in myFriends)
            {
                if (!item.isPersuit && !item.isAttack)
                {
                    item.Founded();
                    item.answerCall = true;
                }
            }     
        }
    }

    List<Cell> GetTransitableCells()
    {
        transitableCells.AddRange(FindObjectsOfType<Cell>().Where(x => x.transitable).Where(x =>
        {
            var d = Vector3.Distance(x.transform.position, transform.position);
            if (d > 12) return false;
            else return true;

        }));

        return transitableCells;
    }

    Cell GetCloseCell(Vector3 enemy)
    {
        return FindObjectsOfType<Cell>().Where(x =>
        {
            var d = Vector3.Distance(x.transform.position, enemy);
            if (d <1f) return true;
            else return false;

        }).Where(x=> x.transitable).First();      
    }

    Cell GetRandomCell()
    {
      return GetTransitableCells()[UnityEngine.Random.Range(0, transitableCells.Count())];
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

    Vector3 AvoidObstacles()
    {
        var obs = Physics.OverlapSphere(transform.position, 2,layerObst);
        if (obs.Count() > 0)
        {
            var dir = transform.position - obs.First().transform.position;
            return dir.normalized;
        }
        else return Vector3.zero;
    }

    public void Dead()
    {
        gameObject.SetActive(false);
        isDead = true;
    }

    public override IEnumerator FoundTarget(float time)
    {
        throw new NotImplementedException();
    }
}
