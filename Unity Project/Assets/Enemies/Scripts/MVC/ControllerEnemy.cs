using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerEnemy : MonoBehaviour {

    public ModelEnemy _model;

	void Awake () {
		
	}
	
	void Update () {

        if (_model.isFollow) _model.Follow();

        if (_model.isAttack) _model.Attack();

	}
}
