using UnityEngine;
using System.Collections;

public class DeliverySurface : MonoBehaviour
{
    [SerializeField] private float rejectionTime = 10f;
    private bool _alreadyRejected = false;

    public Transform DeliveryTransform { get { return transform; } }

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
