using UnityEngine;
/// <summary>
/// A pointer that appears near the DeliverableObject to show the player where to deliver it
/// </summary>
public class DeliverablePointer : MonoBehaviour
{
    
    [SerializeField] private DeliverableObject DeliverableObject;

    private Transform DeliveryPointTransform;

    public Transform LookAtTarget { get {return DeliveryPointTransform; } }

    private void SetTarget(Transform target = null)
    {
        DeliveryPointTransform = target;
    }

    private void Start()
    {
        
        //DeliverableObject.
        gameObject.SetActive(false);
    }

    private void Update()
    {

        if (LookAtTarget)
        {
            transform.LookAt(LookAtTarget);
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
