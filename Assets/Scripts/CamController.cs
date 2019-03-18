using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour {

    public Transform player;
    public Model model;
    public float distance;
    float currentX = 0;
    float currentY = 0;
    public float sensitivityX;
    public float sensitivityY;
    public float smooth;
    public bool invertY;
    public bool cameraActivate;
    public bool blockMouse;

    Vector3 startPositionPivot;

    void Start () {
        if (invertY)
            sensitivityY = -sensitivityY;
        model = FindObjectOfType<Model>();

    }

    void Update () {

        if (blockMouse)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        if (!cameraActivate)
        {
            currentX += Input.GetAxis("Mouse X") * sensitivityX;
            currentY += Input.GetAxis("Mouse Y") * sensitivityY;
            currentY = Mathf.Clamp(currentY, 0, 30);
        }
    }
	
	void FixedUpdate () {

        if (!cameraActivate)
        {
            if (model.isInCombat)
            {
              distance += 35 * Time.deltaTime;
              if (distance >= 5) distance = 5;
            }
            else
            {
              distance -= 5 * Time.deltaTime;
              if (distance <= 3) distance =3;
            }
            Vector3 direction = new Vector3(0, 0, distance);
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            transform.position =Vector3.Lerp(player.position, player.position + rotation * direction,Time.deltaTime * smooth);
            transform.LookAt(player.position);
        }
	}
}
