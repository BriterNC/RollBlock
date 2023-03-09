using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotationController : MonoBehaviour
{
    [SerializeField] private bool isRotating;
    [SerializeField] private float rotateSpeed = 500f;

    private void Update()
    {
        if (isRotating)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            StartCoroutine(Rotate(Vector3.up));
        }
        else if (Input.GetKey(KeyCode.E))
        {
            StartCoroutine(Rotate(Vector3.down));
        }
    }

    private IEnumerator Rotate(Vector3 angle)
    {
        isRotating = true;
        
        var angleToRotate = 90f;
        
        while (angleToRotate > 0)
        {
            float rotationAngle = Mathf.Min(Time.deltaTime * rotateSpeed, angleToRotate);
            transform.Rotate(angle, rotationAngle);
            angleToRotate -= rotationAngle;
            yield return null;
        }

        transform.GetChild(0).transform.position = new Vector3(Mathf.RoundToInt(transform.GetChild(0).transform.position.x),Mathf.RoundToInt(transform.GetChild(0).transform.position.y),Mathf.RoundToInt(transform.GetChild(0).transform.position.z));
        
        isRotating = false;
    }
}
