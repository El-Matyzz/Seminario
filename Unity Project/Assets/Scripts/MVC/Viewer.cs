using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewer : MonoBehaviour {

    public Model model;
    public Animator anim;

    public void Idel()
    {
        anim.Play("Idel");
    }

    public void Trot()
    {
        anim.Play("Trot");
    }

    public void Run()
    {
        anim.Play("Run");
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
