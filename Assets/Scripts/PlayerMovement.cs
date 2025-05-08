using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5;
    private Rigidbody2D _rb;
    private Vector2 _moveInput;
    
    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        if(_rb == null)
            Debug.LogError("Player is missing rigidbody");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _rb.velocity = _moveInput * movementSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>().normalized;
    }
}
