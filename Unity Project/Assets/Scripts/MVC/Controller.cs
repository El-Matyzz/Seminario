using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    
    public Model model;
    public Viewer view;
    public GameObject text;
    bool aux;

    // Use this for initialization
    void Awake() {

        model.Idle += view.Idel;
        model.Trot += view.Trot;
        model.Run += view.Run;
        model.Estocada += view.Estocada;
        model.GolpeGiratorio += view.GolpeGiratorio;
        model.SaltoyGolpe1 += view.SaltoyGolpe1;
        model.SaltoyGolpe2 += view.SaltoyGolpe2;
    }

    // Update is called once per frame
    void Update () {
		
        if (Input.GetKeyUp(KeyCode.Alpha1)) model.CastPower1();

        if (Input.GetKeyUp(KeyCode.Alpha2)) model.CastPower2();

        if (Input.GetKeyUp(KeyCode.Alpha3)) model.CastPower3();

        if (Input.GetKeyUp(KeyCode.Alpha4)) model.CastPower4();

        if (Input.GetKeyDown(KeyCode.Q)) view.RockThrow();

        if (Input.GetKeyDown(KeyCode.Alpha5)) view.ReciveDamage();

        if (Input.GetKeyDown(KeyCode.Alpha6)) view.OpenChest();

        if (Input.GetKeyDown(KeyCode.Alpha7)) view.SacarEspada();

        if (Input.GetKeyDown(KeyCode.Alpha8)) view.StopAttack();

        if (Input.GetKeyDown(KeyCode.Mouse0)) view.BasicAttack();

        if (Input.GetKeyDown(KeyCode.Mouse1)) view.Attack2();

        if (Input.GetKeyDown(KeyCode.Alpha9)) view.UseSword();

        if (Input.GetKeyDown(KeyCode.F)) view.Dead();

        if (Input.GetKeyDown(KeyCode.G)) view.SwordDead();

        if (Input.GetKeyDown(KeyCode.H)) view.TakeDamageSword();

        if (Input.GetKeyDown(KeyCode.J)) view.Fall();

        if (Input.GetKeyDown(KeyCode.K)) view.Fall2();

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (!aux)
            {
                text.SetActive(true);
                aux = true;
            }

            else
            {
                text.SetActive(false);
                aux = false;
            }

        }

    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W)) model.MoveForward();
      
        if (Input.GetKey(KeyCode.S)) model.MoveBackward();

        if (Input.GetKey(KeyCode.D)) model.MoveRight();

        if (Input.GetKey(KeyCode.A)) model.MoveLeft();

        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A)) model.AnimIdel();

        if (Input.GetKey(KeyCode.LeftShift)) model.isRuning = true;
        if (Input.GetKeyUp(KeyCode.LeftShift)) model.isRuning = false;

    }
}
