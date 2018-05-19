using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Model : MonoBehaviour {

    PowerManager _powerManager;
   // public Powers prefabPower;
    public Pool<Powers> powerPool;

    public int power1ID;
    public int power2ID;
    public int power3ID;
    public int power4ID;

    public float extraFireDamage;
    public float extraSlameDamage;
    public float skillPoints;
    public float speed;
    public float runSpeed;
    public Collider enemy;

    public int stocadaAmount;

    public Skills mySkills;
    public bool stocadaState;
    public bool jumpAttackWarriorState;
    public bool chargeTankeState;
    public bool isRuning;

    public Transform mainCamera;
    public Vector3 dir;
    public Rigidbody rb;
    public EnemyClass currentEnemy;

    public event Action Idle;
    public event Action Trot;
    public event Action Run;
    public event Action Estocada;
    public event Action GolpeGiratorio;
    public event Action SaltoyGolpe1;
    public event Action SaltoyGolpe2;

    // Use this for initialization
    void Start () {

        rb = GetComponent<Rigidbody>();
        _powerManager = FindObjectOfType<PowerManager>();
       // powerPool = new Pool<Powers>(10, PowersFactory, Powers.InitializePower, Powers.DisposePower, true);
        mySkills = new Skills();
    }

    void Update () {
		
	}


    public void CastPower1()
    {
        Powers newPower = powerPool.GetObjectFromPool();
        newPower.myCaller = transform;
        _powerManager.SetIPower(power1ID, newPower, this);
        Estocada();
    }

    public void CastPower2()
    {
        Powers newPower = powerPool.GetObjectFromPool();
        newPower.myCaller = transform;
        _powerManager.SetIPower(power2ID, newPower, this);
        GolpeGiratorio();
    }

    public void CastPower3()
    {
        Powers newPower = powerPool.GetObjectFromPool();
        newPower.myCaller = transform;
        _powerManager.SetIPower(power3ID, newPower, this);
    }

    public void CastPower4()
    {
        Powers newPower = powerPool.GetObjectFromPool();
        newPower.myCaller = transform;
        _powerManager.SetIPower(power4ID, newPower, this);
    }

    public void AnimIdel()
    {
        Idle();
    }   

    public void MoveForward()
    {
        if (!chargeTankeState)
        {
            var dirCamera = mainCamera.forward;
            dirCamera.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dirCamera, 0.5f);
            if (!isRuning)
            {
                rb.MovePosition(rb.position + dirCamera * speed * Time.deltaTime);
                Trot();
            }
            else
            {
                rb.MovePosition(rb.position + dirCamera * runSpeed * Time.deltaTime);
                Run();
            }
        }
    }

    public void MoveBackward()
    {
        if (!chargeTankeState)
        {
            var dir = -mainCamera.transform.forward;
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, 0.5f);
            if (!isRuning)
            {
                rb.MovePosition(rb.position + dir * speed * Time.deltaTime);
                Trot();
            }
            else
            {
                rb.MovePosition(rb.position + dir * runSpeed * Time.deltaTime);
                Run();
            }
            
        }
    }

    public void MoveLeft()
    {
        if (!chargeTankeState)
        {
            dir = -mainCamera.right;
            transform.forward = Vector3.Lerp(transform.forward, dir, 0.5f);
            if (!isRuning)
            {
                rb.MovePosition(rb.position + dir * speed * Time.deltaTime);
                Trot();
            }
            else
            {
                rb.MovePosition(rb.position + dir * runSpeed * Time.deltaTime);
                Run();
            }
        }
    }

    public void MoveRight()
    {
        if (!chargeTankeState)
        {
            dir = mainCamera.right;
            transform.forward = Vector3.Lerp(transform.forward, dir, 0.5f);
            if (!isRuning)
            {
                rb.MovePosition(rb.position + dir * speed * Time.deltaTime);
                Trot();
            }
            else
            {
                rb.MovePosition(rb.position + dir * runSpeed * Time.deltaTime);
                Run();
            }
        }
    }

  /*  public Powers PowersFactory()
    {
        Powers newPower = Instantiate(prefabPower);
        newPower.transform.SetParent(_powerManager.transform);
        newPower.myCaller = transform;
        return newPower;
    }*/

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

    public void AnimSaltoyGolpe1()
    {
        SaltoyGolpe1();
    }

    public void AnimSaltoyGolpe2()
    {
        SaltoyGolpe2();
    }

    public void AnimIdle()
    {
        Idle();
    }
}
