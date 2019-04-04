﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Viewer : MonoBehaviour {

    public Model model;
    public Controller controller;
    public Animator anim;
    bool turn;
    bool melleCombo1;
    bool melleCombo2;
    bool melleCombo3;
    CamShake camShake;

    public Image power1;
    public Image power2;
    public Image power3;
    public Image power4;

    public GameObject youDied;
    public GameObject youWin;
    public GameObject phParticles;
    public GameObject trail;

    public Image lifeBar;
    public Image staminaBar;
    public Image manaBar;
    public Image armor;

    public Text[] potions;
    public Text potionTimer;

    public List<GameObject> particlesSowrd;

    public int currentAttackAnimation;

    public IEnumerator DestroyParticles(GameObject p)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(p);
    }

    public IEnumerator SaveSwordAnim()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetLayerWeight(1, 0);
    }

    public void Update()
    {
        

        var velocityX = Input.GetAxis("Vertical") ;
        var velocityZ = Input.GetAxis("Horizontal") ;

        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D) && !model.isDead && model.isInCombat) velocityZ = 0;
   
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A) && !model.isDead && model.isInCombat) velocityZ = 0;

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D) && !model.isDead && model.isInCombat) velocityZ = 0;

        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A) && !model.isDead && model.isInCombat) velocityZ = 0;


        if (velocityX >1) velocityX = 1;
        if (velocityZ >1) velocityZ = 1;

        anim.SetFloat("VelX", velocityX);
        anim.SetFloat("VelZ", velocityZ);

       
    }

    public void Awake()
    {
        trail.SetActive(false);
        anim.SetLayerWeight(1, 0);
        camShake = GameObject.Find("FreeLookCameraRig").GetComponentInChildren<CamShake>();
    
    }

    public void RollAnim()
    {
        anim.SetBool("Roll", true);
    }

    public void BackRollAnim()
    {
        anim.SetBool("Roll", false);
    }

    public void Blocked()
    {
        anim.Play("Blocked");
    }

    public void AwakeTrail()
    {
        trail.SetActive(true);
    } 

    public void SleepTrail()
    {
        trail.SetActive(false);
    }

    public void RunSword()
    {
        anim.SetBool("runSword", true);
    }

    public void TrotAnim()
    {
        anim.SetBool("trotAnim", true);
    }

    public void FalseTrotAnim()
    {
        anim.SetBool("trotAnim", false);
    }

    public void RunAnim()
    {      
        anim.SetBool("runAnim", true);
        anim.SetBool("WalkW", false);
        anim.SetBool("WalkS", false);
        anim.SetBool("WalkD", false);
        anim.SetBool("WalkA", false);
    }

    public void FalseRunAnim()
    {
        
        anim.SetBool("runAnim", false);
    }

    public void FalseAnimRunSword()
    {
        anim.SetBool("runSword", false);
    }
  

    public void FalseAnimWalk()
    {
        anim.SetBool("runSword", false);
        anim.SetBool("WalkW", false);
        anim.SetBool("WalkS", false);
        anim.SetBool("WalkD", false);
        anim.SetBool("WalkA", false);
        
    }
    public void UpdateLifeBar(float val)
    {
        StartCoroutine(BarSmooth(val));
    }

    public void UpdateStaminaBar(float val)
    {
        staminaBar.fillAmount = val;
    }

    public void UpdateManaBar(float val)
    {
        manaBar.fillAmount = val;
    }

    public void UpdateArmorBar(float val)
    {
        armor.fillAmount = val;
    }

    public IEnumerator BarSmooth(float target)
    {
        bool timerRunning = true;
        float smoothTimer = 0;

        float current = lifeBar.fillAmount;

        if (current - target <= 0.025f)
            lifeBar.fillAmount = target;

        while (timerRunning)
        {
            smoothTimer += Time.deltaTime * 1.5f;
            lifeBar.fillAmount = Mathf.Lerp(current, target, smoothTimer);
            if (smoothTimer > 1)
                timerRunning = false;
            yield return new WaitForEndOfFrame();
        }
    }

    public void UpdatePotions(int i)
    {
        potions[i].text = "x" + model.potions[i];
        if (model.potions[i] == 0)
            potions[i].transform.parent.gameObject.SetActive(false);
        else
            potions[i].transform.parent.gameObject.SetActive(true);
    }

    public void UpdateTimer(string val = "")
    {
        potionTimer.text = val.ToString();
    }

    public IEnumerator SlowSpeed()
    {
        anim.speed = 0f;
        yield return new WaitForSeconds(0.025f);
        anim.speed = 1;
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

    public void Falling()
    {
        anim.SetBool("FallBool", true);
    }

    public void FalseFall()
    {
        anim.SetBool("FallBool", false);
    }

    public void DesactivateLayer()
    {
        //  anim.SetLayerWeight(1, 0);
        anim.SetBool("SaveSword", false);
        anim.SetBool("TakeSword", false);
    }

    public void Estocada()
    {
        //  anim.SetLayerWeight(1, 0);
        anim.SetBool("EstocadaBool", true);
    }

    public void NoEstocada()
    {
        //  anim.SetLayerWeight(1, 0);
        anim.SetBool("EstocadaBool", false);
    }

    public void BackEstocada()
    {
        //  anim.SetLayerWeight(1, 0);
        anim.SetBool("BackEstocada", true);
    }

    public void NoBackEstocada()
    {    
        anim.SetBool("BackEstocada", false);
    }

    public void GolpeGiratorio()
    {
        // anim.SetLayerWeight(1, 0);
        if (!model.mySkills.secondRotate) anim.SetBool("GolpeGiratorio2", true);

        else anim.SetBool("GolpeGiratorio", true);
    }

    public void NoGolpeGiratorio()
    {
        //  anim.SetLayerWeight(1, 0);
        if (!model.mySkills.secondRotate) anim.SetBool("GolpeGiratorio2", false);

        else anim.SetBool("GolpeGiratorio",false);
    }

    public void Defence()
    {
        anim.SetBool("Defence", true);
    }

    public void NoDefence()
    {
        anim.SetBool("Defence", false);
    }

    public void SaltoyGolpe1()
    {
        //  anim.SetLayerWeight(1, 0);
        anim.SetBool("JumpAttack", true);
    }

    public void NoSaltoyGolpe1()
    {
        // anim.SetLayerWeight(1, 0);
        anim.SetBool("JumpAttack", false);
    }

    public void SaltoyGolpe2()
    {
        // anim.SetLayerWeight(1, 0);
        anim.SetBool("JumpAttack2", true);
    }

    public void NoSaltoyGolpe2()
    {
        // anim.SetLayerWeight(1, 0);
        anim.SetBool("JumpAttack2", false);
    }

    public void Uppercut()
    {
        // anim.SetLayerWeight(1, 0);
        anim.SetBool("Uppercut", true);
    }

    public void FalseUppercut()
    {
        //  anim.SetLayerWeight(1, 0);
        anim.SetBool("Uppercut", false);
    }

    public void ReciveDamage()
    {
        if (!model.onPowerState)
        {
            camShake.ShakeCamera(3.5f, 1);
            //  anim.SetLayerWeight(1, 0);
            var random = Random.Range(1, 4);
            anim.SetInteger("TakeDamage", random);
        }
    }

    public void NoReciveDamage()
    {
        //  anim.SetLayerWeight(1, 0);
        anim.SetInteger("TakeDamage", 0);
    }

    public void BasicAttack()
    {
        currentAttackAnimation ++;
        Mathf.Clamp(currentAttackAnimation, 0, 4);
        anim.SetInteger("AttackAnim", currentAttackAnimation);
    }
     

    public void Dead()
    {
        anim.SetBool("IsDead", true);
    }

    public IEnumerator YouDied()
    {
        yield return new WaitForSeconds(0.5f);
        youDied.gameObject.SetActive(true);
        var tempColor = youDied.GetComponent<Image>().color;
        var alpha = 0f;
        while (alpha <= 1)
        {
            alpha += 0.5f * Time.deltaTime;
            tempColor.a = alpha;
            youDied.GetComponent<Image>().color = tempColor;
            if (alpha >= 1)
            {
                SceneManager.LoadScene(2);
                /*
                camController.blockMouse = false;
                for (int i = 0; i < youDied.transform.childCount; i++)
                    youDied.transform.GetChild(i).gameObject.SetActive(true);
                Time.timeScale = 0;
                */
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator YouWin()
    {
        youWin.gameObject.SetActive(true);
        var tempColor = youWin.GetComponent<Image>().color;
        var alpha = 0f;
        while (alpha <= 1)
        {
            alpha += 0.75f * Time.deltaTime;
            tempColor.a = alpha;
            youWin.GetComponent<Image>().color = tempColor;
            if (alpha >= 1)
                SceneManager.LoadScene(3);
            yield return new WaitForEndOfFrame();
        }
    }
}
