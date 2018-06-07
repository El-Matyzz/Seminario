using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    
    public Model model;
    public Viewer view;
    public GameObject text;
    bool smashBool;
    public bool useSword;
    float count;
   
    public IEnumerator DelaySmash()
    {
        smashBool = true;
        yield return new WaitForSeconds(0.2f);
        smashBool = false;
    }
    
    // Use this for initialization
    void Awake() {

        model.Attack += view.BasicAttack;
        model.OnDamage += view.ReciveDamage;
        model.Estocada += view.Estocada;
        model.RotateAttack += view.GolpeGiratorio;
        model.SaltoyGolpe1 += view.SaltoyGolpe1;
        model.SaltoyGolpe2 += view.SaltoyGolpe2;
        model.Uppercut += view.Uppercut;
        model.Dead += view.Dead;
        model.Combat += view.TakeSword;
        model.Safe += view.SaveSword;
    }

    // Update is called once per frame
    void Update () {
        if (!model.isPlatformJumping)
        {
            if (Input.GetKeyUp(KeyCode.Alpha1)) model.CastPower1();

            if (Input.GetKeyUp(KeyCode.Alpha2)) model.CastPower2();

            if (Input.GetKeyUp(KeyCode.Alpha3)) model.CastPower3();

            if (Input.GetKeyUp(KeyCode.Alpha4)) model.CastPower4();

<<<<<<< HEAD
        if (Input.GetKeyUp(KeyCode.C)  && !model.isInCombat)
        {
           
            model.CombatState();
=======
            if (Input.GetKeyUp(KeyCode.C) && !model.isInCombat)
            {
>>>>>>> 0079c04de8a7ebe90e1b53ab941e8ec16b39b99c

                model.StartInCombat();

                if (!useSword) view.TakeSword();

                else if (useSword && !model.isInCombat) view.SaveSword();
            }
            if (Input.GetKey(KeyCode.LeftShift)) model.isRuning = true;

            if (Input.GetKeyUp(KeyCode.LeftShift)) model.isRuning = false;

            if (Input.GetKey(KeyCode.Mouse0) && !smashBool && !model.onAir)
            {
                StartCoroutine(DelaySmash());
                useSword = true;
                model.NormalAttack();
            }

            if (Input.GetKeyDown(KeyCode.E)) model.StartInteraction();

            if (Input.GetKeyDown(KeyCode.J)) StartCoroutine(model.PlatformJump());
        }
    }

    private void FixedUpdate()
    {
        if (!model.isPlatformJumping)
        {
            if (Input.GetKey(KeyCode.W) && !model.isDead) model.Movement(model.mainCamera.forward);

            if (Input.GetKey(KeyCode.S) && !model.isDead) model.Movement(-model.mainCamera.forward);

            if (Input.GetKey(KeyCode.D) && !model.isDead) model.Movement(model.mainCamera.right);

            if (Input.GetKey(KeyCode.A) && !model.isDead) model.Movement(-model.mainCamera.right);
        }
    }
}
