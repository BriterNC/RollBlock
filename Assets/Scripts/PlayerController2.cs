using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    // THIS SCRIPT IS USE TO TEST PLAYER CONTROLLER BUGS & IS DESIGNED TO BE EASY-TO-READ FORMAT
    
    // Spawning Variables
    public Rigidbody rb;
    public bool isSpawned;
    public float timeToSpawn = 0.2f;
    public Vector3 playerLocalScale, fixedPlayerLocalScale;
    public Quaternion playerLocalRotation;
    
    // Rotating Variables
    public bool isRolling;
    public float angularVelocity = 300f;
    public enum Directions
    {
        Forward,
        Backward,
        Left,
        Right
    }
    
    // Snapping Variables
    public Vector3 playerLocalPosition;
    public Directions lastDirection;
    
    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerLocalScale = transform.localScale;
        fixedPlayerLocalScale = new Vector3(playerLocalScale.x, playerLocalScale.y, playerLocalScale.z);
        playerLocalScale = Vector3.zero;
        playerLocalPosition = transform.localPosition;
        playerLocalRotation = transform.localRotation;
    }

    public void Start()
    {
        StartCoroutine(SpawnPlayer());
    }

    public void FixedUpdate()
    {
        /*ray = new Ray(transform.localPosition, Vector3.up);
        Debug.DrawLine(ray.origin, hit.point, Color.red);*/
        
        // Check if Player spawned
        if (!isSpawned)
        {
            return;
        }
        
        // Check if Player rotating
        if (isRolling)
        {
            return;
        }
        
        // Check Input R
        if (Input.GetKey(KeyCode.R))
        {
            StartCoroutine(SpawnPlayer());
        }
        
        // Check Input Axis
        if (Input.GetAxis("Vertical") > 0)
        {
            FreezeRotationExcept("Z");
            StartCoroutine(Rotate(Directions.Forward));
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            FreezeRotationExcept("Z");
            StartCoroutine(Rotate(Directions.Backward));
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            FreezeRotationExcept("X");
            StartCoroutine(Rotate(Directions.Right));
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            FreezeRotationExcept("X");
            StartCoroutine(Rotate(Directions.Left));
        }
    }

    public IEnumerator SpawnPlayer()
    {
        Debug.Log("Spawning Player");
            
        var timer = 0f;

        while (timer <= timeToSpawn)
        {
            timer += Time.deltaTime;
            playerLocalScale = new Vector3(fixedPlayerLocalScale.x * (timer / timeToSpawn), fixedPlayerLocalScale.y * (timer / timeToSpawn), fixedPlayerLocalScale.z * (timer / timeToSpawn));
            yield return null;
        }

        playerLocalScale = fixedPlayerLocalScale;
        rb.useGravity = true;
        isSpawned = true;
    }

    public IEnumerator Rotate(Directions direction)
    {
        Debug.Log($"Rotating Player to {direction}");
        isRolling = true;
        var angleToRotate = 90f;
            
        Vector3 directionVector = Vector3.zero;
            
        // Fixing Direction Input Enum
        switch ((int)direction)
        {
            // Forward
            case (0):
                directionVector = Vector3.left; 
                break;
            // Backward
            case (1): 
                directionVector = Vector3.right; 
                break; 
            // Right
            case (2):
                directionVector = Vector3.back;
                break;
            // Left
            case (3):
                directionVector = Vector3.forward;
                break;
        }
            
        var rotationCenter = transform.position + directionVector / 2 + Vector3.down / 2;
        var rotationAxis = Vector3.Cross(Vector3.up, directionVector);
            
        while (angleToRotate > 0)
        {
            var rotateAngle = Mathf.Min(Time.deltaTime * angularVelocity, angleToRotate);
            transform.RotateAround(rotationCenter, rotationAxis, rotateAngle);
            angleToRotate -= rotateAngle;
            yield return null;
        }
            
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        rb.inertiaTensor = Vector3.zero;
        rb.inertiaTensorRotation = Quaternion.Euler(Vector3.zero);
        rb.centerOfMass = Vector3.zero;
        
        playerLocalRotation = Quaternion.Euler(Vector3.zero);

        //SnapPlayerToGrid();
        playerLocalPosition = new Vector3(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y), Mathf.RoundToInt(transform.localPosition.z));

        UnfreezePlayerPositionAndRotation();
            
        isRolling = false;
    }

    public void UnfreezePlayerPositionAndRotation()
    {
        Debug.Log($"Unfreezing Player");
        rb.constraints = RigidbodyConstraints.None;
    }

    public void FreezePlayerPositionAndRotation()
    {
        Debug.Log($"Freezing Player except Y Position");
        // Freeze All Except Position Y To Make Object Falling
        rb.constraints = RigidbodyConstraints.FreezeRotation |
                         RigidbodyConstraints.FreezePositionX |
                         RigidbodyConstraints.FreezePositionZ;
    }
    
    private void FreezeRotationExcept(string exceptAxis)
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
    
    // For Sliding
    public void FreezePositionExcept(string exceptAxis)
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

    public void SnapPlayerToGrid()
    {
        if (!isRolling)
        {
            Debug.Log($"Snapping Player to Grid");
            //playerLocalPosition = new Vector3(Mathf.RoundToInt(playerLocalPosition.x), Mathf.RoundToInt(playerLocalPosition.y), Mathf.RoundToInt(playerLocalPosition.z));
            playerLocalPosition = new Vector3Int(Mathf.RoundToInt(playerLocalPosition.x), Mathf.RoundToInt(playerLocalPosition.y), Mathf.RoundToInt(playerLocalPosition.z));
        }
    }
}
