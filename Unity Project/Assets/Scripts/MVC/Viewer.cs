using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewer : MonoBehaviour {

    public Model model;
    public Animator anim;
    bool turn;
    public float animTrotSpeedZ;
    public float animTrotSpeedX;

    public void Update()
    {
         
        if (!model.isRuning)
        {
            animTrotSpeedZ = Input.GetAxis("Vertical")*1.2f;
            animTrotSpeedX = Input.GetAxis("Horizontal")*1.2f;
            
        }
        if(model.isRuning)
        {
            animTrotSpeedZ += Input.GetAxis("Vertical")/10; 
            animTrotSpeedX += Input.GetAxis("Horizontal")/10;
        }
        anim.SetFloat("VelZ", animTrotSpeedZ);
        anim.SetFloat("VelX", animTrotSpeedX);
    }

    public void Estocada()
    {
        anim.Play("Estocada");
    }

    public void GolpeGiratorio()
    {
        anim.Play("GolpeGiratorio");
    }

    public void SaltoyGolpe1()
    {
        anim.Play("SaltoyGolpe1");
    }

    public void SaltoyGolpe2()
    {
        anim.Play("SaltoyGolpe2");
    }

    public void RockThrow()
    {
        anim.Play("RockThrow");
    }

    public void ReciveDamage()
    {
        anim.Play("ReciveDamage");
    }

    public void OpenChest()
    {
        anim.Play("OpenChest");
    }

    public void SacarEspada()
    {
        anim.Play("SacarEspada");
    }

    public void StopAttack()
    {
        anim.Play("StopAttack");
    }

    public void BasicAttack()
    {
        anim.Play("BasicAttack");
    }

    public void UseSword()
    {
        anim.Play("SacarEspada");
    }

    public void Dead()
    {
        anim.Play("Dead");
    }

    public void SwordDead()
    {
        anim.Play("SwordDead");
    }

    public void TakeDamageSword()
    {
        anim.Play("TakeDamageSword");
    }

    public void Attack2()
    {
        anim.Play("Attack2");
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
