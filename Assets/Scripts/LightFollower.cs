using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFollower : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private Vector3 offset;
    
    void Awake()
    {
        offset = transform.position;
    }
    
    void Update()
    {
        transform.position = target.transform.localPosition + offset;
    }
}
