using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatArea : MonoBehaviour
{
    public List<GameObject> myNPCs = new List<GameObject>();
    public Grid grid;
    public GameObject player;
    public bool shutDown;

    IEnumerator ShutDown()
    {
        yield return new WaitForSeconds(0.5f);
        if (shutDown)
        {
            foreach (var item in myNPCs) item.SetActive(false);
            grid.gameObject.SetActive(false);
        }
    }

    public void Start()
    {
        StartCoroutine(ShutDown());
    }

    public void OnTriggerExit(Collider c)
    {
        if(c.GetComponent<Model>())
        {
            grid.gameObject.SetActive(false);
            foreach (var item in myNPCs)
            {
                item.SetActive(false);
               
            }
        }
    }

    public void OnTriggerEnter(Collider c)
    {

        if (c.GetComponent<Model>())
        {
            grid.gameObject.SetActive(true);
            foreach (var item in myNPCs)
            {
                if (!item.GetComponent<EnemyClass>().isDead) item.SetActive(true);

            }
        }
    }
}
