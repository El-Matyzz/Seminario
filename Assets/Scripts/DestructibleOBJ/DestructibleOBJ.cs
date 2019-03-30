using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleOBJ : MonoBehaviour
{
    public GameObject principalMesh;
    public GameObject destructibleMesh;
    public BoxCollider myBox;
    public Animator anim;
    BoxCollider col;
    Material mat;
    float time;
    bool first;
    bool change;
    public Rigidbody rb;


    public IEnumerator Destroy()
    {
        yield return new WaitForSeconds(5);
        Destroy();
    }

    public IEnumerator startDisolve()
    {
        if (!first)
        {
           
            first = true;
            principalMesh.SetActive(false);
            destructibleMesh.SetActive(true);
            anim.SetBool("IsHit", true);
            myBox.isTrigger = true;
            yield return new WaitForSeconds(5);
            col.isTrigger = true;
            StartCoroutine(Destroy());
        }
    }

    public void Start()
    {
        rb = destructibleMesh.GetComponent<Rigidbody>();
        anim = destructibleMesh.GetComponent<Animator>();
        col = destructibleMesh.GetComponent<BoxCollider>();
        mat = principalMesh.GetComponent<MeshRenderer>().materials[0];
        myBox = GetComponent<BoxCollider>();
    }

    public void Update()
    {
        if (!change)
        {
            time -= Time.deltaTime;
            if (time <= 0) change = true;
        }
        else
        {
            time += Time.deltaTime;
            if (time >= 1) change = false;
        }
        
        mat.SetFloat("_Opacity", time);
    }


}
