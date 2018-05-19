using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerManager : MonoBehaviour {
  
    public Ipower currentPowerAction;
    public IDecorator _currentDecorator;
    public List<GameObject> powerParticles = new List<GameObject>();
    public List<BoxCollider> powerColliders = new List<BoxCollider>();
    public List<Collider> enemies= new List<Collider>();
    public bool constancepower;
    public int amountOfTimes;
	Vector3 mousePosition;

    void Start () {
		
    }
	
	void Update () {
		
        if (constancepower==true) currentPowerAction.Ipower();
    }

    public void SetIPower(int id, Powers power, Model model)
    {
        if (id == 0)
        {
            float extraDamage = model.extraFireDamage;
            currentPowerAction = new FireBall(power, model, extraDamage);
            power.SetStrategy(currentPowerAction);          
            var p = Instantiate(powerParticles[id]);
            power.newParticles = p;
            p.transform.position = power.transform.position;
            p.transform.SetParent(power.transform);
            p.transform.forward = power.transform.forward;
            SetDecorator(id,p, power, model);
        }

        if (id ==1)
        {
            model.ReturnBulletToPool(power);
            var rb = model.GetComponent<Rigidbody>();
            var actualPos = model.transform.position;
            var radius = model.mySkills.RadiusSlameSkill;
            float extraDamage = model.extraSlameDamage;
            currentPowerAction = new Slame(actualPos, extraDamage, radius, rb);
            currentPowerAction.Ipower();
            constancepower = false;
        }
        
        if (id==2)
        {
            model.ReturnBulletToPool(power);
            currentPowerAction = new RotateAttackWarrior(model.transform);
            currentPowerAction.Ipower();
            constancepower = false;
        }

		if (id==3)
		{
            model.ReturnBulletToPool(power);          				
			currentPowerAction = new JumpAttackWarrior(mousePosition, model.transform, model);
			constancepower = true;
		}

        if (id==4)
        {
            model.ReturnBulletToPool(power);
            currentPowerAction = new StocadaWarrior(model,this);
            constancepower = true;
        }

        if (id==5)
        {
            model.ReturnBulletToPool(power);
            currentPowerAction = new UppercutWarrior(model);
            currentPowerAction.Ipower();
            constancepower = false;
        }

        if (id==6)
        {
            model.ReturnBulletToPool(power);
            currentPowerAction = new ShieldPunchTanke(model);
            currentPowerAction.Ipower();
            constancepower = false;
        }

        if (id == 7)
        {
            model.ReturnBulletToPool(power);
            currentPowerAction = new ChargeTanke(model);
            constancepower = true;
        }

    }

    public void SetDecorator(int id , GameObject particles, Powers power , Model model)
    {

        if (id == 0)
        {
            if (model.mySkills.FireSkill1)
            {
                _currentDecorator = new BiggerFireBall(particles);
                power.SetDecorator(_currentDecorator);
            }
        }
    }

   
}
