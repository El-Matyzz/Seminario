﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerEnemyArcher : MonoBehaviour {

    public ModelEnemyArcher _model;

    void Awake()
    {

    }

    void Update()
    {
      if (_model.isAttack) _model.AttackRange();
      if (_model.isAttackMelle) _model.Waiting();
      if (_model.isPersuit) _model.Persuit();
      if (_model.isOnPatrol) _model.Patrol();
    }
}
