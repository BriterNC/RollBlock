using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Player Variables
    [SerializeField] private Rigidbody rb;
    [SerializeField] private ConstantForce cf;
    [SerializeField] private bool
        isSpawning,
        isGroundOnFront,
        isGroundOnLeft,
        isGroundOnBack,
        isGroundOnRight,
        isGroundOnUpperFront,
        isGroundOnUpperLeft,
        isGroundOnUpperBack,
        isGroundOnUpperRight,
        isGroundOnAbove,
        isOnGround,
        isSliding,
        isFalling,
        isRolling,
        groundIsIce,
        isUsingItem;
    [SerializeField] public float playerScale = 0.9f;
    [SerializeField] private int rollCount;
    [SerializeField] private TMP_Text rollCountText;
    
    // Raycast Variables
    [SerializeField] private Transform raycastLocker;
    [SerializeField] private RaycastHit _hit;
    [SerializeField] private Vector3 positionRoundedYAxis, positionRoundedYUpperAxis;
    [SerializeField] private Vector3 ray1, ray2, ray3, ray4, ray5, ray6;
    
    // Rotating Variables
    [SerializeField] private int angularVelocity = 300;
    [SerializeField] private int initialedRollOnCam;
    [SerializeField] private bool camSwitchWhileSliding;
    private enum Directions
    {
        None,
        Forward,
        Backward,
        Left,
        Right
    }
    
    // Sliding Variables
    [SerializeField] private Directions lastDirection = Directions.None;
    //[SerializeField] private FixedAxisDirections lockedDirection = FixedAxisDirections.None;
    /*private enum FixedAxisDirections
    {
        None,
        XForward,
        XBackward,
        ZForward,
        ZBackward
    }*/
    [SerializeField] private float slidingSpeed = 10f;
    [SerializeField] private bool disabledMovementInput;
    
    // Item Variables
    [SerializeField] private Items currentItem;
    [SerializeField] private GameObject teleporter1, teleporter2;
    [SerializeField] private bool teleporter1IsActive, teleporter2IsActive;
    private enum Items
    {
        None, //0
        Teleporter //1
    }
    
    // Camera Variables
    [SerializeField] private CinemachineBrain mainCam;
    [SerializeField] private GameObject virtualCam1, virtualCam2, virtualCam3, virtualCam4;
    [SerializeField] private int activeCam = 1;
    [SerializeField] private bool isChangingCamera;
    private enum CameraRotation
    {
        Left,
        Right
    }
    
    // Rigidbody & ConstantForce & Item & Raycast Setter Initializer
    private void Awake()
    {
        // Set Raycast direction based on RaycastLocker (prevent rotation)
        ray1 = raycastLocker.TransformDirection(Vector3.right) * 0.5f;
        ray2 = raycastLocker.TransformDirection(Vector3.left) * 0.5f;
        ray3 = raycastLocker.TransformDirection(Vector3.forward) * 0.5f;
        ray4 = raycastLocker.TransformDirection(Vector3.back) * 0.5f;
        ray5 = raycastLocker.TransformDirection(Vector3.down) * 0.5f;
        ray6 = raycastLocker.TransformDirection(Vector3.up) * 0.5f;
        
        // Get Components from Player Cube then Spawn Player
        rb = GetComponent<Rigidbody>();
        cf = GetComponent<ConstantForce>();
        transform.localScale = Vector3.zero;
        currentItem = Items.None;
        StartCoroutine(SpawnCube(Vector3.zero));
    }
    
    // Check Ice / SlipStopper trigger for slippery movement
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fallout"))
        {
            ResetCube();
            return;
        }
        
        if (other.CompareTag("Teleporter"))
        {
            currentItem = Items.Teleporter;
            if (other.gameObject == teleporter1)
            {
                Debug.Log("Teleport 1 Activated");
                teleporter1IsActive = true;
            }
            else if (other.gameObject == teleporter2)
            {
                Debug.Log("Teleport 2 Activated");
                teleporter2IsActive = true;
            }
        }
        
        if (other.CompareTag("Ice"))
        {
            if (!isFalling)
            {
                disabledMovementInput = true;
            }
            groundIsIce = true;
            /*if (lockedDirection == FixedAxisDirections.None)
            {
                Vector3 pos = transform.position;
                Vector3 pos2;
                float resultX;
                float resultZ;
                
                var limittime = 0.2f;
                var timer = 0f;

                while (timer < limittime)
                {
                    timer += Time.deltaTime;
                }

                pos2 = transform.position;

                if (pos2.x - pos.x > 0)
                {
                    Debug.Log("The X is bigger than 0");
                }
                else if (pos2.x - pos.x < 0)
                {
                    Debug.Log("The X is smaller than 0");
                }

                if (pos2.z - pos.z > 0)
                {
                    Debug.Log("The Z is bigger than 0");
                }
                else if (pos2.z - pos.z < 0)
                {
                    Debug.Log("The Z is smaller than 0");
                }
                
                resultX = Mathf.Abs(pos2.x - pos.x);
                resultZ = Mathf.Abs(pos2.z - pos.z);

                if (resultX > resultZ)
                {
                    Debug.Log("The object is moving in X axis");
                }
                else if (resultX > resultZ)
                {
                    Debug.Log("The object is moving in Z axis");
                }
                
                //lockedDirection = lastDirection;
            }*/
        }
        else if (other.CompareTag("SlipStopper"))
        {
            if (groundIsIce && isSliding)
            {
                isSliding = false;
                cf.force = Vector3.zero;
                rb.velocity = Vector3.zero;
                rb.useGravity = true;
                transform.localPosition = new Vector3(Mathf.RoundToInt(transform.localPosition.x),
                    Mathf.RoundToInt(transform.localPosition.y), Mathf.RoundToInt(transform.localPosition.z));
            }

            if (isGroundOnAbove)
            {
                rb.constraints = RigidbodyConstraints.FreezeAll;
                groundIsIce = true;
                isSliding = false;
                cf.force = Vector3.zero;
                rb.velocity = Vector3.zero;
                rb.useGravity = true;
                transform.localPosition = new Vector3(Mathf.RoundToInt(transform.localPosition.x),
                    Mathf.RoundToInt(transform.localPosition.y), Mathf.RoundToInt(transform.localPosition.z));
                return;
            }

            // Welcome to buggy area :')

            if (!camSwitchWhileSliding)
            {
                GetMovementWithActiveCamera(lastDirection);
            }
            
            // Too buggy to fix
            /*else
            {
                if (isRolling)
                {
                    return;
                }

                switch (lockedDirection)
                {
                    case (FixedAxisDirections.XBackward):
                        StartCoroutine(Roll(Vector3.left));
                        break;
                    case (FixedAxisDirections.XForward):
                        StartCoroutine(Roll(Vector3.right));
                        break;
                    case (FixedAxisDirections.ZBackward):
                        StartCoroutine(Roll(Vector3.back));
                        break;
                    case (FixedAxisDirections.ZForward):
                        StartCoroutine(Roll(Vector3.forward));
                        break;
                }
            }*/

            isSliding = false;
            UnlockAllFreeze();
            groundIsIce = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Teleporter"))
        {
            currentItem = Items.None;
            if (other.gameObject == teleporter1)
            {
                Debug.Log("Teleport 1 Activated");
                teleporter1IsActive = false;
            }
            else if (other.gameObject == teleporter2)
            {
                Debug.Log("Teleport 2 Activated");
                teleporter2IsActive = false;
            }
        }
    }

    // Drawing Raycast on Gizmos in Editor
    private void OnDrawGizmos()
    {
        // 6 Sides axis raycast
        Gizmos.color = Color.red;
        Gizmos.DrawRay(positionRoundedYAxis, ray1); // Back
        Gizmos.DrawRay(positionRoundedYAxis, ray2); // Front
        Gizmos.DrawRay(positionRoundedYAxis, ray3); // Right
        Gizmos.DrawRay(positionRoundedYAxis, ray4); // Left
        Gizmos.DrawRay(positionRoundedYAxis, ray5); // Under
        Gizmos.DrawRay(positionRoundedYAxis, ray6); // Above
        
        // 4 Sides upper axis raycast
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(positionRoundedYUpperAxis, ray1); // Upper Back
        Gizmos.DrawRay(positionRoundedYUpperAxis, ray2); // Upper Front
        Gizmos.DrawRay(positionRoundedYUpperAxis, ray3); // Upper Right
        Gizmos.DrawRay(positionRoundedYUpperAxis, ray4); // Upper Left
    }

    private void FixedUpdate()
    {
        // Drawing Raycast on in Game Scene
        
        // Prevent Raycast from juggling
        if (!isFalling)
        {
            positionRoundedYAxis = new Vector3(transform.position.x, Mathf.RoundToInt(transform.position.y), transform.position.z);
            positionRoundedYUpperAxis = new Vector3(positionRoundedYAxis.x, positionRoundedYAxis.y + 1, positionRoundedYAxis.z);
        }
        else if (isFalling)
        {
            positionRoundedYAxis = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            positionRoundedYUpperAxis = new Vector3(positionRoundedYAxis.x, positionRoundedYAxis.y + 1, positionRoundedYAxis.z);
        }
        
        // 6 Sides axis raycast
        
        if (Physics.Raycast(positionRoundedYAxis, ray1, out _hit, 0.5f))
        {
            if (_hit.transform.CompareTag("Ground"))
            {
                isGroundOnBack = true;
            }
            else
            {
                isGroundOnBack = false;
            }
        }
        else
        {
            isGroundOnBack = false;
        }
        
        if (Physics.Raycast(positionRoundedYAxis, ray2, out _hit, 0.5f))
        {
            if (_hit.transform.CompareTag("Ground"))
            {
                isGroundOnFront = true;
            }
            else
            {
                isGroundOnFront = false;
            }
        }
        else
        {
            isGroundOnFront = false;
        }
        
        if (Physics.Raycast(positionRoundedYAxis, ray3, out _hit, 0.5f))
        {
            if (_hit.transform.CompareTag("Ground"))
            {
                isGroundOnRight = true;
            }
            else
            {
                isGroundOnRight = false;
            }
        }
        else
        {
            isGroundOnRight = false;
        }
        
        if (Physics.Raycast(positionRoundedYAxis, ray4, out _hit, 0.5f))
        {
            if (_hit.transform.CompareTag("Ground"))
            {
                isGroundOnLeft = true;
            }
            else
            {
                isGroundOnLeft = false;
            }
        }
        else
        {
            isGroundOnLeft = false;
        }
        
        if (Physics.Raycast(positionRoundedYAxis, ray5, out _hit, 0.5f))
        {
            if (_hit.transform.CompareTag("Ground"))
            {
                isOnGround = true;
                isFalling = false;
            }
            else
            {
                isOnGround = false;
                isFalling = true;
                lastDirection = Directions.None;
            }
        }
        else
        {
            isOnGround = false;
            isFalling = true;
            lastDirection = Directions.None;
        }
        
        if (Physics.Raycast(positionRoundedYAxis, ray6, out _hit, 0.5f))
        {
            if (_hit.transform.CompareTag("Ground"))
            {
                isGroundOnAbove = true;
            }
            else
            {
                isGroundOnAbove = false;
            }
        }
        else
        {
            isGroundOnAbove = false;
        }
        
        // 4 Sides upper axis raycast
        
        if (Physics.Raycast(positionRoundedYUpperAxis, ray1, out _hit, 0.5f))
        {
            if (_hit.transform.CompareTag("Ground"))
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
        
        if (Physics.Raycast(positionRoundedYUpperAxis, ray2, out _hit, 0.5f))
        {
            if (_hit.transform.CompareTag("Ground"))
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
        
        if (Physics.Raycast(positionRoundedYUpperAxis, ray3, out _hit, 0.5f))
        {
            if (_hit.transform.CompareTag("Ground"))
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
        
        if (Physics.Raycast(positionRoundedYUpperAxis, ray4, out _hit, 0.5f))
        {
            if (_hit.transform.CompareTag("Ground"))
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

        // Check if the game is paused
        if (GameObject.FindWithTag("GameController").GetComponent<GameController>().isPaused)
        {
            return;
        }

        // Check if player is sliding or not
        if (!isRolling && groundIsIce)
        {
            CheckIfToSlide();
        }
        
        // Check if Key R is Pressed to Reset Player
        if (Input.GetKey(KeyCode.R) && !isRolling)
        {
            if (isSpawning)
            {
                return;
            }
            ResetCube();
        }
        
        
        // Check if player is spawning / rolling / changing camera or not
        if (isChangingCamera)
        {
            return;
        }

        // Check if Key pressed = WASD / Arrow keys, then roll the cube (along side to the current camera)
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            if (isGroundOnAbove || isSliding || isFalling || isSpawning || isRolling || disabledMovementInput)
            {
                return;
            }
            switch (activeCam)
            {
                case (1):
                    if (!isGroundOnFront && !isGroundOnUpperFront)
                    {
                        GetMovementWithActiveCamera(Directions.Forward);
                    }
                    break;
                case (2):
                    if (!isGroundOnRight && !isGroundOnUpperRight)
                    {
                        GetMovementWithActiveCamera(Directions.Forward);
                    }
                    break;
                case (3):
                    if (!isGroundOnBack && !isGroundOnUpperBack)
                    {
                        GetMovementWithActiveCamera(Directions.Forward);
                    }
                    break;
                case (4):
                    if (!isGroundOnLeft && !isGroundOnUpperLeft)
                    {
                        GetMovementWithActiveCamera(Directions.Forward);
                    }
                    break;
            }
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            if (isGroundOnAbove || isSliding || isFalling || isSpawning || isRolling || disabledMovementInput)
            {
                return;
            }
            switch (activeCam)
            {
                case (1):
                    if (!isGroundOnBack && !isGroundOnUpperBack)
                    {
                        GetMovementWithActiveCamera(Directions.Backward);
                    }
                    break;
                case (2):
                    if (!isGroundOnLeft && !isGroundOnUpperLeft)
                    {
                        GetMovementWithActiveCamera(Directions.Backward);
                    }
                    break;
                case (3):
                    if (!isGroundOnFront && !isGroundOnUpperFront)
                    {
                        GetMovementWithActiveCamera(Directions.Backward);
                    }
                    break;
                case (4):
                    if (!isGroundOnRight && !isGroundOnUpperRight)
                    {
                        GetMovementWithActiveCamera(Directions.Backward);
                    }
                    break;
            }
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (isGroundOnAbove || isSliding || isFalling || isSpawning || isRolling || disabledMovementInput)
            {
                return;
            }
            switch (activeCam)
            {
                case (1):
                    if (!isGroundOnLeft && !isGroundOnUpperLeft)
                    {
                        GetMovementWithActiveCamera(Directions.Left);
                    }
                    break;
                case (2):
                    if (!isGroundOnFront && !isGroundOnUpperFront)
                    {
                        GetMovementWithActiveCamera(Directions.Left);
                    }
                    break;
                case (3):
                    if (!isGroundOnRight && !isGroundOnUpperRight)
                    {
                        GetMovementWithActiveCamera(Directions.Left);
                    }
                    break;
                case (4):
                    if (!isGroundOnBack && !isGroundOnUpperBack)
                    {
                        GetMovementWithActiveCamera(Directions.Left);
                    }
                    break;
            }
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (isGroundOnAbove || isSliding || isFalling || isSpawning || isRolling || disabledMovementInput)
            {
                return;
            }
            switch (activeCam)
            {
                case (1):
                    if (!isGroundOnRight && !isGroundOnUpperRight)
                    {
                        GetMovementWithActiveCamera(Directions.Right);
                    }
                    break;
                case (2):
                    if (!isGroundOnBack && !isGroundOnUpperBack)
                    {
                        GetMovementWithActiveCamera(Directions.Right);
                    }
                    break;
                case (3):
                    if (!isGroundOnLeft && !isGroundOnUpperLeft)
                    {
                        GetMovementWithActiveCamera(Directions.Right);
                    }
                    break;
                case (4):
                    if (!isGroundOnFront && !isGroundOnUpperFront)
                    {
                        GetMovementWithActiveCamera(Directions.Right);
                    }
                    break;
            }
        }
        
        // Check if Key pressed = Q / E, then switching camera
        else if (Input.GetKey(KeyCode.Q) && !isRolling)
        {
            if (isSliding)
            {
                return; //Temporary
            }
            if (initialedRollOnCam == 0)
            {
                initialedRollOnCam = activeCam;
            }
            StartCoroutine(RotateCamera(CameraRotation.Left));
        }
        else if (Input.GetKey(KeyCode.E) && !isRolling)
        {
            if (isSliding)
            {
                return; //Temporary
            }
            if (initialedRollOnCam == 0)
            {
                initialedRollOnCam = activeCam;
            }
            StartCoroutine(RotateCamera(CameraRotation.Right));
        }
        
        // Check if Key pressed = Space bar, then using item
        else if (Input.GetKey(KeyCode.Space))
        {
            if (isRolling || isUsingItem)
            {
                return;
            }
            StartCoroutine(UseItem());
        }
    }

    private void ResetCube()
    {
        rollCount = 0;
        transform.localScale = Vector3.zero;
        disabledMovementInput = false;
        currentItem = Items.None;
        isRolling = false;
        isSliding = false;
        isOnGround = false;
        groundIsIce = false;
        cf.force = Vector3.zero;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        StartCoroutine(SpawnCube(Vector3.zero));
    }
    
    private void TeleportCube(Vector3 position)
    {
        transform.localScale = Vector3.zero;
        disabledMovementInput = false;
        isRolling = false;
        isSliding = false;
        isOnGround = false;
        groundIsIce = false;
        cf.force = Vector3.zero;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        StartCoroutine(SpawnCube(position));
    }

    private void FreezeRotation()
    {
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }
    
    private void UnlockAllFreeze()
    {
        rb.constraints = RigidbodyConstraints.None;
    }

    private IEnumerator SpawnCube(Vector3 position)
    {
        isSpawning = true;

        transform.localPosition = position;

        UpdateRollCount(rollCount);
        
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
        
        UnlockAllFreeze();
    }
    
    // Function to make player rolling
    private IEnumerator Roll(Vector3 direction)
    {
        if (!isOnGround)
        {
            Debug.Log("You can't move while there's no ground!");
            yield break;
        }

        if (isSliding)
        {
            Debug.Log("You can't move while sliding!");
            yield break;
        }
        
        initialedRollOnCam = activeCam;
        isRolling = true;

        var angleToRotate = 90f;
        
        Vector3 rotationCenter = transform.position + direction / 2 + Vector3.down / 2;
        Vector3 rotationAxis = Vector3.Cross(Vector3.up, direction);

        while (angleToRotate > 0)
        {
            float rotationAngle = Mathf.Min(Time.deltaTime * angularVelocity, angleToRotate);
            angleToRotate -= rotationAngle;
            transform.RotateAround(rotationCenter, rotationAxis, rotationAngle);
            yield return null;
        }

        rollCount++;
        UpdateRollCount(rollCount);

        if (groundIsIce)
        {
            isRolling = false;
        }
        else
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.inertiaTensor = Vector3.zero;
            rb.inertiaTensorRotation = Quaternion.Euler(Vector3.zero);
        
            transform.rotation = Quaternion.Euler(Vector3.zero);
            transform.localPosition = new Vector3(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y), Mathf.RoundToInt(transform.localPosition.z));
        
            initialedRollOnCam = 0;
            isRolling = false;
        }
        
        disabledMovementInput = false;
        camSwitchWhileSliding = false;
        //lockedDirection = FixedAxisDirections.None;
    }

    private void CheckIfToSlide()
    {
        if (isSliding)
        {
            return;
        }
        
        if (lastDirection != Directions.None)
        {
            FreezeRotation();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezePositionY;
            
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.02f, transform.localPosition.z);
            
            isSliding = true;
            
            switch (initialedRollOnCam)
            {
                case (1):
                    if (lastDirection == Directions.Forward)
                    {
                        cf.force = Vector3.left * slidingSpeed;
                        //lockedDirection = FixedAxisDirections.XBackward;
                    }
                    else if (lastDirection == Directions.Backward)
                    {
                        cf.force = Vector3.right * slidingSpeed;
                        //lockedDirection = FixedAxisDirections.XForward;
                    }
                    else if (lastDirection == Directions.Right)
                    {
                        cf.force = Vector3.forward * slidingSpeed;
                        //lockedDirection = FixedAxisDirections.ZForward;
                    }
                    else if (lastDirection == Directions.Left)
                    {
                        cf.force = Vector3.back * slidingSpeed;
                        //lockedDirection = FixedAxisDirections.ZBackward;
                    }
                    break;
                case (2):
                    if (lastDirection == Directions.Forward)
                    {
                        cf.force = Vector3.forward * slidingSpeed;
                        //lockedDirection = FixedAxisDirections.ZForward;
                    }
                    else if (lastDirection == Directions.Backward)
                    {
                        cf.force = Vector3.back * slidingSpeed;
                        //lockedDirection = FixedAxisDirections.ZBackward;
                    }
                    else if (lastDirection == Directions.Right)
                    {
                        cf.force = Vector3.right * slidingSpeed;
                        //lockedDirection = FixedAxisDirections.XForward;
                    }
                    else if (lastDirection == Directions.Left)
                    {
                        cf.force = Vector3.left * slidingSpeed;
                        //lockedDirection = FixedAxisDirections.XBackward;
                    }
                    break;
                case (3):
                    if (lastDirection == Directions.Forward)
                    {
                        cf.force = Vector3.right * slidingSpeed;
                        //lockedDirection = FixedAxisDirections.XForward;
                    }
                    else if (lastDirection == Directions.Backward)
                    {
                        cf.force = Vector3.left * slidingSpeed;
                        //lockedDirection = FixedAxisDirections.XBackward;
                    }
                    else if (lastDirection == Directions.Right)
                    {
                        cf.force = Vector3.back * slidingSpeed;
                        //lockedDirection = FixedAxisDirections.ZBackward;
                    }
                    else if (lastDirection == Directions.Left)
                    {
                        cf.force = Vector3.forward * slidingSpeed;
                        //lockedDirection = FixedAxisDirections.ZForward;
                    }
                    break;
                case (4):
                    if (lastDirection == Directions.Forward)
                    {
                        cf.force = Vector3.back * slidingSpeed;
                        //lockedDirection = FixedAxisDirections.ZBackward;
                    }
                    else if (lastDirection == Directions.Backward)
                    {
                        cf.force = Vector3.forward * slidingSpeed;
                        //lockedDirection = FixedAxisDirections.ZForward;
                    }
                    else if (lastDirection == Directions.Right)
                    {
                        cf.force = Vector3.left * slidingSpeed;
                        //lockedDirection = FixedAxisDirections.XBackward;
                    }
                    else if (lastDirection == Directions.Left)
                    {
                        cf.force = Vector3.right * slidingSpeed;
                        //lockedDirection = FixedAxisDirections.XForward;
                    }
                    break;
            }
        }
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
            case (Items.Teleporter):
                if (!isOnGround || isRolling)
                {
                    isUsingItem = false;
                    yield break;
                }
                
                Debug.Log("Item Teleporter Will Be Used");
                if (teleporter1IsActive)
                {
                    TeleportCube(teleporter2.transform.position - new Vector3(0.5f,0,0.5f));
                }
                else if (teleporter2IsActive)
                {
                    TeleportCube(teleporter1.transform.position - new Vector3(0.5f,0,0.5f));
                }
                break;
        }
        
        while (timer < cooldownSeconds)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        isUsingItem = false;
    }
    
    // Function to rotate side camera
    private IEnumerator RotateCamera(CameraRotation side)
    {
        isChangingCamera = true;
        
        var timeToRotate = mainCam.m_DefaultBlend.BlendTime;
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

        if (isSliding)
        {
            camSwitchWhileSliding = true;
        }
        
        isChangingCamera = false;
    }
    
    // Convert the input into the right movement even after camera rotated
    private void GetMovementWithActiveCamera(Directions direction)
    {
        if (isRolling)
        {
            return;
        }
        
        lastDirection = direction;
        
        switch (activeCam)
        {
            case (1):
                if (direction == Directions.Forward)
                {
                    StartCoroutine(Roll(Vector3.left));
                }
                else if (direction == Directions.Backward)
                {
                    StartCoroutine(Roll(Vector3.right));
                }
                else if (direction == Directions.Left)
                {
                    StartCoroutine(Roll(Vector3.back));
                }
                else if (direction == Directions.Right)
                {
                    StartCoroutine(Roll(Vector3.forward));
                }
                break;
            case (2):
                if (direction == Directions.Forward)
                {
                    StartCoroutine(Roll(Vector3.forward));
                }
                else if (direction == Directions.Backward)
                {
                    StartCoroutine(Roll(Vector3.back));
                }
                else if (direction == Directions.Left)
                {
                    StartCoroutine(Roll(Vector3.left));
                }
                else if (direction == Directions.Right)
                {
                    StartCoroutine(Roll(Vector3.right));
                }
                break;
            case (3):
                if (direction == Directions.Forward)
                {
                    StartCoroutine(Roll(Vector3.right));
                }
                else if (direction == Directions.Backward)
                {
                    StartCoroutine(Roll(Vector3.left));
                }
                else if (direction == Directions.Left)
                {
                    StartCoroutine(Roll(Vector3.forward));
                }
                else if (direction == Directions.Right)
                {
                    StartCoroutine(Roll(Vector3.back));
                }
                break;
            case (4):
                if (direction == Directions.Forward)
                {
                    StartCoroutine(Roll(Vector3.back));
                }
                else if (direction == Directions.Backward)
                {
                    StartCoroutine(Roll(Vector3.forward));
                }
                else if (direction == Directions.Left)
                {
                    StartCoroutine(Roll(Vector3.right));
                }
                else if (direction == Directions.Right)
                {
                    StartCoroutine(Roll(Vector3.left));
                }
                break;
        }
    }

    private void UpdateRollCount(int rollCount)
    {
        rollCountText.text = $"Rolls: \n{rollCount}";
    }
}
