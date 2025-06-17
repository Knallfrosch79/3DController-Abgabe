using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Movement_Input : MonoBehaviour
{
    private float movementSpeed = 1;
    private float jumpHeight = 1;

    private new Transform transform;
    private Rigidbody rigidbody;

    private void Awake()
    {
        transform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveInDirection(MoveDirection direction)
    {
        rigidbody.velocity = new Vector3(direction * movementSpeed, rigidbody.velocity.y);
}
