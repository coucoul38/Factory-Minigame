using UnityEngine;
using UnityEngine.Serialization;
using Slider = UnityEngine.UI.Slider;

public class ObjectDispenser : MonoBehaviour
{
    [SerializeField] private PickableObject pickableObject;
    [FormerlySerializedAs("objectResuplyDelay")] [SerializeField] private float resuplyDelay = 5;
    [SerializeField] private bool autoResuply;
    [SerializeField] private bool startSupplied = false;
    [SerializeField] private Slider slider;
    private float _resuplyTime;
    private bool _resuplying = false;
    private bool _supplied;
    private PickableObject _objInstance;
    private SpriteRenderer _sprite;
    
    // Start is called before the first frame update
    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        if (startSupplied)
        {
            EndResupply();
        }
        else
        {
            _sprite.color = Color.red;
        }
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
        Debug.Log("Start Resuply");
        _resuplyTime = resuplyDelay;
        _resuplying = true;
        _sprite.color = Color.yellow;
    }
    
    public void EndResupply()
    {
        _supplied = true;
        _sprite.color = Color.green;
        // show object
        _objInstance = Instantiate(pickableObject);
        _resuplying = false;
    }

    public void OnInteract(PlayerScript player)
    {
        Debug.Log("Interact with dispenser. resuplyDelay: " + _resuplyTime + " resuplying: " + _resuplying);
        if (_supplied)
        {
            // Player picks up the object
            player.PickupObject(_objInstance);
            _supplied = false;
            _sprite.color = Color.red;
        } else if (_resuplyTime == 0 && _resuplying == false)
        {
            StartResupply();
        }
    }
}