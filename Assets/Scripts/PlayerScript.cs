using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5;
    private Rigidbody2D _rb;
    private Vector2 _moveInput;

    [SerializeField] private int inventorySize;
    private List<PickableObject> _inventory;

    [SerializeField] private float interactionRange = 5;
    
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

    private void Interact(InputAction.CallbackContext context)
    {
        float closestDistance = interactionRange + 100;
        Interactable closestInteractable;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRange);
        foreach (Collider2D col in colliders)
        {
            // Check if the collider has an Interactable component
            Interactable interactable = col.GetComponent<Interactable>();
            if (interactable != null && (transform.position - interactable.gameObject.transform.position).magnitude < closestDistance)
            {
                closestInteractable = interactable;
                closestDistance = (transform.position - interactable.gameObject.transform.position).magnitude;
            }
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>().normalized;
    }

    // add object to inventory
    public void PickupObject(PickableObject obj)
    {
        // If inventory full, override first object
        if (_inventory.Count > inventorySize)
        {
            _inventory.RemoveAt(0);
        }
        _inventory.Add(obj);
    }
}
