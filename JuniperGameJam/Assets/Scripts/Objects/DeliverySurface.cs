using UnityEngine;

public class DeliverySurface : MonoBehaviour
{
    [SerializeField] private Transform deliveryMarkerTransform;

    public Transform DeliveryTransform { get { return deliveryMarkerTransform; } }
}
