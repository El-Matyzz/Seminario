using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewer : MonoBehaviour {

    public Model model;
    public Controller controller;
    public Animator anim;
    bool turn;
    public float animTrotSpeedZ;
    public float animTrotSpeedX;

    

    public void Update()
    {
         
        if (!model.isRuning && !model.isDead)
        {
            animTrotSpeedZ = Input.GetAxis("Vertical")*1.2f;
            animTrotSpeedX = Input.GetAxis("Horizontal")*1.2f;
            if (animTrotSpeedX > 1) animTrotSpeedX = 1;
            if (animTrotSpeedZ > 1) animTrotSpeedZ = 1;
        }
        if(model.isRuning && !model.isDead) 
        {
            animTrotSpeedZ += Input.GetAxis("Vertical")/10; 
            animTrotSpeedX += Input.GetAxis("Horizontal")/10;
        }
        anim.SetFloat("VelZ", animTrotSpeedZ);
        anim.SetFloat("VelX", animTrotSpeedX);
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
        anim.SetBool("TakeSword", true);
    }

    public void FalseTakeSword()
    {
        anim.SetBool("TakeSword", false);
        controller.useSword = true;
    }

    public void SaveSword()
    {
        anim.SetBool("SaveSword",true);
        controller.useSword = false;
    }

    public void FalseSaveSword()
    {
        anim.SetBool("SaveSword", false);
    }

    public void Estocada()
    {    
     anim.SetBool("EstocadaBool", true);
    }

    public void NoEstocada()
    {
        anim.SetBool("EstocadaBool", false);
    }

    public void BackEstocada()
    {
        anim.SetBool("BackEstocada", true);
    }

    public void NoBackEstocada()
    {
        anim.SetBool("BackEstocada", false);
    }

    public void GolpeGiratorio()
    {
        anim.SetBool("GolpeGiratorio", true);
    }

    public void NoGolpeGiratorio()
    {
        if (!model.mySkills.secondRotate)
            anim.Play("GolpeGiratorio2");
        else anim.SetBool("GolpeGiratorio",false);
    }

    public void NoGolpeGiratorio2()
    {      
      anim.SetBool("GolpeGiratorio", false);
    } 
      
    public void NoSecondGolpeGiratorio()
    {
        if (!model.mySkills.secondRotate)
            anim.SetBool("GolpeGiratorio", false);
    }

    public void SaltoyGolpe1()
    {
        anim.SetBool("JumpAttack", true);
    }

    public void NoSaltoyGolpe1()
    {
        anim.SetBool("JumpAttack", false);
    }

    public void SaltoyGolpe2()
    {
        anim.SetBool("JumpAttack2", true);
    }

    public void NoSaltoyGolpe2()
    {
        anim.SetBool("JumpAttack2", false);
    }

    public void RockThrow()
    {
        anim.Play("RockThrow");
    }

    public void ReciveDamage()
    {
        anim.SetBool("TakeDamageBool", true);
    }

    public void NoReciveDamage()
    {
        anim.SetBool("TakeDamageBool", false);
    }

    public void OpenChest()
    {
        anim.Play("OpenChest");
    }

    public void StopAttack()
    {
        anim.Play("StopAttack");
    }

    public void BasicAttack()
    {
          
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

    public void SwordDead()
    {
        anim.Play("SwordDead");
    }

    public void TakeDamageSword()
    {
        anim.Play("TakeDamageSword");
    }

    public void Fall()
    {
        anim.Play("Caida");
    }

    public void Fall2()
    {
        anim.Play("Caida2");
    }
}
