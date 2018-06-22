using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerEnemyArcher : MonoBehaviour {

    public ModelEnemyArcher _model;

    void Awake()
    {

    }

    void Update()
    {
      if (_model.isAttack) _model.Attack();
      if (_model.isFollow) _model.Follow();
      if (_model.startScape) _model.Scape();
    }
}
