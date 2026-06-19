using UnityEngine;
using System.Collections.Generic;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private List<DeliverableObject> prefabDeliverables = new List<DeliverableObject>();

    private bool _hasObjectSpawned = false;
    public bool HasObjectSpawned { get { return _hasObjectSpawned; } }

    public void SpawnDeliverable()
    {
        if (_hasObjectSpawned)
        {
            Debug.Log("Spawner already has object on spawn pad");
            return;
        }
        else
        {
            DeliverableObject obj = Instantiate(prefabDeliverables.GetRandomItem(), spawnTransform.position, Quaternion.identity);
            Transform dest = DeliveryManager.Instance.SetDeliveryDestination(obj);
            Debug.Log("spawning object: " + obj + ", at: " + spawnTransform.position + ", for destination: " + dest);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Deliverable Object"))
        {
            _hasObjectSpawned = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Deliverable Object"))
        {
            _hasObjectSpawned = false;
        }
    }
}
