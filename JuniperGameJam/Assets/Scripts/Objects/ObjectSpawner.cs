using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// this is what spawns the deliverables
/// </summary>
public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawnTransform;
    [SerializeField] private Collider spawnTrigger;
    [SerializeField] private float cooldownTime = 10f;
    [SerializeField] private float spawnDelayAfterCooldown = 10f;

    [Tooltip("Should the spawner wait for the next package to be delivered or just spawn anyways?")]
    [SerializeField] private bool spawnAfterCooldown = true;
    [SerializeField] private List<DeliverableObject> prefabDeliverables = new List<DeliverableObject>();

    private bool _hasObjectSpawned = false;
    private bool _isOnCooldown = false;
    public bool HasObjectSpawned { get { return _hasObjectSpawned; } }

    public void TrySpawnDeliverable()
    {
        if (_hasObjectSpawned || _isOnCooldown || DeliveryManager.Instance.GlobalBoxCount + 1 > DeliveryManager.Instance.GlobalBoxMaximum)
        {
            Debug.Log("Not going to spawn object: Spawner " + gameObject + " already has object on spawn pad: " + _hasObjectSpawned + ", is cooling down: " + _isOnCooldown + ", or would exceed max box count, currently: " + DeliveryManager.Instance.GlobalBoxMaximum);
            return;
        }

        DeliverableObject obj = Instantiate(prefabDeliverables.GetRandomItem(), spawnTransform.position, Quaternion.identity);
        Transform dest = DeliveryManager.Instance.SetDeliveryDestination(obj);

        Debug.Log("spawning object: " + obj + ", at: " + spawnTransform.position + ", for destination: " + dest.position);

        _hasObjectSpawned = true;
        DeliveryManager.Instance.IncrementBoxCount();

        StartCoroutine(CooldownRoutine());
    }

    public void ForceSpawnDeliverable()
    {
        if (_hasObjectSpawned) return; //don't want to force two boxes into each other

        Debug.LogWarning("forcing deliverable spawns for spawner " + gameObject);

        DeliverableObject obj = Instantiate(prefabDeliverables.GetRandomItem(), spawnTransform.position, Quaternion.identity);
        Transform dest = DeliveryManager.Instance.SetDeliveryDestination(obj);

        //Debug.Log("force spawning object: " + obj + ", at: " + spawnTransform.position + ", for destination: " + dest.position);

        _hasObjectSpawned = true;
        DeliveryManager.Instance.IncrementBoxCount();

        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        _isOnCooldown = true;
        yield return new WaitForSeconds(cooldownTime);
        _isOnCooldown = false;

        if (spawnAfterCooldown) 
        { 
            Debug.Log("spawner " + gameObject + " is now ready to spawn again, spawning a new box");
            yield return new WaitUntil(() => _hasObjectSpawned == false);

            yield return new WaitForSeconds(spawnDelayAfterCooldown); //wait a few seconds to make sure the previous box is gone before spawning a new one

            TrySpawnDeliverable();
        }

    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.gameObject.CompareTag("Deliverable Object"))
    //    {
    //        _hasObjectSpawned = true;
    //    }
    //}

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Deliverable Object"))
        {
            _hasObjectSpawned = false;
            //Debug.Log("deliverable exited spawn trigger, spawner " + gameObject + " is now ready to spawn again");
            
        }
    }
}
