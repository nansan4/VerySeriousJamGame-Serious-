using UnityEngine;

public class DeliverableObject : MonoBehaviour
{
    [Header("Component Refs")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider col;
    [SerializeField] private Collider detectionTrigger;
    [SerializeField] private GameObject hookLoop;

    [Header("Instance Vars")]
    [SerializeField] private bool isFragile = false;
    [SerializeField] private float breakForceThreshold = 1f;
    [SerializeField] private float malfunctionMultiplier = 1f;

    private float _currentMalfunctionChance;
    private bool _isOnSurface = false;
    private bool _isOffHook = false;
    private bool _isDelivered = false;

    private void Start()
    {
        _currentMalfunctionChance = GameState.Instance.BoxMalfunctionChance * malfunctionMultiplier;
    }


    //is delivered? : 
    //on coll enter with the deliver surface && trigger not overlap with object on hook layer

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("Delivery Surface"))
        {
            _isOnSurface = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.collider.CompareTag("Delivery Surface"))
        {
            _isOnSurface = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Hook"))
        {

        }
    }
}
