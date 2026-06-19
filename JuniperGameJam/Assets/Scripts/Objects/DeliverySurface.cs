using UnityEngine;
using System.Collections;

public class DeliverySurface : MonoBehaviour
{
    [SerializeField] private Transform deliveryMarkerTransform;
    [SerializeField] private float rejectionTime = 10f;

    public Transform DeliveryTransform { get { return deliveryMarkerTransform; } }

    public void DeliverySequence()
    {
        //play animations, fx, sounds, etc. here
        Debug.Log("Correct surface!");

        DeliveryManager.Instance.ChangeScore(false); //increase score
        DeliveryManager.Instance.SpawnDeliverables();
    }

    public void RejectSequence()
    {
        //play reject sound, change mat, etc. here
        Debug.Log("wrong surface!");

        StartCoroutine(RejectCooldown());
    }

    private IEnumerator RejectCooldown()
    {
        yield return new WaitForSeconds(rejectionTime);

        //reset mat, vfx, etc. here
    }
}
