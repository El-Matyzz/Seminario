using UnityEngine;

public class ExtraHealth : IPotionEffect {

    Model m;
    float timer;

    public ExtraHealth(Model M, float Timer)
    {
        m = M;
        timer = Timer;
    }

    public void PotionEffect()
    {
        timer -= Time.deltaTime;
        m.view.UpdateTimer(((int)timer).ToString());

        if (!m.armorActive)
        {
            m.armorActive = true;
            m.armor = m.totalArmor;
            m.view.UpdateArmorBar(m.armor / m.totalArmor);
            if(m.life > m.totalLife * 0.15f)
            {
                m.life -= m.totalLife * 0.15f;
                m.view.UpdateLifeBar(m.life / m.totalLife);
            }
            if(m.stamina > m.totalStamina * 0.15f)
            {
                m.stamina -= m.totalStamina * 0.15f;
                m.view.UpdateStaminaBar(m.stamina / m.totalStamina);
            }
        }
        if(timer <= 0)
        {
            m.armor = 0;
            m.armorActive = false;
            m.view.UpdateArmorBar(m.armor / m.totalArmor);
            m.view.UpdateTimer();
            m.currentPotionEffect = null;
        }
    }
}
