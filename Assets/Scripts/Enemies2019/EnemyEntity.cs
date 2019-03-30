﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyEntity: MonoBehaviour
{
    public float life;
    public float totalLife;
    public abstract Vector3 ObstacleAvoidance();
    public abstract Vector3 EntitiesAvoidance();
    public Vector3 avoidVectObstacles;
    public Vector3 entitiesAvoidVect;
    public bool isPersuit;
    public bool isAttack;
    public bool onAttack;
    public bool isFollow;
    public bool isAnswerCall;
    public bool isDead;
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
    public List<EnemyEntity> nearEntities = new List<EnemyEntity>();
    public List<Node> pathToTarget = new List<Node>();
    public abstract void GetDamage(float damage);
    public List<Node> myNodes = new List<Node>();
    public i_EnemyActions currentAction;
    public Rigidbody rb;
    public float dileyToAttack;
    public float maxDileyToAttack;
    public float knockbackForce;
    public float radiusAttack;
    public float attackDamage;
}
