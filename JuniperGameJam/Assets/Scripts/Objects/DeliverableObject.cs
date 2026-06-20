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
    private float _currentMalfunctionChance;
    private GameObject _destination;
    private float _invalidityTime;

    private void Start()
    {
        _currentMalfunctionChance = GameState.Instance.BoxMalfunctionChance * malfunctionMultiplier;
        //_invalidityTime = GameState.Instance.BoxInvalidityTime;
        _invalidityTime = 45f;

        _meshRenderer = gameObject.GetComponent<MeshRenderer>();

        //if (enableDebug) CheckDelivery();

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
            return;
        }

        if(collision.collider.CompareTag("Delivery Surface") && collision.gameObject == _destination)
        {
            _isOnSurface = true;

            CheckDelivery();
        }else if(collision.collider.CompareTag("Delivery Surface") && collision.gameObject != _destination)
        {
            collision.collider.gameObject.TryGetComponent<DeliverySurface>(out DeliverySurface surface);
            if (surface) surface.RejectSequence();
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

            CheckDelivery();
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
        Debug.Log("starting invalidity routine");
        yield return new WaitForSeconds(_invalidityTime);
        Debug.Log("box is invalid now");
        DeliveryManager.Instance.DecrementScore();
        //pass reason to ui manager to display

        PrepareBoxDestroy();
    }
    private void BreakDeliverable()
    {
        Debug.Log("Box broke!");
        //pass reason to UI manager here also
        DeliveryManager.Instance.DecrementScore();

        PrepareBoxDestroy();
    }

    private void CheckDelivery()
    {
        //Debug.Log("checking delivery for object: " + gameObject + ": is on surface: " + _isOnSurface + " is off hook: " + _isOffHook);
        if((_isOnSurface && _isOffHook) || enableDebug)
        {
            Debug.Log("box is delivered!");

            float mal = Random.Range(0.0f, 1.0f);

            if(mal <= _currentMalfunctionChance)
            {
                Debug.Log("Box loop malfunctioned!");
                //play malfunction vfx here and skip over animation loop
            }
            else
            {
                Debug.Log("setting hook loop inactive");
                hookLoop.SetActive(false);
                //or play anim here
            }

            // minimal components + not happening often = prolly not a big issue
            if(_destination != null)
            {
                _destination.TryGetComponent<DeliverySurface>(out DeliverySurface surface);
                if (surface) surface.DeliverySequence();
                StartCoroutine(DespawnRoutine());
            }
            else
            {
                Debug.Log("_destination is null");
            }

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
        Debug.Log("preparing box for destruction");
        _meshRenderer.enabled = false;
        col.enabled = false;
        StopCoroutine(InvalidityRoutine());
        DeliveryManager.Instance.DecrementBoxCount();

        BoxCleanupManager.Instance.AddObjectToDestroy(gameObject);

    }

    public void SetDestination(GameObject destination)
    {
        //set game object in order to compare object using collision detection
        _destination = destination;
    }
}
