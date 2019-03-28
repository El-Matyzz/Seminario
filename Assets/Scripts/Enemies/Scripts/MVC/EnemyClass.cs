﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyClass : MonoBehaviour
{

    public abstract void GetDamage(float damage);
    public abstract IEnumerator Stuned(float stunedTimed);
    public abstract IEnumerator Knocked(float knockedTime);
    public abstract IEnumerator Bleeding(float bleedingTime);
    public abstract IEnumerator FoundTarget(float time);
    public abstract void Founded();
    public bool createAttack;
    public bool isAttack;
    public bool myTimeToAttack;
    public bool isBackHome;
    public bool IsOnSearching;
    public bool foundTarget;
    public bool answerCall;
    public float dileyToAttack;
    public float bleedingDamage;
    public float life;
    public Rigidbody rb;
    public Transform target;
    public List<ModelEnemy> myFriends = new List<ModelEnemy>();
    public EnemyScreenSpace ess;
    public int currentIndex = 0;
    public List<Cell> pathToTarget = new List<Cell>();
    public Cell cellToPatrol;
    public Cell startCell;
    public GridSearcher myGridSearcher;
    public Grid myGrid;
    public Vector3 avoidVectFriends;
    public Vector3 avoidVectObstacles;
    public Vector3 startRotation;
    public Vector3 lastTargetPosition;

}
