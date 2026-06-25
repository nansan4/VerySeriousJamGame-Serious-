using UnityEngine;
using System.Collections;
using UnityEngine.VFX;

/// <summary>
/// The objects the player is delivering, it handles it's own delivery, disabling, and adding to object pool 
/// </summary>
public class DeliverableObject : MonoBehaviour
{
    #region Serialized Fields
    [Header("Component Refs")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Collider col;
    [SerializeField] private Collider hookDetectionTrigger;
    [SerializeField] private GameObject hookLoop;
    [Tooltip("the parent object to which all meshes are parented, except VFX")]
    [SerializeField] private GameObject boxObj;
    [SerializeField] private VisualEffect vfx;

    //[SerializeField] private Material mat;

    [Header("Instance Vars")]
    //[SerializeField] private bool isFragile = false;
    [SerializeField] private DeliverableObjectType objectType;
    [SerializeField] private bool deliverAnywhere = false;
    [SerializeField] private float breakForceThreshold = 1f;
    [Tooltip("percent change to add to the malfunction chance, 1 is effectivley guaranteed (0 base + 1 = 1) and -1 means it will never happen (1 base - 1 = 0)")]
    [SerializeField][Range(-1f, 1f)] private float malfunctionAdditive = 0f;
    [SerializeField] private float despawnWaitTime = 15f;

    [Header("DEBUG")]
    [SerializeField] private bool enableDebug = false;
    #endregion

    #region Private Fields
    private bool _isOnSurface = false;
    private bool _isOffHook = false;
    private float _currentMalfunctionChance;
    private GameObject _destination;
    private float _invalidityTime;
    private GameObject _surface;
    #endregion

    #region Public Getters

    public Transform BoxDeliveryDestination { get { return _destination.transform; } }

    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        _currentMalfunctionChance = GameState.Instance.BoxMalfunctionChance + malfunctionAdditive;
        //_invalidityTime = GameState.Instance.BoxInvalidityTime;
        _invalidityTime = 45f;

        //_meshRenderer = gameObject.GetComponent<MeshRenderer>();

        //if (enableDebug) CheckDelivery();

        StartCoroutine(InvalidityRoutine());
    }
    #endregion

    #region Collision / Trigger Handlers
    //is delivered? : 
    //on coll enter with the deliver surface && trigger not overlap with object on hook layer

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.impulse.magnitude > breakForceThreshold && objectType == DeliverableObjectType.Fragile)
        {
            Debug.Log(gameObject + " impulse: " + collision.impulse.magnitude);
            BreakDeliverable();
            return;
        }

        if(collision.collider.CompareTag("Delivery Surface") && (collision.gameObject == _destination || deliverAnywhere))
        {
            _isOnSurface = true;
            _surface = deliverAnywhere ? collision.gameObject : _destination;

            CheckDelivery();
        }
        else if(collision.collider.CompareTag("Delivery Surface") && collision.gameObject != _destination)
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
    #endregion

    #region Delivery / State Logic
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
        Debug.Log(gameObject + "Box broke!");
        //pass reason to UI manager here also
        DeliveryManager.Instance.DecrementScore();

        vfx.SendEvent("OnBoxDestroy");

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
                //play malfunction vfx here
            }
            else
            {
                                //or play anim here
                Debug.Log("setting hook loop inactive");
                hookLoop.SetActive(false);
            }

            // minimal components + not happening often = prolly not a big issue
            if(_destination != null)
            {
                //_destination.TryGetComponent<DeliverySurface>(out DeliverySurface surface);
                //if (surface) surface.DeliverySequence();
                _surface.GetComponent<DeliverySurface>()?.DeliverySequence();
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

        //_meshRenderer.enabled = false;

        PrepareBoxDestroy();
    }

    private void PrepareBoxDestroy()
    {
        Debug.Log("preparing box for destruction");
        //disable object components
        hookLoop.SetActive(false);
        boxObj.SetActive(false);
        col.enabled = false;
        //rb.useGravity = false;
        hookDetectionTrigger.enabled = false;

        StopCoroutine(InvalidityRoutine());
        DeliveryManager.Instance.DecrementBoxCount();

        BoxCleanupManager.Instance.AddObjectToDestroy(gameObject);
        //gameObject.SetActive(false);

    }
    #endregion

    #region Public API
    public void SetDestination(GameObject destination)
    {
        //set game object in order to compare object using collision detection
        _destination = destination;
    }
    #endregion
}

#region Enums

public enum DeliverableObjectType
{
    Fragile,
    NonFragile
}

#endregion
