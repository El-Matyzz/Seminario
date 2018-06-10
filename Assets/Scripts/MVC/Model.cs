﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Model : MonoBehaviour {

    public Transform attackPivot;
    public Viewer view;

    public PowerManager powerManager;
    public Powers prefabPower;
    public Pool<Powers> powerPool;

    public float radiusAttack;
    public float extraFireDamage;
    public float extraSlameDamage;
    public float skillPoints;
    public float life;
    public float totalLife;
    public float speed;
    public float runSpeed;
    public float timeOnCombat;

    public int countAnimAttack;
    public Collider enemy;  

    public int stocadaAmount;

    public Skills mySkills;
    public bool isIdle;
    public bool onAir;
    public bool stocadaState;
    public bool jumpAttackWarriorState;
    public bool chargeTankeState;
    public bool isRuning;
    public bool isAnimatedMove;
    public bool isInCombat;
    public bool isDead;

    bool cdPower1;
    bool cdPower2;
    bool cdPower3;
    bool cdPower4;
    public bool InAction;
    public bool InActionAttack;
    bool WraperInAction;

    public bool onDamage;

    public Transform mainCamera;
    public Vector3 dir;
    public Rigidbody rb;
    public EnemyClass currentEnemy;

    Platform currentPlatform;
    public bool isPlatformJumping;

    List<bool> cdList = new List<bool>();
    
    public  Action Estocada;
    public event Action Attack;
    public event Action RotateAttack;
    public Action SaltoyGolpe1;
    public Action SaltoyGolpe2;
    public Action Uppercut;
    public Action OnDamage;
    public event Action Combat;
    public event Action Safe;
    public Action Dead;


    public IEnumerator PowerColdown(float cdTime, int n)
    {
        float t1;
        float t2;
        float t3;
        float t4;

        for (t1 = 0; t1 < cdTime && n == 1; t1 += Time.deltaTime)
        {
            cdPower1 = true;
            view.UpdatePowerCD(n, t1 / cdTime);
            yield return new WaitForEndOfFrame();
        }
        for (t2 = 0; t2 < cdTime && n == 2; t2 += Time.deltaTime)
        {
            cdPower2 = true;
            view.UpdatePowerCD(n, t2 / cdTime);
            yield return new WaitForEndOfFrame();
        }
        for (t3 = 0; t3 < cdTime && n == 3; t3 += Time.deltaTime)
        {
            cdPower3 = true;
            view.UpdatePowerCD(n, t3 / cdTime);
            yield return new WaitForEndOfFrame();
        }
        for (t4 = 0; t4 < cdTime && n == 4; t4 += Time.deltaTime)
        {
            cdPower4 = true;
            view.UpdatePowerCD(n, t4 / cdTime);
            yield return new WaitForEndOfFrame();
        }

        if (n == 1 && t1 >= cdTime)
        {
            cdPower1 = false;
            view.UpdatePowerCD(n, 1);
        }
        if (n == 2 && t2 >= cdTime)
        {
            cdPower2 = false;
            view.UpdatePowerCD(n, 1);
        }
        if (n == 3 && t3 >= cdTime)
        {
            cdPower3 = false;
            view.UpdatePowerCD(n, 1);
        }
        if (n == 4 && t4 >= cdTime)
        {
            cdPower4 = false;
            view.UpdatePowerCD(n, 1);
        }
    }

    public IEnumerator InActionDelay(float cdTime)
    {
        WraperInAction = true;
        yield return new WaitForSeconds(cdTime);
        WraperInAction = false;
    }

    public IEnumerator OnDamageDelay(float cdTime)
    {
        onDamage = true;
        yield return new WaitForSeconds(cdTime);
        onDamage = false;
    }

    public IEnumerator ActionDelay(Action power) {

        yield return new WaitForSeconds(1f);
        RotateAttack();
        power();
    }

    public IEnumerator CountAttack()
    {
        yield return new WaitForSeconds(1f);
        countAnimAttack = 0;
    }

    void Start () {

        rb = GetComponent<Rigidbody>();
        powerManager = FindObjectOfType<PowerManager>();
        powerPool = new Pool<Powers>(10, PowersFactory, Powers.InitializePower, Powers.DisposePower, true);
        mySkills = new Skills();
    }

    void Update () {
      
        timeOnCombat -= Time.deltaTime;
        if(timeOnCombat <=0) timeOnCombat = 0;
        if (timeOnCombat <= 0 && isInCombat)
        {
            Safe();
            isInCombat = false;
        }
 
        WraperAction();
 
    }


    public void CastPower1()
    {
        if (!cdPower1 && !InAction && !onDamage)
        {
            Powers newPower = powerPool.GetObjectFromPool();
            newPower.myCaller = transform;
            powerManager.SetIPower(0, newPower, this);
            Estocada();
        }
    }

    public void CastPower2()
    {
        if (!cdPower2 && !InAction && !onDamage)
        {          
            Powers newPower = powerPool.GetObjectFromPool();
            newPower.myCaller = transform;
            powerManager.SetIPower(1, newPower, this);
            RotateAttack();
        }
    }

    public void CastPower3()
    {
        if (!cdPower3 && !InAction && !onDamage)
        {
            Powers newPower = powerPool.GetObjectFromPool();
            newPower.myCaller = transform;
            powerManager.SetIPower(2, newPower, this);
            Uppercut();
        }
    }

    public void CastPower4()
    {
        if (!cdPower4 && !InAction && !onDamage)
        {
            Powers newPower = powerPool.GetObjectFromPool();
            newPower.myCaller = transform;
            powerManager.SetIPower(3, newPower, this);
        }
    }

    public void Movement(Vector3 direction)
    {      
        if (!InAction && !onDamage)
        {
            Quaternion targetRotation;
            direction.y = 0;
            targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);

            if (!isRuning) rb.MovePosition(rb.position + direction * speed * Time.deltaTime);
          
            else rb.MovePosition(rb.position + direction * runSpeed * Time.deltaTime);
        }
        
    }

    public void Idle()
    {
        isIdle = true;
    }

    public void NoIdle()
    {
        isIdle = false;
    }

    public void NormalAttack()
    {
        if (!isDead)
        {
            Attack();
            countAnimAttack++;
            countAnimAttack = Mathf.Clamp(countAnimAttack, 0, 3);
        }
        if (!InActionAttack)
        {           
            StopCoroutine(CountAttack());
            StartCoroutine(CountAttack());
            InActionAttack = true;
            
        }      
    }

    public void MakeDamage()
    {
        rb.AddForce(transform.forward * 2, ForceMode.Impulse);
        Collider[] col = Physics.OverlapSphere(attackPivot.position, radiusAttack);
        foreach (var item in col)
        {
            if (item.GetComponent<EnemyClass>())
            {
              item.GetComponent<EnemyClass>().GetDamage(10);
               item.GetComponent<Rigidbody>().AddForce(-item.transform.forward * 2, ForceMode.Impulse);  
            }
        }
    }

    public void CombatState()
    {
        timeOnCombat = 5;
        if (!isInCombat) Combat();
        isInCombat = true;
    }

    public void ActiveAttack()
    {
        InActionAttack = false;
        InAction = false;
    }

    public void CountAnimZero()
    {
        countAnimAttack = 0;
    }

    public void GetDamage(float damage, Transform enemy)
    {
        life -= damage;
        view.animTrotSpeedX = 0;
        view.animTrotSpeedZ = 0;
        StartCoroutine(OnDamageDelay(1f));
        if (!InAction)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(enemy.forward * 2, ForceMode.Impulse);
        }
        if (life > 0) OnDamage();
        else
        {
            Dead();
            isDead = true;
        }
    }

    public Powers PowersFactory()
    {
        Powers newPower = Instantiate(prefabPower);
        newPower.transform.SetParent(powerManager.transform);
        newPower.myCaller = transform;
        return newPower;
    }

    public void ReturnBulletToPool(Powers powers)
    {
        this.powerPool.DisablePoolObject(powers);
    }

    public void OnCollisionEnter(Collision c)
    {
       if (stocadaState)
        {
            enemy = c.gameObject.GetComponent<Collider>();
            powerManager.currentPowerAction.Ipower2();
            stocadaAmount++;
            if (stocadaAmount > powerManager.amountOfTimes)
            {
              powerManager.constancepower = false;
              powerManager.currentPowerAction = null;
              stocadaState = false;
              stocadaAmount = 0;
              powerManager.amountOfTimes = 0;
              view.BackEstocada();
            }
        }
       
       if (jumpAttackWarriorState)
        {
            powerManager.currentPowerAction.Ipower2();
            powerManager.constancepower = false;
            powerManager.currentPowerAction = null;
            jumpAttackWarriorState = false;
            onAir = false;
        }

       if (chargeTankeState)
        {
            if(c.gameObject.GetComponent(typeof(EnemyClass))) currentEnemy = c.gameObject.GetComponent<EnemyClass>();
            powerManager.currentPowerAction.Ipower2();
        }

        if (c.gameObject.GetComponent<Platform>())
            currentPlatform = c.gameObject.GetComponent<Platform>();
    }

    public void StartInteraction()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1.5f))
        {
            var comp = hit.transform.GetComponent<Interactable>();
            if (comp)
                comp.Interaction();
        }
    }

    public IEnumerator PlatformJump()
    {
        if (currentPlatform)
        {
            Vector3 p1 = currentPlatform.transform.position;
            Vector3 p3 = currentPlatform.otherPlatform.position;
            Vector3 p2 = Vector3.Lerp(p1, p3, 0.5f);
            p2.y = (p1.y > p3.y ? p1.y : p3.y) + 15;
            float t = 0;
            isPlatformJumping = true;

            while (currentPlatform != null)
            {
                t += Time.deltaTime;
                rb.transform.position = Vector3.Lerp(Vector3.Lerp(p1, p2, t), Vector3.Lerp(p2, p3, t), t);

                if (transform.position.x == p3.x && transform.position.z == p3.z)
                {
                    currentPlatform = null;
                    isPlatformJumping = false;
                }
                yield return new WaitForFixedUpdate();
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPivot.position, radiusAttack);
    }

    public void WraperAction()
    {
        if (stocadaState || WraperInAction || chargeTankeState || jumpAttackWarriorState || InActionAttack || onAir || isDead) InAction = true;
        else InAction = false;
    }


}
