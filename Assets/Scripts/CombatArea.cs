using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatArea : MonoBehaviour
{
    public List<EnemyEntity> myNPCs = new List<EnemyEntity>();
    public List<GameObject> walls = new List<GameObject>();
    public int myEntities;
    bool aux;
    public bool startArea;

    private void Awake()
    {
       
    }

    void Start()
    {
        myEntities = myNPCs.Count;
        if (startArea == true) foreach (var item in walls) item.SetActive(false);
    }

    void Update()
    {
        if (myEntities <= 0 && !aux)
        {
            foreach (var item in walls) item.SetActive(false);
            aux = true;
        }
    }

    public void OnTriggerEnter(Collider c)
    {
        foreach (var item in myNPCs) item.target = FindObjectOfType<Model>();
        foreach (var item in walls)
        {
          if(myEntities>0) item.SetActive(true);
        }
    }
}
