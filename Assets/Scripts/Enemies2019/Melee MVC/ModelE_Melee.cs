using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ModelE_Melee : EnemyEntity
{

    public enum EnemyInputs { PATROL, PERSUIT, ATTACK, FOLLOW, DIE }
    private EventFSM<EnemyInputs> _myFsm;
    public float timeToPatrol;
    public LayerMask layerPlayer;
    public LayerMask layerObst;
    public bool timeToAttack;
    public EnemyCombatManager cm;
    public List<ModelE_Melee> myWarriorFriends = new List<ModelE_Melee>();
    public Transform attackPivot;

    public IEnumerator Resting()
    {
       if (myWarriorFriends.Count < 0) timeToAttack = false;
       onAttack = true;
       yield return new WaitForSeconds(1);
       timeToAttack = false;
       onAttack = false;
    }

    public void Awake()
    {
        dileyToAttack = UnityEngine.Random.Range(3f, 5f);
        rb = gameObject.GetComponent<Rigidbody>();


        var patrol = new FSM_State<EnemyInputs>("PATROL");
        var persuit = new FSM_State<EnemyInputs>("PERSUIT");
        var attack = new FSM_State<EnemyInputs>("ATTACK");
        var die = new FSM_State<EnemyInputs>("DIE");
        var follow = new FSM_State<EnemyInputs>("FOLLOW");

        StateConfigurer.Create(patrol)
           .SetTransition(EnemyInputs.PERSUIT, persuit)          
           .SetTransition(EnemyInputs.DIE, die)
           .Done();

        StateConfigurer.Create(persuit)
         .SetTransition(EnemyInputs.ATTACK, attack)
         .SetTransition(EnemyInputs.FOLLOW, follow)
         .SetTransition(EnemyInputs.DIE, die)
         .Done();

        StateConfigurer.Create(attack)
         .SetTransition(EnemyInputs.PERSUIT, persuit)
         .SetTransition(EnemyInputs.FOLLOW, follow)
         .SetTransition(EnemyInputs.DIE, die)
         .Done();

        StateConfigurer.Create(follow)
         .SetTransition(EnemyInputs.PERSUIT, persuit)
         .SetTransition(EnemyInputs.ATTACK, attack)
         .SetTransition(EnemyInputs.DIE, die)
         .Done();

        StateConfigurer.Create(die).Done();

        patrol.OnFixedUpdate += () =>
        {
            Debug.Log("patrol");
            timeToPatrol -= Time.deltaTime;

            currentAction = new A_Patrol(this);

            if(isPersuit) SendInputToFSM(EnemyInputs.PERSUIT);

        };

        patrol.OnUpdate += () =>
        {
            timeToPatrol -= Time.deltaTime;
        };

        persuit.OnFixedUpdate += () =>
        {
            Debug.Log("persuit");
            currentAction = new A_Persuit(this);

            if(isAttack) SendInputToFSM(EnemyInputs.ATTACK);

            if (!isAttack && !isPersuit) SendInputToFSM(EnemyInputs.FOLLOW);
        };

        attack.OnUpdate += () => 
        {

            Debug.Log("attack");
            currentAction = new A_AttackMeleeWarrior(this);

            if (!isAttack && isPersuit) SendInputToFSM(EnemyInputs.PERSUIT);

            if (!isAttack && !isPersuit) SendInputToFSM(EnemyInputs.FOLLOW);
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

            Debug.Log("follow");
            if (!isAttack && isPersuit) SendInputToFSM(EnemyInputs.PERSUIT);

            if (isAttack) SendInputToFSM(EnemyInputs.ATTACK);
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

        avoidVectObstacles = ObstacleAvoidance();

        if (!isAttack && SearchForTarget.SearchTarget(target.transform, viewDistancePersuit, angleToPersuit, transform, true, layerObst)) isPersuit = true;
        else isPersuit = false;

        if (SearchForTarget.SearchTarget(target.transform, viewDistanceAttack, angleToAttack, transform, true, layerObst)) isAttack = true;
        else isAttack = false;
        
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

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, viewDistanceAttack);

        Vector3 rightLimit2 = Quaternion.AngleAxis(angleToAttack, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (rightLimit2 * viewDistanceAttack));

        Vector3 leftLimit2 = Quaternion.AngleAxis(-angleToAttack, transform.up) * transform.forward;
        Gizmos.DrawLine(transform.position, transform.position + (leftLimit2 * viewDistanceAttack));

        Gizmos.DrawWireSphere(attackPivot.position, radiusAttack);
    }
}
