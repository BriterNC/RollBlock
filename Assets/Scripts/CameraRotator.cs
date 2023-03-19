using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cam0;
    [SerializeField] private float rotationSpeed = 60f;

    private void Awake()
    {
        cam0 = transform.GetComponent<CinemachineVirtualCamera>();
    }

    private void FixedUpdate()
    {
        if (cam0.isActiveAndEnabled)
        {
            transform.eulerAngles += new Vector3(0, rotationSpeed * Time.deltaTime, 0);
        }
    }
}
