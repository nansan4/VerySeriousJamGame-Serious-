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

    [Tooltip("Should the spawner wait for the next package to be delivered or just spawn anyways?")]
    [SerializeField] private bool spawnAfterCooldown = true;
    [SerializeField] private List<DeliverableObject> prefabDeliverables = new List<DeliverableObject>();

    private bool _hasObjectSpawned = false;
    private bool _isOnCooldown = false;
    public bool HasObjectSpawned { get { return _hasObjectSpawned; } }

    public void SpawnDeliverable()
    {
        if (_hasObjectSpawned || _isOnCooldown || DeliveryManager.Instance.GlobalBoxCount + 1 > DeliveryManager.Instance.GlobalBoxMaximum)
        {
            Debug.Log("Not going to spawn object: Spawner already has object on spawn pad, is cooling down, or would exceed max box count, which is currently " + DeliveryManager.Instance.GlobalBoxCount);
            return;
        }
        else
        {
            DeliverableObject obj = Instantiate(prefabDeliverables.GetRandomItem(), spawnTransform.position, Quaternion.identity);
            Transform dest = DeliveryManager.Instance.SetDeliveryDestination(obj);

            Debug.Log("spawning object: " + obj + ", at: " + spawnTransform.position + ", for destination: " + dest.position);

            _hasObjectSpawned = true;
            DeliveryManager.Instance.IncrementBoxCount();

            StartCoroutine(CooldownRoutine());
        }
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
        if(spawnAfterCooldown) SpawnDeliverable();
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
