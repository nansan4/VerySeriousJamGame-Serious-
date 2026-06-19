using UnityEngine;
using System.Collections.Generic;

public class BoxCleanupManager : MonoBehaviour
{
    private static BoxCleanupManager _instance = null; //static/global reference to the single instance of the object because singleton pattern
    public static BoxCleanupManager Instance { get { return _instance; } } //safe way for other objects to reference GameManager without changing the ref

    private List<GameObject> _toDestroy = new List<GameObject>();

    [SerializeField] private int destroyCount = 5;

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

    public void AddObjectToDestroy(GameObject obj)
    {
        _toDestroy.Add(obj);
        Debug.Log("Added object to destroy, count is now: " + _toDestroy.Count);

        if(_toDestroy.Count >= destroyCount)
        {
            foreach(GameObject go in _toDestroy)
            {
                if(go != null) Destroy(go);
            }
        }
    }
}
