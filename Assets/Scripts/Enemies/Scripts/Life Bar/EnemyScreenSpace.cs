using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScreenSpace : MonoBehaviour
{
    EnemyClass enemy;
    float maxLife;

    public Canvas canvas;
    public GameObject barPrefab;

    GameObject healthBar;
    Image healthFill;

    DepthUI depthUI;

    public float offset;

    void Start()
    {
        enemy = GetComponent<EnemyClass>();
        healthBar = Instantiate(barPrefab);
        healthBar.transform.SetParent(canvas.transform, false);
        healthFill = healthBar.transform.GetChild(0).GetComponent<Image>();

        depthUI = healthBar.GetComponent<DepthUI>();
        canvas.GetComponent<ScreenSpaceCanvas>().AddToCanvas(healthBar);

        maxLife = enemy.life;
    }

    void Update()
    {
        Vector3 worldPos = transform.position + (Vector3.up * offset);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        float distance = Vector3.Distance(worldPos, Camera.main.transform.position);
        depthUI.depth = -distance;

        Vector3 vp = Camera.main.WorldToViewportPoint(worldPos);

        if (distance < 15 && (vp.x >= 0 && vp.x <= 1 && vp.y >= 0 && vp.y <= 1 && vp.z > 0))
        {
            healthBar.SetActive(true);
            healthBar.transform.position = screenPos;
        }
        else
            healthBar.SetActive(false);
    }

    public IEnumerator BarSmooth(float target)
    {
        bool timerRunning = true;
        float smoothTimer = 0;

        float current = healthFill.fillAmount;

        if (current - target <= 0.025f)
            healthFill.fillAmount = target;

        while (timerRunning)
        {
            smoothTimer += Time.deltaTime * 1.5f;
            healthFill.fillAmount = Mathf.Lerp(current, target, smoothTimer);
            if (smoothTimer > 1)
                timerRunning = false;
            yield return new WaitForEndOfFrame();
        }
    }

    void OnDisable()
    {
        if (canvas)
            canvas.GetComponent<ScreenSpaceCanvas>().RemoveFromCanvas(healthBar);
        Destroy(healthBar);
    }
}
