using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player Variables
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ConstantForce cf;
    [SerializeField] private bool isSpawned, isGroundInFront, isGroundOnLeft, isGroundOnBack, isGroundOnRight, isSliding;
    [SerializeField] public bool firstLanded, isOnGround, isOnIce, isMoving, isUsingItem;
    [SerializeField] private float playerScale = 0.9f;
    
    // Rotating Variables
    [SerializeField] private int angularVelocity = 300;
    private enum Directions
    {
        Forward,
        Backward,
        Left,
        Right
    }
    
    // Sliding Variables
    [SerializeField] public KeyCode lastDirection;
    
    // Item Variables
    [SerializeField] private Items currentItem;
    private enum Items
    {
        None, //0
        Jumper //1
    }

    private void Awake()
    {
        // Get Components from Player Cube then Spawn Player
        rb = GetComponent<Rigidbody>();
        cf = GetComponent<ConstantForce>();
        transform.localScale = Vector3.zero;
        currentItem = Items.None;
        StartCoroutine(SpawnCube());
    }

    private void OnCollisionEnter()
    {
        // If any collision is detected then Player is now landed and Unfreeze
        if (!firstLanded)
        {
            firstLanded = true;
            UnlockFreeze();
        }
    }

    // BUGGY AREA START
    // Check ICE trigger for slipper movement
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ice"))
        {
            rb.mass = 0.01f;
            UnlockFreeze();
            isSliding = true;
            isMoving = true;
            Sliding();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ice"))
        {
            rb.mass = 0.1f;
            UnlockFreeze();
            isSliding = true;
            isMoving = true;
            Sliding();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ice"))
        {
            rb.mass = 1f;
            cf.force = Vector3.zero;
            rb.velocity = Vector3.zero;
            UnlockFreeze();
            isSliding = false;
            rb.angularVelocity = Vector3.zero;
            rb.inertiaTensor = Vector3.zero;
            rb.inertiaTensorRotation = Quaternion.Euler(Vector3.zero);
            rb.centerOfMass = Vector3.zero;
            transform.rotation = Quaternion.Euler(Vector3.zero);
            transform.localPosition = new Vector3(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y), Mathf.RoundToInt(transform.localPosition.z));
            isMoving = false;
        }
    }
    // BUGGY AREA END
    
    // Drawing Raycast on Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var pos = transform.position;
            
        Vector3 ray1 = transform.TransformDirection(Vector3.right) * 0.5f;
        Vector3 ray2 = transform.TransformDirection(Vector3.left) * 0.5f;
        Vector3 ray3 = transform.TransformDirection(Vector3.forward) * 0.5f;
        Vector3 ray4 = transform.TransformDirection(Vector3.back) * 0.5f;
        Vector3 ray5 = transform.TransformDirection(Vector3.down) * 1f;
        Vector3 ray6 = transform.TransformDirection(Vector3.up) * 0.5f;
        Gizmos.DrawRay(pos, ray1);
        Gizmos.DrawRay(pos, ray2);
        Gizmos.DrawRay(pos, ray3);
        Gizmos.DrawRay(pos, ray4);
        Gizmos.DrawRay(pos, ray5);
        Gizmos.DrawRay(pos, ray5);
        Gizmos.DrawRay(pos, ray6);
    }

    private void FixedUpdate()
    {
        // Check if player is already spawned
        if (!isSpawned)
        {
            return;
        }
        
        // Casting Raycast
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, transform.right, out hit, 0.5f))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                isGroundOnBack = true;
            }
        }
        else
        {
            isGroundOnBack = false;
        }
        
        if (Physics.Raycast(transform.position, -transform.right, out hit, 0.5f))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                isGroundInFront = true;
            }
        }
        else
        {
            isGroundInFront = false;
        }
        
        if (Physics.Raycast(transform.position, -transform.forward, out hit, 0.5f))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                isGroundOnLeft = true;
            }
            if (hit.transform.CompareTag("Ice"))
            {
                Debug.Log("ICE ON LEFT!");
            }
        }
        else
        {
            isGroundOnLeft = false;
        }
        
        if (Physics.Raycast(transform.position, transform.forward, out hit, 0.5f))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                isGroundOnRight = true;
            }
        }
        else
        {
            isGroundOnRight = false;
        }
        
        if (Physics.Raycast(transform.position, -transform.up, out hit, 0.5f))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                isOnGround = true;
            }
            else if (hit.transform.CompareTag("Ice"))
            {
                isOnIce = true;
            }
        }
        else
        {
            isOnGround = false;
            isOnIce = false;
        }
        
        // Check if player is landed
        if (!firstLanded)
        {
            return;
        }
        
        // Check if the game is paused
        if (GameObject.FindWithTag("GameController").GetComponent<GameController>().isPaused)
        {
            return;
        }
        
        // Check if Key R is Pressed
        if (Input.GetKey(KeyCode.R))
        {
            ResetCube();
        }
        
        // Check if player is moving or isn't on ground
        if (isMoving || !isOnGround)
        {
            return;
        }
        
        // Check if Key pressed WASD or Arrow keys for movement
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && !isGroundInFront)
        {
            FreezeRotationExcept("Z");
            lastDirection = KeyCode.W;
            StartCoroutine(Roll(Vector3.left));
        }
        else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && !isGroundOnLeft)
        {
            FreezeRotationExcept("X");
            lastDirection = KeyCode.A;
            StartCoroutine(Roll(Vector3.back));
        }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && !isGroundOnBack)
        {
            FreezeRotationExcept("Z");
            lastDirection = KeyCode.S;
            StartCoroutine(Roll(Vector3.right));
        }
        else if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && !isGroundOnRight)
        {
            FreezeRotationExcept("X");
            lastDirection = KeyCode.D;
            StartCoroutine(Roll(Vector3.forward));
        }
        else if (Input.GetKey(KeyCode.Space) && !isUsingItem)
        {
            StartCoroutine(UseItem());
        }
    }

    private void ResetCube()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.inertiaTensor = Vector3.zero;
        rb.inertiaTensorRotation = Quaternion.Euler(Vector3.zero);
        rb.centerOfMass = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.localPosition = Vector3.zero;
    }

    private void FreezeRotation()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
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
    
    private void UnlockFreeze()
    {
        rb.constraints = RigidbodyConstraints.None;
    }

    private IEnumerator SpawnCube()
    {
        var secondToSpawn = 1f;
        var currentTimer = 0f;
        
        while (currentTimer <= secondToSpawn)
        {
            currentTimer += Time.deltaTime;
            transform.localScale = new Vector3(currentTimer * playerScale, currentTimer * playerScale, currentTimer * playerScale);
            yield return null;
        }

        transform.localScale = new Vector3(playerScale, playerScale, playerScale);
        rb.useGravity = true;
        isSpawned = true;
    }
    
    private IEnumerator Roll(Vector3 direction)
    {
        isMoving = true;
        var angleToRotate = 90f;
        
        Vector3 rotationCenter = transform.position + direction / 2 + Vector3.down / 2;
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction);

        while (angleToRotate > 0)
        {
            float rotationAngle = Mathf.Min(Time.deltaTime * angularVelocity, angleToRotate);
            transform.RotateAround(rotationCenter, rotationAxis, rotationAngle);
            angleToRotate -= rotationAngle;
            yield return null;
        }
        
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        if (!isSliding)
        {
            rb.inertiaTensor = Vector3.zero;
            rb.inertiaTensorRotation = Quaternion.Euler(Vector3.zero);
            rb.centerOfMass = Vector3.zero;
            transform.rotation = Quaternion.Euler(Vector3.zero);
            transform.localPosition = new Vector3(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y), Mathf.RoundToInt(transform.localPosition.z));
            UnlockFreeze();
            isMoving = false;
        }
    }

    private void Sliding()
    {
        Vector3 direction = Vector3.zero;
            
        switch (lastDirection)
        {
            case (KeyCode.W):
                direction = Vector3.left;
                FreezePositionExcept("X");
                break;
            case (KeyCode.A):
                direction = Vector3.back;
                FreezePositionExcept("Z");
                break;
            case (KeyCode.S):
                direction = Vector3.right;
                FreezePositionExcept("X");
                break;
            case (KeyCode.D):
                direction = Vector3.forward;
                FreezePositionExcept("Z");
                break;
        }
        
        FreezeRotation();
        cf.force = direction * 15f;
    }
    
    private IEnumerator UseItem()
    {
        isUsingItem = true;
        
        var timer = 0f;
        var cooldownSeconds = 0.5f;
        
        switch (currentItem)
        {
            case (Items.None):
                Debug.Log("No Item Owned");
                break;
            case (Items.Jumper):
                Debug.Log("Item Jumper Will Be Used");
                break;
        }
        
        while (timer < cooldownSeconds)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        currentItem = Items.None;
        isUsingItem = false;
    }

    private void FreezePositionExcept(string exceptAxis)
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
