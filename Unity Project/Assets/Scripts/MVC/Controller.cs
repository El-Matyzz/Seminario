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
        
        model.Attack += view.BasicAttack;
        model.Estocada += view.Estocada;
        model.RotateAttack += view.GolpeGiratorio;
        model.SaltoyGolpe1 += view.SaltoyGolpe1;
        model.SaltoyGolpe2 += view.SaltoyGolpe2;
    }

    // Update is called once per frame
    void Update () {
		
        if (Input.GetKeyUp(KeyCode.Alpha1)) model.CastPower1();

        if (Input.GetKeyUp(KeyCode.Alpha2)) model.CastPower2();

        if (Input.GetKeyUp(KeyCode.Alpha3)) model.CastPower3();

        if (Input.GetKeyUp(KeyCode.Alpha4)) model.CastPower4();

        if (Input.GetKeyUp(KeyCode.Space)) model.NormalAttack();

        if (Input.GetKeyUp(KeyCode.Alpha5)) view.Attack2();

        if (Input.GetKey(KeyCode.LeftShift)) model.isRuning = true;
     
        if (Input.GetKeyUp(KeyCode.LeftShift)) model.isRuning = false;      

    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W)) model.Movement(model.mainCamera.forward);
      
        if (Input.GetKey(KeyCode.S)) model.Movement(-model.mainCamera.forward);

        if (Input.GetKey(KeyCode.D)) model.Movement(model.mainCamera.right);

        if (Input.GetKey(KeyCode.A)) model.Movement(-model.mainCamera.right);

     
    }
}
