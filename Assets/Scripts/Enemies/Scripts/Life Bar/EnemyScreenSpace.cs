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
    Slider healthSlider;

    DepthUI depthUI;
    public Renderer enemyRenderer;

    void Start()
    {
        enemy = GetComponent<EnemyClass>();
        healthBar = Instantiate(barPrefab);
        healthBar.transform.SetParent(canvas.transform, false);
        healthSlider = healthBar.GetComponent<Slider>();

        depthUI = healthBar.GetComponent<DepthUI>();
        canvas.GetComponent<ScreenSpaceCanvas>().AddToCanvas(healthBar);

        maxLife = enemy.life;
    }

    void Update()
    {
        Vector3 worldPos = transform.position + Vector3.up;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        healthBar.transform.position = screenPos;

        float distance = Vector3.Distance(worldPos, Camera.main.transform.position);
        depthUI.depth = -distance;

        Vector3 vp = Camera.main.WorldToViewportPoint(worldPos);

        if (distance < 15 && (vp.x >= 0 && vp.x <= 1 && vp.y >= 0 && vp.y <= 1 && vp.z > 0))
            healthBar.SetActive(true);
        else
            healthBar.SetActive(false);
    }

    public void UpdateLifeBar (float val)
    {
        healthSlider.value = val / maxLife;
    }

    void OnDestroy()
    {
        if (canvas)
            canvas.GetComponent<ScreenSpaceCanvas>().RemoveFromCanvas(healthBar);
        Destroy(healthBar);
    }
}
