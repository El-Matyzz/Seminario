using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyCombatManager : MonoBehaviour {

    public int times;

	// Use this for initialization
	void Start () {

        times = Random.Range(1,3);
     
	}

    public void ResetTimes()
    {
        times--;
        if(times<=0) times = Random.Range(1, 3); 
    }

}
