using UnityEngine;
using System.Collections.Generic;

public class DeliveryManager : MonoBehaviour
{
    private static DeliveryManager _instance = null; //static/global reference to the single instance of the object because singleton pattern
    public static DeliveryManager Instance { get { return _instance; } } //safe way for other objects to reference GameManager without changing the ref

    [SerializeField] private List<DeliverySurface> deliverySurfaces = new List<DeliverySurface>();

    private void Awake()
    {
        #region Singleton

        if (_instance == null) //if an instance of the object doesnt already exist
        {
            _instance = this;
        }
        else //if an instance already exists and its not this one
        {
            Destroy(gameObject); //gameObject points to the object the script is on
        }

        #endregion
    }
    

    
}
