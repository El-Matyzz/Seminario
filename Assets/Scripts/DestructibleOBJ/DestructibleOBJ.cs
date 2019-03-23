using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleOBJ : MonoBehaviour
{
    public GameObject principalMesh;
    public GameObject destructibleMesh;
    public Animator anim;
    BoxCollider col;
    bool first;
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
    }


}
