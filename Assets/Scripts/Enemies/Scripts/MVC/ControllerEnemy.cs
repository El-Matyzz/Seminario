using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerEnemy : MonoBehaviour {

    public ModelEnemy _model;

	void Awake () {
		
	}
	
	void Update () {

        if (_model.isPersuit && !_model.isDead) _model.Persuit();

        if (_model.isAttack && !_model.isDead) _model.WaitTurn();

        if (!_model.isAttack && !_model.isDead && !_model.isPersuit && !_model.isBackHome) _model.Patrol();

	}
}
