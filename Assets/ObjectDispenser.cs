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

    
    // Start is called before the first frame update
    void Start()
    {
        if (startSupplied)
        {
            EndResupply();
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
    }
    
    public void EndResupply()
    {
        _supplied = true;
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
        } else if (_resuplyTime == 0 && _resuplying == false)
        {
            StartResupply();
        }
    }
}