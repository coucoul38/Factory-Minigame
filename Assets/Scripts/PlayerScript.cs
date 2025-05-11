using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5;
    private Rigidbody2D _rb;
    private PlayerInputActions _inputActions;
    private Vector2 _moveInput;

    /////** INVENTORY **/////
    private List<PickableObject> _inventory;
    [SerializeField] private Transform hand1Transform; 
    [SerializeField] private Transform hand2Transform;

    
    [SerializeField] private float interactionRange = 5;
    private Interactable _closestInteractable = null;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _inventory = new List<PickableObject>();
        _rb = GetComponent<Rigidbody2D>();
        if(_rb == null)
            Debug.LogError("Player is missing rigidbody");
    }

    private void OnEnable()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Gameplay.Enable();
        _inputActions.Gameplay.Interaction.started += Interact;
        _inputActions.Gameplay.Movement.performed += Move;
        _inputActions.Gameplay.Movement.canceled += StopMovement;
    }

    private void OnDisable()
    {
        _inputActions.Gameplay.Disable();
        _inputActions.Gameplay.Interaction.started -= Interact;
        _inputActions.Gameplay.Movement.performed -= Move;
        _inputActions.Gameplay.Movement.canceled -= StopMovement;
        _inputActions = null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _rb.velocity = _moveInput * movementSpeed;
        FindInteractables();
    }

    private void FindInteractables()
    {
        float closestDistance = interactionRange + 100;
        _closestInteractable = null;
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRange);
        foreach (Collider2D col in colliders)
        {
            // Check if the collider has an Interactable component
            Interactable interactable = col.GetComponent<Interactable>();
            if (interactable != null && (transform.position - interactable.gameObject.transform.position).magnitude < closestDistance)
            {
                _closestInteractable = interactable;
                closestDistance = (transform.position - interactable.gameObject.transform.position).magnitude;
            }
        }

        if (closestDistance <= interactionRange && _closestInteractable != null)
        {
            // Highlight the interactable
            //_closestInteractable.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
    
    public void Interact(InputAction.CallbackContext context)
    {
        //Debug.Log("Interact");
        if(_closestInteractable != null)
            _closestInteractable.Interact();
    }

    public void Move(InputAction.CallbackContext context)
    {
        _moveInput = context.ReadValue<Vector2>().normalized;
    }

    private void StopMovement(InputAction.CallbackContext context)
    {
        _moveInput = Vector2.zero;
    }

    // add object to inventory
    public void PickupObject(PickableObject obj)
    {
        PickableObject objInstance;
        
        // If inventory full, override first object
        if (_inventory.Count == 2)
        {
            // Unparent object so childcount updates
            _inventory[0].gameObject.transform.SetParent(null);
            
            Destroy(_inventory[0].gameObject);
            _inventory.RemoveAt(0);
        }
        
        if (hand1Transform.childCount == 0)
        {
            objInstance = Instantiate(obj, hand1Transform);
        } else
        {
            objInstance = Instantiate(obj, hand2Transform);
        }
        
        objInstance.transform.localPosition = Vector2.zero;
        _inventory.Add(objInstance);
    }

    public bool TakeIngredient(string ingredientName)
    {
        foreach (PickableObject ingredient in _inventory)
        {
            if (ingredient.objName == ingredientName)
            {
                _inventory[_inventory.IndexOf(ingredient)].gameObject.transform.SetParent(null);
                Destroy(_inventory[_inventory.IndexOf(ingredient)].gameObject);
                _inventory.Remove(ingredient);
                return true;
            }
        }
        return false;
    }
}
