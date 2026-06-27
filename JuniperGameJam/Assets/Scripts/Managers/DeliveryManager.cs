using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

/// <summary>
/// the delivery manager manages sending spawn commands to the object spawners, setting delivery destinations, handling box count, and passing score changes
/// </summary>
public class DeliveryManager : MonoBehaviour
{
    private static DeliveryManager _instance = null; //static/global reference to the single instance of the object because singleton pattern
    public static DeliveryManager Instance { get { return _instance; } } //safe way for other objects to reference GameManager without changing the ref

    public int GlobalBoxCount { get {  return _boxCount; } }
    public int GlobalBoxMaximum { get {  return maxBoxesAllowed; } }

    [SerializeField] private List<ObjectSpawner> objectSpawners = new List<ObjectSpawner>();
    [SerializeField] private List<DeliverySurface> deliverySurfaces = new List<DeliverySurface>();

    //should probably fold these into the GameState difficulty system
    [SerializeField] private int maxBoxesToSpawn = 3;
    [SerializeField] private int maxBoxesAllowed = 4;

    private int _boxCount = 0; 
    private Transform _destination;
    private Transform _marker;

    private void Awake()
    {
        #region Singleton

        if (_instance == null) //if an instance of the object doesnt already exist
        {
            _instance = this;
        }
        else //if an instance already exists and its not this one
        {
            Destroy(gameObject); //gameObject points to the object the script is on
        }

        #endregion
    }
    
    /// <summary>
    /// call this in the object spawner, obj is the object that is being spawned, surface is the index in the DeliveryManager's deliverySurfaces array, default is -1 (random)
    /// </summary>
    public Transform SetDeliveryDestination(DeliverableObject obj, int surface = -1)
    {
        if (surface < 0)
        {
            int rand = Random.Range(0, deliverySurfaces.Count);
            _destination = deliverySurfaces[rand].DeliveryTransform;
            _marker = deliverySurfaces[rand].MarkerTransform;
        }
        else
        {
            _destination = deliverySurfaces[surface].DeliveryTransform;
            _marker = deliverySurfaces[surface].MarkerTransform;
        }

        //pass _marker ref here, likely to a UI manager
        // smth like UIManager.Instance.AddDestinationMarker(_marker);

        obj.SetDestination(_destination.gameObject);
        return _destination;
    }

    public void IncrementScore()
    {
        GameManager.Instance.IncrementScore();
        UI_Manager.instance.IncrementDelivered(1);
    }

    public void DecrementScore()
    {
        GameManager.Instance.DecrementScore();
        UI_Manager.instance.IncrementMissed(1);
    }

    public void IncrementBoxCount()
    {
        _boxCount++;
    }

    public void DecrementBoxCount()
    {
        _boxCount--;

        if( _boxCount <= 0)
        {
            _boxCount = 0;
            //Debug.Log("box count is " + _boxCount + ", force spawning more boxes");
            SpawnDeliverables(true);
        }
    }

    public void SpawnDeliverables(bool force)
    {
        if(force == true){ StartCoroutine(ForceSpawnRoutine()); }
        else { StartCoroutine(SpawnRoutine()); }
    }

    private IEnumerator SpawnRoutine()
    {
        int rand = Random.Range(1, maxBoxesToSpawn + 1);
        int idx = 0;
        int count = 0;
        objectSpawners.Shuffle();

        yield return new WaitForSeconds(2f);

        //Debug.Log("about to spawn " + rand + " boxes");
        while (count < rand)
        {
            objectSpawners[idx].TrySpawnDeliverable();
            idx = (idx + 1) % objectSpawners.Count;
            count++;
            yield return null;
        }

        //Debug.Log("boxes spawned!");

        yield return null;
    }

    private IEnumerator ForceSpawnRoutine()
    {
        int rand = Random.Range(1, maxBoxesToSpawn + 1); //want to include the input max boxes to spawn number
        int idx = Random.Range(0, objectSpawners.Count); //since max of random.range is exclusive
        int count = 0;

        yield return new WaitForSeconds(2f);

        //Debug.Log("about to spawn " + rand + " boxes");

        while(count < rand)
        {
            objectSpawners[idx].ForceSpawnDeliverable();
            idx = (idx + 1) % objectSpawners.Count;
            count++;
            yield return null;
        }

        yield return null;
    }
    
}
