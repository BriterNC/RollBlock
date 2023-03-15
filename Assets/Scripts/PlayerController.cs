using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player Variables
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ConstantForce cf;
    [SerializeField] private bool isSpawning, isGroundInFront, isGroundOnLeft, isGroundOnBack, isGroundOnRight, isGroundOnUpperFront, isGroundOnUpperLeft, isGroundOnUpperBack, isGroundOnUpperRight, isSliding;
    [SerializeField] public bool isOnGround, isMoving, isElevating, isUsingItem;
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
    
    // Camera Variables
    [SerializeField] private CinemachineBrain mainCam;
    [SerializeField] private GameObject virtualCam1, virtualCam2, virtualCam3, virtualCam4;
    [SerializeField] private int activeCam = 1;
    [SerializeField] private bool isChangingCamera;
    private enum CameraRotation
    {
        Left, //0
        Right //1
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

    /*private void OnCollisionEnter()
    {
        // If any collision is detected then Player is now landed and Unfreeze
        if (Co)
        {
            UnlockFreeze();
        }
    }*/

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

    /*private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Ice"))
        {
            rb.mass = 0.1f;
            UnlockFreeze();
            isSliding = true;
            isMoving = true;
            Sliding();
        }
        else if (other.CompareTag("Elevator"))
        {
            StartCoroutine(Elevate());
        }
    }*/

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
        
        // 6 Sides axis raycast
        Vector3 ray1 = transform.TransformDirection(Vector3.right) * 0.5f;
        Vector3 ray2 = transform.TransformDirection(Vector3.left) * 0.5f;
        Vector3 ray3 = transform.TransformDirection(Vector3.forward) * 0.5f;
        Vector3 ray4 = transform.TransformDirection(Vector3.back) * 0.5f;
        Vector3 ray5 = transform.TransformDirection(Vector3.down) * 0.5f;
        Vector3 ray6 = transform.TransformDirection(Vector3.up) * 0.5f;
        Gizmos.DrawRay(pos, ray1); // Back
        Gizmos.DrawRay(pos, ray2); // Front
        Gizmos.DrawRay(pos, ray3); // Right
        Gizmos.DrawRay(pos, ray4); // Left
        Gizmos.DrawRay(pos, ray5); // Under
        Gizmos.DrawRay(pos, ray6); // Above
        
        // 4 Sides upper axis raycast
        Gizmos.DrawRay(new Vector3(pos.x, pos.y + 1, pos.z), ray1); // Upper Back
        Gizmos.DrawRay(new Vector3(pos.x, pos.y + 1, pos.z), ray2); // Upper Front
        Gizmos.DrawRay(new Vector3(pos.x, pos.y + 1, pos.z), ray3); // Upper Right
        Gizmos.DrawRay(new Vector3(pos.x, pos.y + 1, pos.z), ray4); // Upper Left
    }

    private void FixedUpdate()
    {
        // 6 Sides axis raycast
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, transform.right, out hit, 0.5f))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                isGroundOnBack = true;
            }
            else
            {
                isGroundOnBack = false;
            }
            /*
            else if (hit.transform.CompareTag("Ice"))
            {
                isGroundOnBack = true;
            }
            else if (hit.transform.CompareTag("Elevator"))
            {
                isGroundOnBack = false;
            }*/
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
            else
            {
                isGroundInFront = false;
            }
            /*
            else if (hit.transform.CompareTag("Ice"))
            {
                isGroundInFront = false;
            }
            else if (hit.transform.CompareTag("Elevator"))
            {
                isGroundInFront = false;
            }*/
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
            else
            {
                isGroundOnLeft = false;
            }/*
            else if (hit.transform.CompareTag("Ice"))
            {
                isGroundOnLeft = true;
            }
            else if (hit.transform.CompareTag("Elevator"))
            {
                isGroundOnLeft = false;
            }*/
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
            else
            {
                isGroundOnRight = false;
            }/*
            else if (hit.transform.CompareTag("Ice"))
            {
                isGroundOnRight = true;
            }
            else if (hit.transform.CompareTag("Elevator"))
            {
                isGroundOnRight = false;
            }*/
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
            else
            {
                isOnGround = false;
            }/*
            else if (hit.transform.CompareTag("Ice"))
            {
                isOnIce = true;
            }*/
        }
        else
        {
            isOnGround = false;
            //isOnIce = false;
        }
        
        // 4 Sides upper axis raycast
        
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.right, out hit, 0.5f))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                isGroundOnUpperBack = true;
            }
            else
            {
                isGroundOnUpperBack = false;
            }
        }
        else
        {
            isGroundOnUpperBack = false;
        }
        
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), -transform.right, out hit, 0.5f))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                isGroundOnUpperFront = true;
            }
            else
            {
                isGroundOnUpperFront = false;
            }
        }
        else
        {
            isGroundOnUpperFront = false;
        }
        
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), -transform.forward, out hit, 0.5f))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                isGroundOnUpperLeft = true;
            }
            else
            {
                isGroundOnUpperLeft = false;
            }
        }
        else
        {
            isGroundOnUpperLeft = false;
        }
        
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.forward, out hit, 0.5f))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                isGroundOnUpperRight = true;
            }
            else
            {
                isGroundOnUpperRight = false;
            }
        }
        else
        {
            isGroundOnUpperRight = false;
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
        if (isSpawning || isMoving || !isOnGround || isChangingCamera || isElevating)
        {
            return;
        }
        
        // Check if Key pressed WASD or Arrow keys for movement
        if ((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) && !isGroundInFront && !isGroundOnUpperFront)
        {
            FreezeRotationExcept("Z");
            lastDirection = KeyCode.W;
            StartCoroutine(Roll(Vector3.left));
        }
        else if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && !isGroundOnLeft && !isGroundOnUpperLeft)
        {
            FreezeRotationExcept("X");
            lastDirection = KeyCode.A;
            StartCoroutine(Roll(Vector3.back));
        }
        else if ((Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) && !isGroundOnBack && !isGroundOnUpperBack)
        {
            FreezeRotationExcept("Z");
            lastDirection = KeyCode.S;
            StartCoroutine(Roll(Vector3.right));
        }
        else if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && !isGroundOnRight && !isGroundOnUpperRight)
        {
            FreezeRotationExcept("X");
            lastDirection = KeyCode.D;
            StartCoroutine(Roll(Vector3.forward));
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            StartCoroutine(RotateCamera(CameraRotation.Left));
        }
        else if (Input.GetKey(KeyCode.E))
        {
            StartCoroutine(RotateCamera(CameraRotation.Right));
        }
        /*else if (Input.GetKey(KeyCode.Space) && !isUsingItem)
        {
            StartCoroutine(UseItem());
        }*/
    }

    private void ResetCube()
    {
        transform.localScale = Vector3.zero;
        currentItem = Items.None;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.inertiaTensor = Vector3.zero;
        rb.inertiaTensorRotation = Quaternion.Euler(Vector3.zero);
        rb.centerOfMass = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.localPosition = Vector3.zero;
        StartCoroutine(SpawnCube());
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
        isSpawning = true;
        
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
        isSpawning = false;
        
        UnlockFreeze();
    }
    
    // Function to make player roll
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
    
    /*private IEnumerator UseItem()
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
    }*/

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
    
    // Function to rotate side camera
    private IEnumerator RotateCamera(CameraRotation side)
    {
        isChangingCamera = true;
        
        var timeToRotate = 1f;
        var timer = 0f;
        
        virtualCam1.SetActive(false);
        virtualCam2.SetActive(false);
        virtualCam3.SetActive(false);
        virtualCam4.SetActive(false);
        
        if (side == CameraRotation.Left)
        {
            switch (activeCam)
            {
                case(1):
                    virtualCam2.SetActive(true);
                    activeCam = 2;
                    break;
                case(2):
                    virtualCam3.SetActive(true);
                    activeCam = 3;
                    break;
                case(3):
                    virtualCam4.SetActive(true);
                    activeCam = 4;
                    break;
                case(4):
                    virtualCam1.SetActive(true);
                    activeCam = 1;
                    break;
            }
        }
        else if (side == CameraRotation.Right)
        {
            switch (activeCam)
            {
                case(1):
                    virtualCam4.SetActive(true);
                    activeCam = 4;
                    break;
                case(2):
                    virtualCam1.SetActive(true);
                    activeCam = 1;
                    break;
                case(3):
                    virtualCam2.SetActive(true);
                    activeCam = 2;
                    break;
                case(4):
                    virtualCam3.SetActive(true);
                    activeCam = 3;
                    break;
            }
        }
        
        while (timer < timeToRotate)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        isChangingCamera = false;
    }

    /*private IEnumerator Elevate()
    {
        if (!isMoving)
        {
            isElevating = true;
        
            var timer = 0f;
            var timeToElevate = 0.5f;
            var tempPosition = transform.localPosition.y;

            rb.useGravity = false;
        
            while (timer < timeToElevate)
            {
                timer += Time.deltaTime;
                transform.localPosition = new Vector3(transform.localPosition.x, tempPosition + (timer / timeToElevate), transform.localPosition.z);
                yield return null;
            }

            rb.useGravity = true;
        
            isElevating = false;
        }
    }*/
}
