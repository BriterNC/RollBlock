using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private enum Directions
    {
        Left, //0
        Right //1
    }/*

    private enum Facing
    {
        Front, //0
        Right, //1
        Back, //2
        Left //3
    }*/
    
    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 offset;

    private void Awake()
    {
        offset = transform.position;
    }

    private void Update()
    {
        if (target.GetComponent<PlayerController>().firstLanded)
        {
            if (target.GetComponent<PlayerController>().isMoving)
            {
                transform.position = new Vector3(target.transform.localPosition.x + offset.x,Mathf.RoundToInt(target.transform.localPosition.y) + offset.y,target.transform.localPosition.z + offset.z);
            }
            else
            {
                transform.position = target.transform.localPosition + offset;
            }
        }
    }
}
