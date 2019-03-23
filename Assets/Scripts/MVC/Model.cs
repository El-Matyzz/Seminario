using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Model : MonoBehaviour
{
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
    public float acceleration;
    public float maxAcceleration;
    public float timeOnCombat;
    public float stamina;
    public float totalStamina;
    public float mana;
    public float totalMana;
    public float recoveryMana;
    public float armor;
    public float totalArmor;
    public bool armorActive;

    public float runStamina;
    public float attackStamina;
    public float powerStamina;
    public float dashStamina;
    public float recoveryStamina;
    public float timeAnimCombat;

    public int[] potions = new int[5];
    public IPotionEffect currentPotionEffect;
    IPotionEffect[] potionEffects = new IPotionEffect[5];

    public int countAnimAttack;
    public Collider enemy;

    public int stocadaAmount;

    public Skills mySkills;
    public bool isIdle;
    public bool onAir;
    public bool stocadaState;
    public bool onPowerState;
    public bool jumpAttackWarriorState;
    public bool chargeTankeState;
    public bool isRuning;
    public bool isAnimatedMove;
    public bool isInCombat;
    public bool isDead;
    public bool onDefence;
    public bool onDash;
    public bool biz;

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

    public Action Trot;
    public Action Run;
    public Action Estocada;
    public event Action Attack;
    public event Action RotateAttack;
    public Action SaltoyGolpe1;
    public Action SaltoyGolpe2;
    public Action Uppercut;
    public Action OnDamage;
    public Action Fall;
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
        onPowerState = false;
    }

    public IEnumerator ActionDelay(Action power)
    {
        yield return new WaitForSeconds(1.5f);
        RotateAttack();
        power();
    }

    public IEnumerator Dash(Vector3 dir)
    {
        if (!onDash && stamina - dashStamina >= 0)
        {
            stamina -= dashStamina;
            view.UpdateStaminaBar(stamina / totalStamina);

            rb.velocity = Vector3.zero;
            rb.AddForce(dir * 8, ForceMode.Impulse);
            onDash = true;
            yield return new WaitForSeconds(1f);
            onDash = false;
        }
    }

    public IEnumerator PowerDelay(float time)
    {
        yield return new WaitForSeconds(time);
        Powers newPower = powerPool.GetObjectFromPool();
        newPower.myCaller = transform;
        powerManager.SetIPower(2, newPower, this);

        onAir = true;
        onPowerState = true;
    }

    public IEnumerator CountAttack()
    {
        yield return new WaitForSeconds(1f);
        countAnimAttack = 0;
    }

    public enum PotionName { Health, Extra_Health, Stamina, Costless_Hit, Mana };

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        powerManager = FindObjectOfType<PowerManager>();
        powerPool = new Pool<Powers>(10, PowersFactory, Powers.InitializePower, Powers.DisposePower, true);
        mySkills = new Skills();

        for (int i = 0; i < 5; i++)
            view.UpdatePotions(i);
        potionEffects[1] = new ExtraHealth(this, 60);
        currentPotionEffect = null;
    }

    void Update()
    {
        timeOnCombat -= Time.deltaTime;
        if (timeOnCombat > 0)
        {
            view.anim.SetBool("IdleCombat", true);

        }
        if (timeOnCombat <= 0) timeOnCombat = 0;
        if (timeOnCombat <= 0 && isInCombat)
        {
            view.anim.SetBool("IdleCombat", false);
            view.anim.SetBool("Idle", true);
            isInCombat = false;
        }

        WraperAction();
        if (life <= 0)
        {
            Dead();
            isDead = true;
        }

        if (!isRuning && !onPowerState && !onDamage && !isDead && !onDash)
        {
            float prevS = stamina;
            stamina += recoveryStamina * Time.deltaTime;
            if (stamina > totalStamina)
                stamina = totalStamina;
            if (prevS != stamina)
                view.UpdateStaminaBar(stamina / totalStamina);
        }

        float prevM = mana;
        mana += recoveryMana * Time.deltaTime;
        if (mana > totalMana)
            mana = totalMana;
        if (prevM != mana)
            view.UpdateManaBar(mana / totalMana);

        if (stamina <= 0)
        {
            isRuning = false;
            view.FalseRunAnim();
        }

        if (currentPotionEffect != null)
            currentPotionEffect.PotionEffect();


        if (countAnimAttack > 0)
        {
            timeAnimCombat -= Time.deltaTime;
            if (timeAnimCombat < 0)
            {
                countAnimAttack = 0;
                view.currentAttackAnimation = 0;
                view.anim.SetInteger("AttackAnim", 0);
            }
        }
    }

    public void DrinkPotion(int i)
    {
        i -= 1;

        if (potions[i] == 0 || currentPotionEffect != null)
            return;

        if (i == (int)PotionName.Health)
            potionEffects[i] = new Health(this, life, totalLife);
        else
            if (i == (int)PotionName.Stamina)
            potionEffects[i] = new Stamina(this, stamina, totalStamina);
        else
                if (i == (int)PotionName.Mana)
            potionEffects[i] = new Mana(this, mana, totalMana);

        potions[i]--;
        view.UpdatePotions(i);
        currentPotionEffect = potionEffects[i];
    }

    public void CastPower1()
    {
        if (!cdPower1 && !onPowerState && !onDamage && !isDead && !onDash && stamina - powerStamina >= 0)
        {
            stamina -= powerStamina;
            view.UpdateStaminaBar(stamina / totalStamina);

            Powers newPower = powerPool.GetObjectFromPool();
            newPower.myCaller = transform;
            powerManager.SetIPower(0, newPower, this);
            Estocada();
            onPowerState = true;
        }
    }

    public void CastPower2()
    {
        if (!cdPower2 && !onPowerState && !onDamage && !isDead && !onDash && stamina - powerStamina >= 0)
        {
            stamina -= powerStamina;
            view.UpdateStaminaBar(stamina / totalStamina);

            Powers newPower = powerPool.GetObjectFromPool();
            newPower.myCaller = transform;
            powerManager.SetIPower(1, newPower, this);
            RotateAttack();
            onPowerState = true;
        }
    }

    public void CastPower3()
    {
        if (!cdPower3 && !onPowerState && !onDamage && !isDead && !onDash && stamina - powerStamina >= 0)
        {
            stamina -= powerStamina;
            view.UpdateStaminaBar(stamina / totalStamina);

            CombatState();
            Uppercut();
            StartCoroutine(PowerDelay(0.5f));
        }
    }

    public void CastPower4()
    {
        if (!cdPower4 && !onPowerState && !onDamage && !isDead && !onDash && !onAir && countAnimAttack == 0 && stamina - powerStamina >= 0)
        {
            Powers newPower = powerPool.GetObjectFromPool();
            newPower.myCaller = transform;
            powerManager.SetIPower(3, newPower, this);
            onPowerState = true;
        }
    }

    public void Movement(Vector3 direction, bool key, bool backward, bool rotate)
    {
        if (isRuning)
        {
            stamina -= runStamina * Time.deltaTime;
            view.UpdateStaminaBar(stamina / totalStamina);
        }

        biz = false;
        acceleration += 3f * Time.deltaTime;
        if (acceleration > maxAcceleration) acceleration = maxAcceleration;

        if (!InAction && !onDamage && countAnimAttack == 0 && !onDash)
        {
            Quaternion targetRotation;
            direction.y = 0;
            if (key)
            {
                targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
            }

            if (backward)
            {
                var turnDir = -direction;
                targetRotation = Quaternion.LookRotation(turnDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
            }

            if (rotate)
            {
                targetRotation = Quaternion.LookRotation(mainCamera.forward, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
            }

            if (!isRuning && !isInCombat)
            {
                Trot();
                rb.MovePosition(rb.position + direction * acceleration * speed * Time.deltaTime);
            }
            else if (!isRuning && isInCombat)
            {
                Trot();
                rb.MovePosition(rb.position + direction * acceleration * speed * Time.deltaTime);
            }
            else
            {
                Run();
                rb.MovePosition(rb.position + direction * acceleration * runSpeed * Time.deltaTime);
            }
        }
    }

    public void MovementBizectriz(Vector3 d1, Vector3 d2, bool key, bool backward)
    {
        biz = true;
        acceleration += 3f * Time.deltaTime;
        if (acceleration > maxAcceleration) acceleration = maxAcceleration;

        if (!InAction && !onDamage && countAnimAttack == 0 && !onDash)
        {
            Vector3 direction = (d1 + d2) / 2;
            direction.y = 0;

            Quaternion targetRotation;

            if (key)
            {

                targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
            }

            if (backward)
            {
                var turnDir = (d1 + (-d2)) / 2;
                targetRotation = Quaternion.LookRotation(turnDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 7 * Time.deltaTime);
            }

            if (!isRuning && !isInCombat)
            {

                Trot();
                rb.MovePosition(rb.position + direction * acceleration * speed * Time.deltaTime);
            }
            else if (!isRuning && isInCombat)
            {
                Trot();
                rb.MovePosition(rb.position + direction * acceleration * speed * Time.deltaTime);
            }
            else
            {
                Run();
                rb.MovePosition(rb.position + direction * acceleration * runSpeed * Time.deltaTime);
            }
        }
    }

    public void FallDelay()
    {
        StartCoroutine(InActionDelay(0.7f));
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
        if (!isDead && stamina - attackStamina >= 0)
        {
            timeAnimCombat = 0.5f;
            countAnimAttack++;
            countAnimAttack = Mathf.Clamp(countAnimAttack, 0, 3);
            Attack();
        }
        if (!InActionAttack) InActionAttack = true;

    }

    public void MakeDamage()
    {
        stamina -= attackStamina;
        view.UpdateStaminaBar(stamina / totalStamina);

        if (countAnimAttack > 1) rb.AddForce(transform.forward * 2, ForceMode.Impulse);
        var col = Physics.OverlapSphere(attackPivot.position, radiusAttack).Where(x => x.GetComponent<EnemyClass>()).Select(x => x.GetComponent<EnemyClass>());
        var desMesh = Physics.OverlapSphere(attackPivot.position, radiusAttack).Where(x => x.GetComponent<DestructibleOBJ>()).Select(x => x.GetComponent<DestructibleOBJ>()); ;
        foreach (var item in col)
        {
            view.StartCoroutine(view.SlowSpeed());
            item.GetDamage(10);
            item.GetComponent<Rigidbody>().AddForce(-item.transform.forward * 2, ForceMode.Impulse);
        }

        Debug.Log(desMesh.Count());
        foreach (var item in desMesh)
        {
            item.StartCoroutine(item.startDisolve());
        }
    }

    public void Defence()
    {
        onDefence = true;
    }

    public void StopDefence()
    {
        onDefence = false;
    }

    public void CombatState()
    {
        timeOnCombat = 10;
        view.anim.SetBool("IdleCombat", true);
        isInCombat = true;
    }

    public void ActiveAttack()
    {
        InActionAttack = false;
        InAction = false;
    }

    public void FalseActiveAttack()
    {
        InActionAttack = true;
        InAction = true;
    }

    public void FalseOnDamage()
    {
        onDamage = false;
    }

    public void GetDamage(float damage, Transform enemy, bool isProyectile)
    {

        bool isBehind = false;
        Vector3 dir = transform.position - enemy.position;
        float angle = Vector3.Angle(dir, transform.forward);
        if (angle < 90) isBehind = true;

        if (!onDefence || (onDefence && isBehind) || isProyectile)
        {
            if (armor >= damage)
            {
                armor -= damage;
                view.UpdateArmorBar(armor / totalArmor);
            }
            else
            {
                float dmg = damage - armor;
                armor = 0;
                view.UpdateArmorBar(armor / totalArmor);
                life -= dmg;
                view.UpdateLifeBar(life / totalLife);
            }

            if (!onPowerState)
            {
                rb.velocity = Vector3.zero;
                onDamage = true;
            }
            if (life > 0) OnDamage();
            else
            {
                Dead();
                isDead = true;
                StartCoroutine(view.YouDied());
            }
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
        powerPool.DisablePoolObject(powers);
    }

    public void OnCollisionEnter(Collision c)
    {
        if (onAir)
        {
            Fall();
            onAir = false;
        }
        if ((stocadaState && c.gameObject.GetComponent<EnemyClass>()) ||
            (stocadaState && c.gameObject.layer == LayerMask.NameToLayer("Obstacles")))
        {
            enemy = c.gameObject.GetComponent<Collider>();
            powerManager.currentPowerAction.Ipower2();
            stocadaAmount++;
            if (stocadaAmount > powerManager.amountOfTimes)
            {
                powerManager.constancepower = false;
                powerManager.currentPowerAction = null;
                stocadaState = false;
                onPowerState = false;
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
            onAir = false;
            onPowerState = false;
        }

        if (chargeTankeState)
        {
            if (c.gameObject.GetComponent(typeof(EnemyClass))) currentEnemy = c.gameObject.GetComponent<EnemyClass>();
            powerManager.currentPowerAction.Ipower2();
        }

        if (c.gameObject.GetComponent<Platform>())
            currentPlatform = c.gameObject.GetComponent<Platform>();
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.layer == 10)
            StartCoroutine(view.YouWin());

        if (c.gameObject.layer == LayerMask.NameToLayer("Life"))
        {
            if (life < totalLife) life += 30;
            else life = totalLife;
            view.UpdateLifeBar(life / totalLife);
            var col = Physics.OverlapSphere(c.transform.position, 1);
            foreach (var i in col)
                if (i.transform.parent)
                    if (i.transform.parent.name == "Chest")
                        Destroy(i.transform.parent.gameObject);
            Destroy(c.gameObject);
        }
    }

    public void StopJumpAttack()
    {
        jumpAttackWarriorState = false;
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
        if (stocadaState || WraperInAction || chargeTankeState || jumpAttackWarriorState || InActionAttack || onAir || isDead || onDamage || onDash) InAction = true;
        else InAction = false;
    }
}
