using UnityEngine;
using System.Collections.Generic;

public class DeliveryManager : MonoBehaviour
{
    private static DeliveryManager _instance = null; //static/global reference to the single instance of the object because singleton pattern
    public static DeliveryManager Instance { get { return _instance; } } //safe way for other objects to reference GameManager without changing the ref

    [SerializeField] private List<ObjectSpawner> objectSpawners = new List<ObjectSpawner>();
    [SerializeField] private List<DeliverySurface> deliverySurfaces = new List<DeliverySurface>();
    [SerializeField] private int maxBoxesToSpawn = 3;

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

    public void ChangeScore(bool decrement)
    {
        if (decrement)
        {
            //decrement func in GM
        }
        else
        {
            //increment func in GM
        }
    }

    public void SpawnDeliverables()
    {
        int rand = Random.Range(1, maxBoxesToSpawn + 1);
        int count = 0;
        int idx = 0;

        while (count <= rand)
        {
            objectSpawners[idx].SpawnDeliverable();
            idx = (idx + 1) % objectSpawners.Count;
            count++;
        }
    }
    
}
