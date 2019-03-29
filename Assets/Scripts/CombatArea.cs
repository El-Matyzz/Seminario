using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatArea : MonoBehaviour
{
    public List<GameObject> myNPCs = new List<GameObject>();
    public GameObject player;

    IEnumerator ShutDown()
    {
        yield return new WaitForSeconds(0.5f);
        foreach (var item in myNPCs) item.SetActive(false);
    }

    public void Start()
    {
        StartCoroutine(ShutDown());
    }

    public void OnTriggerExit(Collider c)
    {
        if(c.GetComponent<Model>())
        {
            foreach (var item in myNPCs) item.SetActive(false);
        }
    }

    public void OnTriggerEnter(Collider c)
    {

        if (c.GetComponent<Model>())
        {
            Debug.Log("asd");
            foreach (var item in myNPCs) item.SetActive(true);
        }
    }
}
