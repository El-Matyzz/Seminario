using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public Model model;
    public Viewer view;
    public GameObject text;
    bool smashBool;
    public bool useSword;
    public bool pushW;
    public bool pushS;
    public bool pushA;
    public bool pushD;
    float count;

    public IEnumerator DelaySmash()
    {
        smashBool = true;
        yield return new WaitForSeconds(0.2f);
        smashBool = false;
    }

    // Use this for initialization
    void Awake()
    {
        model.Attack += view.BasicAttack;
        model.OnDamage += view.ReciveDamage;
        model.Estocada += view.Estocada;
        model.RotateAttack += view.GolpeGiratorio;
        model.SaltoyGolpe1 += view.SaltoyGolpe1;
        model.SaltoyGolpe2 += view.SaltoyGolpe2;
        model.Uppercut += view.Uppercut;
        model.Dead += view.Dead;
        model.Trot += view.TrotAnim;
        model.Run += view.RunAnim;
        model.Fall += view.Falling;
    }

    // Update is called once per frame
    void Update()
    {
        

        if (!model.isPlatformJumping)
        {
            if (Input.GetKeyDown(KeyCode.Space) && pushD && !pushW && !pushS) model.StartCoroutine(model.Dash(transform.right));

            if (Input.GetKeyDown(KeyCode.Space) && pushA && !pushW && !pushS) model.StartCoroutine(model.Dash(-transform.right));

            if (Input.GetKeyDown(KeyCode.Space) && pushW && !pushS && !pushD && !pushA) model.StartCoroutine(model.Dash(transform.forward));

            if (Input.GetKeyDown(KeyCode.Space) && pushS && !pushW && !pushD && !pushA) model.StartCoroutine(model.Dash(-transform.forward));

            if (Input.GetKeyDown(KeyCode.Space) && pushW && pushD && !pushS) model.StartCoroutine(model.Dash(transform.forward));

            if (Input.GetKeyDown(KeyCode.Space) && pushW && pushA && !pushS) model.StartCoroutine(model.Dash(transform.forward));

            if (Input.GetKeyDown(KeyCode.Space) && pushS && pushD && !pushW) model.StartCoroutine(model.Dash(-transform.forward));

            if (Input.GetKeyDown(KeyCode.Space) && pushS && pushA && !pushW) model.StartCoroutine(model.Dash(-transform.forward));

            /*  if (Input.GetKeyUp(KeyCode.Alpha1)) model.CastPower1();

              if (Input.GetKeyUp(KeyCode.Alpha2)) model.CastPower2();

              if (Input.GetKeyUp(KeyCode.Alpha3)) model.CastPower3();

              if (Input.GetKeyUp(KeyCode.Alpha4)) model.CastPower4();
              */
            if (Input.GetKey(KeyCode.E) && model.isInCombat)
            {
                model.Defence();
                view.Defence();
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                view.NoDefence();
                model.StopDefence();
            }
            /*if (Input.GetKeyUp(KeyCode.C) && !model.isInCombat)
            {

                model.CombatState();

            }
            */
            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
            {

                if (!model.isRuning) view.FalseTrotAnim();
                view.FalseAnimWalk();
            }
        
            if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            {
                if(!view.anim.GetBool("IdleCombat")) view.anim.SetBool("Idle", true);
                model.acceleration = 0;
                view.FalseAnimRunSword();
                view.FalseRunAnim();
            }

            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.A))
            {
                view.anim.SetBool("Idle", false);
                //view.anim.SetBool("IdleCombat", false);
            }
            if (!Input.GetKey(KeyCode.W)) pushW = false;
            if (!Input.GetKey(KeyCode.S)) pushS = false;
            if (!Input.GetKey(KeyCode.D)) pushD = false;
            if (!Input.GetKey(KeyCode.A)) pushA = false;

            if (Input.GetKey(KeyCode.LeftShift) && model.stamina>5 )
            {
                view.FalseTrotAnim();
                model.isRuning = true;
            }

            if (!Input.GetKey(KeyCode.LeftShift))
            {
                view.FalseRunAnim();
                model.isRuning = false;
            }

            if (Input.GetKeyUp(KeyCode.LeftShift)) model.acceleration = 0;


            if (Input.GetKeyDown(KeyCode.Mouse0) && !smashBool && !model.onAir && model.countAnimAttack<4)
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
            if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !model.isDead)
            {
                if (!pushS) pushW = true;
                if (!pushS && pushW)
                {

                    model.Movement(model.mainCamera.forward, true, false, false);
                    if (model.isInCombat && model.isRuning) view.RunSword();
                }
            }
            if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !model.isDead && !model.isInCombat)
            {
                if (!pushW) pushS = true;
                if (!pushW && pushS)
                    model.Movement(-model.mainCamera.forward, true, false, false);
            }
            if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !model.isDead && model.isInCombat && model.isRuning)
            {
                if (!pushW) pushS = true;
                if (!pushW && pushS)
                {
                    view.RunSword();
                    model.Movement(-model.mainCamera.forward, true, false, false);
                }
            }
            if (Input.GetKey(KeyCode.S) && !model.isDead && model.isInCombat && !model.isRuning)
            {
                if (!pushW) pushS = true;
                if (!pushW && pushS)
                {
                    model.Movement(-model.mainCamera.forward, false, true, false);
                }
            }
            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !model.isDead && !model.isInCombat)
            {
                if (!pushA) pushD = true;
                if (!pushA && pushD)
                    model.Movement(model.mainCamera.right, true, false, false);
            }
            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !model.isDead && model.isInCombat && !model.isRuning)
            {
                if (!pushA) pushD = true;
                if (!pushA && pushD)
                {
                    model.Movement(model.mainCamera.right, false, false, true);
                }
            }

            if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !model.isDead && model.isInCombat && model.isRuning)
            {
                if (!pushA) pushD = true;
                if (!pushA && pushD)
                {
                    view.RunSword();
                    model.Movement(model.mainCamera.right, true, false, false);
                }
            }
            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !model.isDead && !model.isInCombat)
            {

                if (!pushD) pushA = true;
                if (!pushD && pushA)
                    model.Movement(-model.mainCamera.right, true, false, false);
            }

            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !model.isDead && model.isInCombat && !model.isRuning)
            {
                if (!pushD) pushA = true;
                if (!pushD && pushA)
                {
                    model.Movement(-model.transform.right, false, false, true);
                }
            }

            if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !model.isDead && model.isInCombat && model.isRuning)
            {
                if (!pushD) pushA = true;
                if (!pushD && pushA)
                {
                    view.RunSword();
                    model.Movement(-model.mainCamera.right, true, false, false);
                }
            }

            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D) && !model.isDead && !model.isInCombat) model.MovementBizectriz(model.mainCamera.forward, model.mainCamera.right, true, false);

            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A) && !model.isDead && !model.isInCombat) model.MovementBizectriz(model.mainCamera.forward, -model.mainCamera.right, true, false);

            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D) && !model.isDead && !model.isInCombat) model.MovementBizectriz(-model.mainCamera.forward, model.mainCamera.right, true, false);

            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A) && !model.isDead && !model.isInCombat) model.MovementBizectriz(-model.mainCamera.forward, -model.mainCamera.right, true, false);

            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D) && !model.isDead && model.isInCombat)
            {
                model.MovementBizectriz(model.mainCamera.forward, model.mainCamera.right, true, false);
            }

            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D) && !model.isDead && model.isInCombat && model.isRuning)
            {
                view.RunSword();
                model.MovementBizectriz(model.mainCamera.forward, model.mainCamera.right, true, false);
            }

            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A) && !model.isDead && model.isInCombat)
            {
                model.MovementBizectriz(model.mainCamera.forward, -model.mainCamera.right, true, false);
            }

            if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A) && !model.isDead && model.isInCombat && model.isRuning)
            {
                view.RunSword();
                model.MovementBizectriz(model.mainCamera.forward, -model.mainCamera.right, true, false);
            }

            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D) && !model.isDead && model.isInCombat && !model.isRuning)
            {
                model.MovementBizectriz(-model.mainCamera.forward, model.mainCamera.right, true, true);
            }
            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A) && !model.isDead && model.isInCombat && !model.isRuning)
            {
                model.MovementBizectriz(-model.mainCamera.forward, -model.mainCamera.right, true, true);
            }

            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D) && !model.isDead && model.isInCombat && model.isRuning)
            {
                model.MovementBizectriz(-model.mainCamera.forward, model.mainCamera.right, true, false);
            }
            if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A) && !model.isDead && model.isInCombat && model.isRuning)
            {
                model.MovementBizectriz(-model.mainCamera.forward, -model.mainCamera.right, true, false);
            }



        }
    }
}


