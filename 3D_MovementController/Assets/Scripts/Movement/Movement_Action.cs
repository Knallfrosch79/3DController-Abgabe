using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Movement_Action : MonoBehaviour
{
    [SerializeField]private float movementSpeed = 1;
    [SerializeField]private float jumpHeight = 1;
    public InputAction moveAction;

    private new Transform transform;    

    private void Awake()
    {
        transform = GetComponent<Transform>();
    }

    public void MoveHorizontally(Vector2 direction)
    {
        transform.position += new Vector3(direction.x * movementSpeed, 0, direction.y * movementSpeed) * Time.deltaTime;
    }

    public void Jump()
    {
        transform.position += new Vector3(0, jumpHeight, 0);
    }


}
