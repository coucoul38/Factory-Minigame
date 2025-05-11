using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Slider = UnityEngine.UI.Slider;

public class ObjectDispenser : MonoBehaviour
{
    [SerializeField] private PickableObject pickableObject;
    [SerializeField] private float resuplyDelay = 5;
    [SerializeField] private bool autoResuply;
    [SerializeField] private bool startSupplied = false;
    
    [SerializeField] private List<string> ingredientsRequired;
    private List<string> _inventory;
    
    [SerializeField] private Slider slider;
    private float _resuplyTime;
    private bool _resuplying = false;
    private bool _supplied;
    [SerializeField] private bool changeColor = true;
    private SpriteRenderer _sprite;
    
    // Start is called before the first frame update
    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        if (startSupplied)
        {
            EndResupply();
        }

        _inventory = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_resuplyTime == 0 && _resuplying)
        {
            EndResupply();
        } else if (autoResuply && _supplied == false && _resuplying == false && _resuplyTime == 0)
        {
            StartResupply();
        }
        
        _resuplyTime -= Time.deltaTime;
        if (_resuplyTime < 0)
            _resuplyTime = 0;
        UpdateSlider();
    }

    void UpdateSlider()
    {
        if (_resuplying)
        {
            slider.gameObject.SetActive(true);
        }
        else
        {
            slider.gameObject.SetActive(false);
        }
        slider.value = _resuplyTime / resuplyDelay;
    }
    
    public void StartResupply()
    {
        _resuplyTime = resuplyDelay;
        _resuplying = true;
        if(changeColor)
            _sprite.color = Color.yellow;
        
        // Use ingredients
        if (ingredientsRequired != null && ingredientsRequired.Count != 0)
        {
            foreach (string ingredient in ingredientsRequired)
            {
                _inventory.Remove(ingredient);
            }
        }
    }
    
    public void EndResupply()
    {
        _supplied = true;
        if(changeColor)
            _sprite.color = Color.green;
        _resuplying = false;
    }

    public void OnInteract()
    {
        PlayerScript player = GameManager.Instance.GetPlayer();
        
        if (_supplied)
        {
            // Player picks up the object
            player.PickupObject(pickableObject);
            _supplied = false;
            if(changeColor)
                _sprite.color = Color.red;
        } 
        else if (_resuplyTime == 0 && _resuplying == false)
        {
            if (ingredientsRequired != null && ingredientsRequired.Count != 0)
            {
                // Check for ingredients
                int count = 0;
                foreach (string ingredient in ingredientsRequired)
                {
                    if (_inventory.Contains(ingredient) == false)
                    {
                        // try to take ingredients from the player
                        if (player.TakeIngredient(ingredient))
                        {
                            _inventory.Add(ingredient);
                            count++;
                        }
                    }
                    else
                    {
                        count++;
                    }
                }

                if (count != ingredientsRequired.Count)
                {
                    Debug.Log(ingredientsRequired.Count + " ingredients required, only " + count + " in");
                    return;
                }
            }
            
            StartResupply();
        }
    }
}