using UnityEngine;
using System.Collections;

public class DeliverableObject : MonoBehaviour
{
    [Header("Component Refs")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider col;
    [SerializeField] private Collider detectionTrigger;
    [SerializeField] private GameObject hookLoop;
    [SerializeField] private Material mat;

    [Header("Instance Vars")]
    [SerializeField] private bool isFragile = false;
    [SerializeField] private float breakForceThreshold = 1f;
    [SerializeField] private float malfunctionMultiplier = 1f;
    [SerializeField] private float despawnWaitTime = 15f;

    [Header("DEBUG")]
    [SerializeField] private bool enableDebug = false;

    private MeshRenderer _meshRenderer;
    private bool _isOnSurface = false;
    private bool _isOffHook = false;
    private bool _isDelivered = false;
    private float _currentMalfunctionChance;
    private GameObject _destination;
    private float _invalidityTime;

    private void Start()
    {
        _currentMalfunctionChance = GameState.Instance.BoxMalfunctionChance * malfunctionMultiplier;
        _invalidityTime = GameState.Instance.BoxInvalidityTime;

        _meshRenderer = gameObject.GetComponent<MeshRenderer>();

        _isDelivered = enableDebug ? true : false;
        if (_isDelivered) CheckDelivery();

        StartCoroutine(InvalidityRoutine());
    }

    //is delivered? : 
    //on coll enter with the deliver surface && trigger not overlap with object on hook layer

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.impulse.magnitude > breakForceThreshold && isFragile)
        {
            //Debug.Log("impulse: " + collision.impulse.magnitude);
            BreakDeliverable();
        }

        if(collision.collider.CompareTag("Delivery Surface") && collision.gameObject == _destination)
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
            _isOffHook = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hook"))
        {
            _isOffHook = false;
        }
    }

    private IEnumerator InvalidityRoutine()
    {
        yield return new WaitForSeconds(_invalidityTime);

        DeliveryManager.Instance.ChangeScore(true); //decrement score for missed box
        //pass reason to ui manager to display

        PrepareBoxDestroy();
    }
    private void BreakDeliverable()
    {
        Debug.Log("Box broke!");
        //pass reason to UI manager here also

        PrepareBoxDestroy();
    }

    private void CheckDelivery()
    {
        Debug.Log("checking delivery for object: " + gameObject + ": is on surface: " + _isOnSurface + " is off hook: " + _isOffHook + " is delivered: " + _isDelivered);
        if((_isOnSurface && _isOffHook) || _isDelivered)
        {
            _isDelivered = true;

            float mal = Random.Range(0.0f, 1.0f);

            if(mal <= _currentMalfunctionChance)
            {
                Debug.Log("Box loop malfunctioned!");
                //play malfunction vfx here and skip over animation loop
            }
            else
            {
                hookLoop.SetActive(false);
                //or play anim here
            }

            // minimal components + not happening often = prolly not a big issue
            DeliverySurface surface = _destination.GetComponent<DeliverySurface>();
            surface.DeliverySequence();

            StartCoroutine(DespawnRoutine());
        }
    }

    private IEnumerator DespawnRoutine()
    {
        yield return new WaitForSeconds(despawnWaitTime);

        _meshRenderer.enabled = false;

        PrepareBoxDestroy();
    }

    private void PrepareBoxDestroy()
    {
        _meshRenderer.enabled = false;
        col.enabled = false;

        BoxCleanupManager.Instance.AddObjectToDestroy(gameObject);

    }

    public void SetDestination(GameObject destination)
    {
        //set game object so i can compare object with collision detection
        _destination = destination;
    }
}
