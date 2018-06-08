using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Viewer : MonoBehaviour {

    public Model model;
    public Controller controller;
    public Animator anim;
    bool turn;
    public float animTrotSpeedZ;
    public float animTrotSpeedX;
    CamShake camShake;

    public Image power1;
    public Image power2;
    public Image power3;
    public Image power4;

    public void Awake()
    {
        camShake = GameObject.Find("FreeLookCameraRig").GetComponentInChildren<CamShake>();
    }

    public void Update()
    {
        if (model.InAction && model.onDamage)
        {
            animTrotSpeedX = 0;
            animTrotSpeedZ = 0;
            
        }
        if (!model.isRuning && !model.isDead && !model.InAction && !model.onDamage)
        {
            animTrotSpeedZ = Input.GetAxis("Vertical")*1.2f;
            animTrotSpeedX = Input.GetAxis("Horizontal")*1.2f;
            if (animTrotSpeedX > 1) animTrotSpeedX = 1;
            if (animTrotSpeedZ > 1) animTrotSpeedZ = 1;
        }
        if(model.isRuning && !model.isDead && !model.InAction && !model.onDamage) 
        {
            animTrotSpeedZ += Input.GetAxis("Vertical")/10; 
            animTrotSpeedX += Input.GetAxis("Horizontal")/10;
        }
        anim.SetFloat("VelZ", animTrotSpeedZ);
        anim.SetFloat("VelX", animTrotSpeedX);
    }

    public void UpdatePowerCD(int id, float fa)
    {
        switch (id)
        {
            case 1:
                power1.fillAmount = fa;
                break;
            case 2:
                power2.fillAmount = fa;
                break;
            case 3:
                power3.fillAmount = fa;
                break;
            case 4:
                power4.fillAmount = fa;
                break;
        }
    }

    public void DesactivateAttack()
    {
        anim.SetBool("attack", false);
    }

    public void DesactivateAttack2()
    {
        anim.SetBool("attack", false);
        model.InAction = false;
        model.InActionAttack = false;
    }

    public void TakeSword()
    {
        anim.SetLayerWeight(1, 1);
        anim.SetBool("SaveSword", false);
        anim.SetBool("TakeSword", true);
    }


    public void SaveSword()
    {
        anim.SetLayerWeight(1, 1);
        anim.SetBool("TakeSword", false);
        anim.SetBool("SaveSword", true);                       
        controller.useSword = false;
    }

    public void DesactivateLayer()
    {
        anim.SetLayerWeight(1, 0);
        anim.SetBool("SaveSword", false);
        anim.SetBool("TakeSword", false);
    }

    public void Estocada()
    {
        anim.SetLayerWeight(1, 0);
        anim.SetBool("EstocadaBool", true);
    }

    public void NoEstocada()
    {
        anim.SetLayerWeight(1, 0);
        anim.SetBool("EstocadaBool", false);
    }

    public void BackEstocada()
    {
        anim.SetLayerWeight(1, 0);
        anim.SetBool("BackEstocada", true);
    }

    public void NoBackEstocada()
    {
    
        anim.SetBool("BackEstocada", false);
    }

    public void GolpeGiratorio()
    {
        anim.SetLayerWeight(1, 0);
        if (!model.mySkills.secondRotate) anim.SetBool("GolpeGiratorio2", true);

        else anim.SetBool("GolpeGiratorio", true);
    }

    public void NoGolpeGiratorio()
    {
        anim.SetLayerWeight(1, 0);
        if (!model.mySkills.secondRotate) anim.SetBool("GolpeGiratorio2", false);

        else anim.SetBool("GolpeGiratorio",false);
    }

  

    public void SaltoyGolpe1()
    {
        anim.SetLayerWeight(1, 0);
        anim.SetBool("JumpAttack", true);
    }

    public void NoSaltoyGolpe1()
    {
        anim.SetLayerWeight(1, 0);
        anim.SetBool("JumpAttack", false);
    }

    public void SaltoyGolpe2()
    {
        anim.SetLayerWeight(1, 0);
        anim.SetBool("JumpAttack2", true);
    }

    public void NoSaltoyGolpe2()
    {
        anim.SetLayerWeight(1, 0);
        anim.SetBool("JumpAttack2", false);
    }

    public void Uppercut()
    {
        anim.SetLayerWeight(1, 0);
        anim.SetBool("Uppercut", true);
    }

    public void FalseUppercut()
    {
        anim.SetLayerWeight(1, 0);
        anim.SetBool("Uppercut", false);
    }

    public void ReciveDamage()
    {
        camShake.ShakeCamera(3.5f, 1);
        anim.SetLayerWeight(1, 0);
        var random = Random.Range(1, 4);
        anim.SetInteger("TakeDamage", random);
    }

    public void NoReciveDamage()
    {
        anim.SetLayerWeight(1, 0);
        anim.SetInteger("TakeDamage", 0);
    }

    public void BasicAttack()
    {
        anim.SetLayerWeight(1, 0);  

        if(model.countAnimAttack==0) anim.SetBool("attack", true);

        if (model.countAnimAttack == 1) BasicAttack2();

        if (model.countAnimAttack >= 2) BasicAttack3();
    }

    public void BasicAttack2()
    {
      
        anim.SetBool("attack", false);
        anim.SetBool("attack2", true);
        StartCoroutine(Triggers1());      
    }

    public void BasicAttack3()
    {
        anim.SetBool("attack2", false);
        anim.SetBool("attack3", true);
        StartCoroutine(Triggers2());       
    }

    IEnumerator Triggers1()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("attack2", false);
    }

    IEnumerator Triggers2()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("attack3", false);
    }
   

    public void Dead()
    {
        anim.SetBool("IsDead", true);
    }

   
}
