using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ViewerEnemy : MonoBehaviour {

    public Light visorLight;
    public float speed;
    public bool change;
    public Action ActiveLightAtack;
    public Action DesactivateLightAttack;

	void Awake ()
    {
        ActiveLightAtack += AttackVisorLight;
        DesactivateLightAttack += DesactivateLigth;
	}
	
	// Update is called once per frame
	void Update () {
       
	}

    public void AttackVisorLight()
    {
        if (!change)
        {
            visorLight.intensity -= speed * Time.deltaTime;
            if (visorLight.intensity <= 0) change = true;
        }

        if (change)
        {
            visorLight.intensity += speed * Time.deltaTime;
            if (visorLight.intensity >= 5f) change = false;
        }
    }

    public void DesactivateLigth()
    {
        visorLight.intensity = 0;
    }
}
