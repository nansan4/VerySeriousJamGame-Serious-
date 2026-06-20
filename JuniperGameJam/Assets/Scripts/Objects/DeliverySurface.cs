using UnityEngine;
using System.Collections;

/// <summary>
/// Players place DeliverableObjects onto this object type, it handles its own effects and sends a spawn event when a package is delivered
/// </summary>
public class DeliverySurface : MonoBehaviour
{
    [SerializeField] private float rejectionTime = 10f;
    [SerializeField] private Transform markerTransform;
    private bool _alreadyRejected = false;

    public Transform DeliveryTransform { get { return transform; } }
    public Transform MarkerTransform { get { return markerTransform; } }

    public void DeliverySequence()
    {
        //play animations, fx, sounds, etc. here
        Debug.Log("delivery sequence");

        DeliveryManager.Instance.IncrementScore();
        DeliveryManager.Instance.SpawnDeliverables(false);
    }

    public void RejectSequence()
    {
        //play reject sound, change mat, etc. here
        Debug.Log("rejection sequence");

        if(!_alreadyRejected) StartCoroutine(RejectCooldown());
    }

    private IEnumerator RejectCooldown()
    {
        yield return new WaitForSeconds(rejectionTime);

        //reset mat, vfx, etc. here
    }
}
