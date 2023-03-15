using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorController : MonoBehaviour
{
    [SerializeField] private PlayerController pc;
    [SerializeField] private Transform elevatorPosition;
    private bool liftActivate;

    /*private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            Debug.Log("Player has entered ELEVATOR");
            liftActivate = true;
        }
    }*/

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        var pos = transform.position;
        
        Vector3 ray1 = transform.TransformDirection(Vector3.up) * 0.5f;
        Gizmos.DrawRay(pos, ray1);
    }*/

    private void FixedUpdate()
    {
        if (pc.isElevating && liftActivate)
        {
            StartCoroutine(Elevate());
        }
    }

    private IEnumerator Elevate()
    {
        liftActivate = true;
        
        var timer = 0f;
        var timeToElevate = 0.5f;
        var tempPosition = elevatorPosition.localPosition.y;
        
        while (timer < timeToElevate)
        {
            timer += Time.deltaTime;
            elevatorPosition.localPosition = new Vector3(elevatorPosition.localPosition.x, tempPosition + (timer / timeToElevate), elevatorPosition.localPosition.z);
            yield return null;
        }
        
        liftActivate = false;
    }
}
