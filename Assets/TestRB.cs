using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRB : MonoBehaviour
{
    public Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.E))
        {
            FreezeRotationExcept("X");
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            FreezePositionExcept("X");
        }
    }

    void FreezeRotationExcept(string exceptAxis)
    {
        switch (exceptAxis)
        {
            case ("X"):
                rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                break;
            case ("Y"):
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
                break;
            case ("Z"):
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
                break;
        }
    }
    
    void FreezePositionExcept(string exceptAxis)
    {
        switch (exceptAxis)
        {
            case ("X"):
                rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
                break;
            case ("Y"):
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                break;
            case ("Z"):
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY;
                break;
        }
    }
}
