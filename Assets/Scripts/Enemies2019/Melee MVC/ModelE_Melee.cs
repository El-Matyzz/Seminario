using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ModelE_Melee : EnemyEntity
{

    public enum EnemyInputs { PATROL, PERSUIT, WAIT, ATTACK, FOLLOW, DIE, ANSWER }
    private EventFSM<EnemyInputs> _myFsm;
    public float timeToPatrol;
    public LayerMask layerPlayer;
    public LayerMask layerObst;
    public LayerMask layerEntites;
    public LayerMask layerAttack;
    public bool timeToAttack;
    public bool flank;
    public EnemyCombatManager cm;
    public List<ModelE_Melee> myWarriorFriends = new List<ModelE_Melee>();
    public Transform attackPivot;
    public Vector3 warriorVectAvoidance;
    ViewerE_Melee _view;
    public float distanceToHit;
    public float angleToHit;
    public bool onAttackArea;

    public Action TakeDamageEvent;
    public Action DeadEvent;
    public Action AttackEvent;
    public Action MoveEvent;
    public Action IdleEvent;

    public IEnumerator Resting()
    {
       if (myWarriorFriends.Count < 0) timeToAttack = false;
       onAttack = true;
       yield return new WaitForSeconds(2);
       timeToAttack = false;
       onAttack = false;
    }
 
    public void Awake()
    {
        dileyToAttack = UnityEngine.Random.Range(3f, 5f);
        rb = gameObject.GetComponent<Rigidbody>();
        _view = GetComponent<ViewerE_Melee>();

        TakeDamageEvent += _view.TakeDamageAnim;
        DeadEvent += _view.DeadAnim;
        AttackEvent += _view.AttackAnim;
        IdleEvent += _view.IdleAnim;
        MoveEvent += _view.BackFromIdle;    


        var patrol = new FSM_State<EnemyInputs>("PATROL");
        var persuit = new FSM_State<EnemyInputs>("PERSUIT");
        var wait = new FSM_State<EnemyInputs>("WAIT");
        var attack = new FSM_State<EnemyInputs>("ATTACK");
        var die = new FSM_State<EnemyInputs>("DIE");
        var follow = new FSM_State<EnemyInputs>("FOLLOW");
        var answerCall = new FSM_State<EnemyInputs>("ANSWER");

        StateConfigurer.Create(patrol)
           .SetTransition(EnemyInputs.PERSUIT, persuit)
           .SetTransition(EnemyInputs.ANSWER, answerCall)
           .SetTransition(EnemyInputs.WAIT, wait)
           .SetTransition(EnemyInputs.DIE, die)
           .Done();

        StateConfigurer.Create(persuit)
         .SetTransition(EnemyInputs.WAIT, wait)
         .SetTransition(EnemyInputs.FOLLOW, follow)
         .SetTransition(EnemyInputs.DIE, die)
         .Done();

        StateConfigurer.Create(wait)
         .SetTransition(EnemyInputs.PERSUIT, persuit)
         .SetTransition(EnemyInputs.FOLLOW, follow)
         .SetTransition(EnemyInputs.ATTACK, attack)
         .SetTransition(EnemyInputs.DIE, die)
         .Done();

         StateConfigurer.Create(attack)
        .SetTransition(EnemyInputs.PERSUIT, persuit)
        .SetTransition(EnemyInputs.FOLLOW, follow)
        .SetTransition(EnemyInputs.ATTACK, attack)
        .SetTransition(EnemyInputs.DIE, die)
        .Done();

        StateConfigurer.Create(follow)
         .SetTransition(EnemyInputs.PERSUIT, persuit)
         .SetTransition(EnemyInputs.WAIT, wait)
         .SetTransition(EnemyInputs.DIE, die)
         .Done();

        StateConfigurer.Create(answerCall)
        .SetTransition(EnemyInputs.PERSUIT, persuit)
        .SetTransition(EnemyInputs.WAIT, wait)
        .SetTransition(EnemyInputs.DIE, die)
        .Done();

        StateConfigurer.Create(die).Done();

        patrol.OnFixedUpdate += () =>
        {
            timeToPatrol -= Time.deltaTime;
            currentAction = new A_Patrol(this);

            if(!isDead && isPersuit && !isAttack) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isDead && isAnswerCall) SendInputToFSM(EnemyInputs.ANSWER);

            if (!isDead && isAttack) SendInputToFSM(EnemyInputs.WAIT);

            if (isDead) SendInputToFSM(EnemyInputs.DIE);

        };

        patrol.OnUpdate += () =>
        {
            timeToPatrol -= Time.deltaTime;
        };

     
        answerCall.OnFixedUpdate += () =>
        {

            currentAction = new A_FollowTarget(this);
            MoveEvent();
            if (!isDead && isPersuit) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isDead && isAttack) SendInputToFSM(EnemyInputs.WAIT);
        };

     
        persuit.OnFixedUpdate += () =>
        {
            isAnswerCall = false;

            if (!onDamage) MoveEvent();

            foreach (var item in nearEntities) if (!item.isAnswerCall) item.isAnswerCall = true;

            currentAction = new A_Persuit(this);

            if(!isDead && isAttack) SendInputToFSM(EnemyInputs.WAIT);

            if (!isDead && !isAttack && !isPersuit) SendInputToFSM(EnemyInputs.FOLLOW);

            if (isDead) SendInputToFSM(EnemyInputs.DIE);
        };

        wait.OnUpdate += () => 
        {
            isAnswerCall = false;

            Debug.Log("wait");

            currentAction = new A_WarriorWait(this);

            if (!isDead && !isAttack && isPersuit) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isDead && !isAttack && !isPersuit) SendInputToFSM(EnemyInputs.FOLLOW);

            if (!isDead && dileyToAttack <= 0) SendInputToFSM(EnemyInputs.ATTACK);

            if (isDead) SendInputToFSM(EnemyInputs.DIE);
        };
      
        attack.OnFixedUpdate += () =>
        {
            Debug.Log("attack");
            currentAction = new A_AttackMeleeWarrior(this);

            if (!isDead && !isAttack && isPersuit && dileyToAttack > 0 && !onAttack) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isDead && !isAttack && !isPersuit && dileyToAttack > 0 && !onAttack) SendInputToFSM(EnemyInputs.FOLLOW);

            if (!isDead && dileyToAttack>0) SendInputToFSM(EnemyInputs.WAIT);

            if (isDead) SendInputToFSM(EnemyInputs.DIE);

        };

        follow.OnEnter += x =>
        {
            Node start = GetMyNode();
            Node end = GetMyTargetNode();

            pathToTarget = MyBFS.GetPath(start, end, myNodes);
            currentIndex = pathToTarget.Count;
        };
      

        follow.OnUpdate += () =>
        {
            currentAction = new A_FollowTarget(this);

            if (!onDamage) MoveEvent();

            if (!isDead && !isAttack && isPersuit) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isDead && isAttack) SendInputToFSM(EnemyInputs.WAIT);
        };

        die.OnEnter += x =>
        {
            DeadEvent();
            currentAction = null;
            foreach (var item in myWarriorFriends) item.myWarriorFriends.Remove(this);
            foreach (var item in nearEntities) item.nearEntities.Remove(this);
        };

        _myFsm = new EventFSM<EnemyInputs>(patrol);

    }

    private void SendInputToFSM(EnemyInputs inp)
    {
        _myFsm.SendInput(inp);
    }

    private void Update()
    {
        _myFsm.Update();

        FillFriends();

        warriorVectAvoidance = WarriorAvoidance();
        avoidVectObstacles = ObstacleAvoidance();
        entitiesAvoidVect = EntitiesAvoidance();

        if (dileyToAttack > 0 && !isAttack && !onAttackArea && SearchForTarget.SearchTarget(target.transform, viewDistancePersuit, angleToPersuit, transform, true, layerObst)) isPersuit = true;
        else isPersuit = false;

        if (dileyToAttack>0 && !onAttackArea && SearchForTarget.SearchTarget(target.transform, viewDistanceAttack, angleToAttack, transform, true, layerObst)) isAttack = true;
        else isAttack = false;

        if (SearchForTarget.SearchTarget(target.transform, distanceToHit, angleToHit , transform, true, layerObst)) onAttackArea = true;
        else onAttackArea = false;

    }

    private void FixedUpdate()
    {
        _myFsm.FixedUpdate();
        if (currentAction != null) currentAction.Actions();
    }

    public override Node GetMyNode()
    {
        var myNode = myNodes.OrderBy(x =>
        {
            var d = Vector3.Distance(x.transform.position, transform.position);
            return d;
        }).First();

        return myNode;
    }

    public override Node GetMyTargetNode()
    {
        var targetNode = myNodes.OrderBy(x =>
        {
            var d = Vector3.Distance(x.transform.position, target.transform.position);
            return d;
   
        }).First();

        return targetNode;
    }

    public override Node GetRandomNode()
    {
        var r = UnityEngine.Random.Range(0, myNodes.Count);
        return myNodes[r];
    }


    public override Vector3 ObstacleAvoidance()
    {     
       var obs = Physics.OverlapSphere(transform.position, 2, layerObst);
       if (obs.Count() > 0)
       {
           var dir = transform.position - obs.First().transform.position;
           return dir.normalized;
       }
       else return Vector3.zero;       
    }

    public void FillFriends()
    {
        myWarriorFriends.Clear();
        myWarriorFriends.AddRange(Physics.OverlapSphere(transform.position, viewDistancePersuit * 2)
                                        .Where(x => x.GetComponent<ModelE_Melee>())
                                        .Select(x => x.GetComponent<ModelE_Melee>()));
    } 

    void OnDrawGizmos()
    {
       Gizmos.color = Color.blue;
       Gizmos.DrawWireSphere(transform.position, viewDistancePersuit);

        Vector3 rightLimit = Quaternion.AngleAxis(angleToPersuit, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit * viewDistancePersuit));

        Vector3 leftLimit = Quaternion.AngleAxis(-angleToPersuit, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit * viewDistancePersuit));

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distanceToHit);

        Vector3 rightLimit3 = Quaternion.AngleAxis(angleToHit, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit3 * distanceToHit));

        Vector3 leftLimit3 = Quaternion.AngleAxis(-angleToHit, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit3 * distanceToHit));

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistanceAttack);

        Vector3 rightLimit2 = Quaternion.AngleAxis(angleToAttack, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit2 * viewDistanceAttack));

        Vector3 leftLimit2 = Quaternion.AngleAxis(-angleToAttack, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit2 * viewDistanceAttack));

        Gizmos.DrawWireSphere(attackPivot.position, radiusAttack);
    }

    public override Vector3 EntitiesAvoidance()
    {
        var obs = Physics.OverlapSphere(transform.position, 2, layerEntites);
        if (obs.Count() > 0)
        {
            var dir = transform.position - obs.First().transform.position;
            return dir.normalized;
        }
        else return Vector3.zero;
    }

    public  Vector3 WarriorAvoidance()
    {
        var obs = Physics.OverlapSphere(transform.position, 2, layerEntites).Where(x=> x.GetComponent<ModelE_Melee>()).Select(x=> x.GetComponent<ModelE_Melee>()).Where(x=> !x.flank);
        if (obs.Count() > 0)
        {
            var dir = transform.position - obs.First().transform.position;
            return dir.normalized;
        }
        else return Vector3.zero;
    }

    public override void GetDamage(float damage)
    {
        isAttack = true;
        dileyToAttack += 0.25f;
        if (dileyToAttack >= maxDileyToAttack) dileyToAttack = maxDileyToAttack;
        TakeDamageEvent();      
        life -= damage;
        if (life <= 0) isDead = true;
    }

    public override void MakeDamage()
    {
        var player = Physics.OverlapSphere(attackPivot.position, radiusAttack).Where(x => x.GetComponent<Model>()).Select(x => x.GetComponent<Model>()).FirstOrDefault();
        if (player != null)
        {
            player.GetDamage(attackDamage, transform, false);
        }
    }

    public override void RetreatTrue()
    {
        throw new NotImplementedException();
    }

    public override void OnDamageFalse()
    {
        throw new NotImplementedException();
    }
}
