using System.Collections;
using UnityEngine;
/// <summary>
/// A pointer that appears near the DeliverableObject to show the player where to deliver it
/// </summary>
public class DeliverablePointer : MonoBehaviour
{
    
    [SerializeField] private DeliverableObject DeliverableObject;
    [SerializeField] private GameObject BoxDetector;

    private Transform DeliveryPointTransform;

    public Transform LookAtTarget { get {return DeliveryPointTransform; } }

    private void SetTarget(Transform target = null)
    {
        DeliveryPointTransform = target;
    }

    private void Start()
    {
        StartCoroutine(DelaySetTransform());
        //gameObject.SetActive(false);
        
    }


    private IEnumerator DelaySetTransform()
    {
        yield return new WaitForSeconds(2f);

        OnGetLocation(DeliverableObject.BoxDeliveryDestination);
    }
    private void Update()
    {

        if (LookAtTarget)
        {
            transform.LookAt(LookAtTarget);
        }

        if (!BoxDetector.activeSelf)
        {
            Destroy(this);
        }
        
    }

    public void OnGetLocation(Transform deliverylocation)
    {
        gameObject.SetActive(true);
        SetTarget(deliverylocation);
    }

    public void OnDeliver()
    {
        SetTarget(null);
        gameObject.SetActive(false);
    }


}
