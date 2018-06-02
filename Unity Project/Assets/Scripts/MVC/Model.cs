using System;
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
    float totalTime = 0.5f;
    public float actualtime = 0;
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
    public bool aux;
    bool cdPower1;
    bool cdPower2;
    bool cdPower3;
    bool cdPower4;
    public bool InAction;
    public bool InActionAttack;
    bool WraperInAction;
    public bool forward;
    public bool backward;
    public bool left;
    public bool right;

    public Transform mainCamera;
    public Vector3 dir;
    public Rigidbody rb;
    public EnemyClass currentEnemy;

    List<bool> cdList = new List<bool>();
    
    public  Action Estocada;
    public event Action Attack;
    public event Action RotateAttack;
    public Action SaltoyGolpe1;
    public Action SaltoyGolpe2;
    public Action OnDamage;
    public event Action Combat;
    public event Action Safe;
    public Action Dead;

    
    public IEnumerator PowerColdown(float cdTime, int n)
    {
        if (n == 1) cdPower1 = true;
        if (n == 2) cdPower2 = true;
        if (n == 3) cdPower3 = true;
        if (n == 4) cdPower4 = true;
        view.NoBackEstocada();
        yield return new WaitForSeconds(cdTime);
        if (n == 1) cdPower1 = false;
        if (n == 2) cdPower2 = false;
        if (n == 3) cdPower3 = false;
        if (n == 4) cdPower4 = false;
        
    }

    public IEnumerator InActionDelay(float cdTime)
    {
        WraperInAction = true;
        yield return new WaitForSeconds(cdTime);
        WraperInAction = false;
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
        if (timeOnCombat <= 0) timeOnCombat = 0;

        if (timeOnCombat > 0)
        {            
            isInCombat = true;
            Combat();
        }
        else
        {
          view.FalseTakeSword();  
          isInCombat = false;
          Safe();
        }
        WraperAction();
        actualtime += Time.deltaTime;
        if (actualtime >= totalTime) actualtime = totalTime;
    }


    public void CastPower1()
    {
        if (!cdPower1 && !InAction)
        {
            Powers newPower = powerPool.GetObjectFromPool();
            newPower.myCaller = transform;
            powerManager.SetIPower(0, newPower, this);
            Estocada();
        }
    }

    public void CastPower2()
    {
        if (!cdPower2 && !InAction)
        {          
            Powers newPower = powerPool.GetObjectFromPool();
            newPower.myCaller = transform;
            powerManager.SetIPower(1, newPower, this);
            RotateAttack();
        }
    }

    public void CastPower3()
    {
        if (!cdPower3 && !InAction)
        {
            Powers newPower = powerPool.GetObjectFromPool();
            newPower.myCaller = transform;
            powerManager.SetIPower(2, newPower, this);
        }
    }

    public void CastPower4()
    {
        if (!cdPower4 && !InAction)
        {
            Powers newPower = powerPool.GetObjectFromPool();
            newPower.myCaller = transform;
            powerManager.SetIPower(3, newPower, this);
        }
    }

    public void Movement(Vector3 direction)
    {      
        if (!InAction)
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
            rb.AddForce(transform.forward * 10, ForceMode.Impulse);
        }      
    }

    public void MakeDamage()
    {
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

    public void StartInCombat()
    {
        view.FalseSaveSword();
        Combat();
        timeOnCombat = 5;
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
        rb.AddForce(enemy.forward * 2, ForceMode.Impulse);
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
