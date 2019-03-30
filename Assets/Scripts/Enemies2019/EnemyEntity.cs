using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyEntity: MonoBehaviour
{
    public abstract Vector3 ObstacleAvoidance();
    public Vector3 avoidVectObstacles;
    public bool isPersuit;
    public bool isAttack;
    public bool onAttack;
    public bool isFollow;
    public bool onDamage;
    public Model target;
    public int currentIndex;
    public float speed;
    public float viewDistancePersuit;
    public float angleToPersuit;
    public float viewDistanceAttack;
    public float angleToAttack;
    public abstract Node GetMyNode();
    public abstract Node GetMyTargetNode();
    public abstract Node GetRandomNode();
    public List<Node> pathToTarget = new List<Node>();
    public List<Node> myNodes = new List<Node>();
    public i_EnemyActions currentAction;
    public Rigidbody rb;
    public float dileyToAttack;
    public float knockbackForce;
    public float radiusAttack;
    public float attackDamage;
}
