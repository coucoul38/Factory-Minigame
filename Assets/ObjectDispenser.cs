using UnityEngine;
using UnityEngine.Serialization;

public class ObjectDispenser : MonoBehaviour
{
    [SerializeField] private PickableObject pickableObject;
    [FormerlySerializedAs("objectResuplyDelay")] [SerializeField] private float resuplyDelay = 5;
    [SerializeField] private bool autoResuply;
    [SerializeField] private bool startSupplied = false;
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
        } else if (autoResuply && _resuplying == false && _resuplyTime == 0)
        {
            StartResupply();
        }
    }

    public void StartResupply()
    {
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

    void OnInterract(PlayerScript player)
    {
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