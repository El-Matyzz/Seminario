using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Model : MonoBehaviour {

    public Transform attackPivot;

    PowerManager _powerManager;
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
    float totalTime = 0.5f;
    public float actualtime = 0;
    public Collider enemy;  

    public int stocadaAmount;

    public Skills mySkills;
    public bool stocadaState;
    public bool jumpAttackWarriorState;
    public bool chargeTankeState;
    public bool isRuning;
    public bool isAnimatedMove;
    public bool aux;
    bool cdPower1;
    bool cdPower2;
    bool cdPower3;
    bool cdPower4;
    bool InAction;
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
    
    public event Action Estocada;
    public event Action Attack;
    public event Action RotateAttack;
    public event Action SaltoyGolpe1;
    public event Action SaltoyGolpe2;

    public IEnumerator PowerColdown(float cdTime, int n)
    {
        if (n == 1) cdPower1 = true;
        if (n == 2) cdPower2 = true;
        if (n == 3) cdPower3 = true;
        if (n == 4) cdPower4 = true;
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

    void Start () {

        rb = GetComponent<Rigidbody>();
        _powerManager = FindObjectOfType<PowerManager>();
        powerPool = new Pool<Powers>(10, PowersFactory, Powers.InitializePower, Powers.DisposePower, true);
        mySkills = new Skills();
    }

    void Update () {

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
            _powerManager.SetIPower(0, newPower, this);
            Estocada();
        }
    }

    public void CastPower2()
    {
        if (!cdPower2 && !InAction)
        {          
            Powers newPower = powerPool.GetObjectFromPool();
            newPower.myCaller = transform;
            _powerManager.SetIPower(1, newPower, this);
            RotateAttack();
        }
    }

    public void CastPower3()
    {
        if (!cdPower3 && !InAction)
        {
            Powers newPower = powerPool.GetObjectFromPool();
            newPower.myCaller = transform;
            _powerManager.SetIPower(2, newPower, this);
        }
    }

    public void CastPower4()
    {
        if (!cdPower4 && !InAction)
        {
            Powers newPower = powerPool.GetObjectFromPool();
            newPower.myCaller = transform;
            _powerManager.SetIPower(3, newPower, this);
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
   
    public void NormalAttack()
    {
        if (!InAction)
        {
            Attack();
            Collider[] col = Physics.OverlapSphere(attackPivot.position, radiusAttack);
            foreach (var item in col)
            {
                if (item.GetComponent<EnemyClass>()) item.GetComponent<EnemyClass>().GetMeleDamage(10, transform);
            }
        }
    }

    public Powers PowersFactory()
    {
        Powers newPower = Instantiate(prefabPower);
        newPower.transform.SetParent(_powerManager.transform);
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
            _powerManager.currentPowerAction.Ipower2();
            stocadaAmount++;
            if (stocadaAmount > _powerManager.amountOfTimes)
            {
                 _powerManager.constancepower = false;
                 _powerManager.currentPowerAction = null;
                stocadaState = false;
                stocadaAmount = 0;
                _powerManager.amountOfTimes = 0;
            }
        }
       
       if (jumpAttackWarriorState)
        {
            _powerManager.currentPowerAction.Ipower2();
            _powerManager.constancepower = false;
            _powerManager.currentPowerAction = null;
            jumpAttackWarriorState = false;
        }

       if (chargeTankeState)
        {
            if(c.gameObject.GetComponent(typeof(EnemyClass))) currentEnemy = c.gameObject.GetComponent<EnemyClass>();
            _powerManager.currentPowerAction.Ipower2();
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPivot.position, radiusAttack);
    }

    public void AnimSaltoyGolpe1()
    {
        SaltoyGolpe1();
    }

    public void AnimSaltoyGolpe2()
    {
        SaltoyGolpe2();
    }


    public void WraperAction()
    {
        if (stocadaState || WraperInAction || chargeTankeState || jumpAttackWarriorState) InAction = true;
        else InAction = false;
    }
}
