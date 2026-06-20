using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    private static DeliveryManager _instance = null; //static/global reference to the single instance of the object because singleton pattern
    public static DeliveryManager Instance { get { return _instance; } } //safe way for other objects to reference GameManager without changing the ref

    public int GlobalBoxCount { get {  return _boxCount; } }
    public int GlobalBoxMaximum { get {  return maxBoxesAllowed; } }

    [SerializeField] private List<ObjectSpawner> objectSpawners = new List<ObjectSpawner>();
    [SerializeField] private List<DeliverySurface> deliverySurfaces = new List<DeliverySurface>();
    [SerializeField] private int maxBoxesToSpawn = 3;
    [SerializeField] private int maxBoxesAllowed = 1;

    private int _boxCount = 0;
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
    /// call this in the object spawner, obj is the object that is being spawned
    /// </summary>
    public Transform SetDeliveryDestination(DeliverableObject obj, int surface = -1)
    {
        if (surface < 0)
        {
            int rand = Random.Range(0, deliverySurfaces.Count);
            _marker = deliverySurfaces[rand].DeliveryTransform;
        }
        else
        {
            _marker = deliverySurfaces[surface].DeliveryTransform;
        }

        //set marker here, likely in a UI manager

        obj.SetDestination(_marker.gameObject);
        return _marker;
    }

    public void IncrementScore()
    {
        GameManager.Instance.IncrementScore();
    }

    public void DecrementScore()
    {
        GameManager.Instance.DecrementScore();
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
            Debug.Log("box count is " + _boxCount + ", spawning more boxes");
            SpawnDeliverables(true);
        }
    }

    public void SpawnDeliverables(bool force)
    {
        StartCoroutine(SpawnRoutine(force));
    }

    private IEnumerator SpawnRoutine(bool force)
    {
        int rand = Random.Range(1, maxBoxesToSpawn + 1);
        //rand = Mathf.Clamp(rand, 0, objectSpawners.Count);
        int count = 0;
        int idx = 0;
        objectSpawners.Shuffle();

        yield return new WaitForNextFrameUnit();

        Debug.Log("about to spawn " + rand + " boxes");
        while (count < rand)
        {
            if(force && count == 0){ objectSpawners[idx].ForceSpawnDeliverable(); }
            else { objectSpawners[idx].SpawnDeliverable(); }
                idx = (idx + 1) % objectSpawners.Count;
            count++;
            yield return null;
        }

        //Debug.Log("boxes spawned!");

        yield return null;
    }
    
}
