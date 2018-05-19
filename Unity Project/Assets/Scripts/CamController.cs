using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour {

    //THIS IS A TEST!
    public Transform player;

    public float distance;
    float currentX = 0;
    float currentY = 0;
    public float sensitivityX;
    public float sensitivityY;
    public bool invertY;
    public bool cameraActivate;

    void Start () {
        if (invertY)
            sensitivityY = -sensitivityY;
    }

    void Update () {

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
            Vector3 direction = new Vector3(0, 0, distance);
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            transform.position = player.position + rotation * direction;
            transform.LookAt(player.position);
        }
	}
}
