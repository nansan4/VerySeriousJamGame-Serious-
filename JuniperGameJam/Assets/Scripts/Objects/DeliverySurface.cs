using UnityEngine;
using System.Collections;
using UnityEngine.VFX;

/// <summary>
/// Players place DeliverableObjects onto this object type, it handles its own effects and sends a spawn event when a package is delivered
/// </summary>
public class DeliverySurface : MonoBehaviour
{
    [SerializeField] private float rejectionTime = 7f;
    [SerializeField] private Transform markerTransform;
    [SerializeField] private VisualEffect fireVfx;
    [SerializeField] private DeliveryBrazier brazier;
    private bool _alreadyRejected = false;

    public Transform DeliveryTransform { get { return transform; } }
    public Transform MarkerTransform { get { return markerTransform; } }

    public void DeliverySequence()
    {
        //play animations, fx, sounds, etc. here
        Debug.Log("delivery sequence");

        fireVfx.SendEvent("StartValid");
        brazier.FadeLightColor("StartValid");

        DeliveryManager.Instance.IncrementScore();
        DeliveryManager.Instance.SpawnDeliverables(false);

        StartCoroutine(CooldownRoutine());
    }

    public void RejectSequence()
    {
        //play reject sound, change mat, etc. here

        Debug.Log("rejection sequence");
        fireVfx.SendEvent("StartNotValid");

        brazier.FadeLightColor("StartNotValid");

        if (!_alreadyRejected) StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        _alreadyRejected = true;
        yield return new WaitForSeconds(rejectionTime);
        _alreadyRejected = false;
        //reset mat, vfx, etc. here
        fireVfx.SendEvent("StartRegular");
        brazier.FadeLightColor("StartRegular");
    }
}
