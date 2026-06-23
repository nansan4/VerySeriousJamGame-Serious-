using UnityEngine;

public class PropellerSpinScript : MonoBehaviour
{
    [SerializeField] private GameObject PropellerBone;
    [SerializeField] private float RotateSpeed = 3f;
    // Update is called once per frame
    void Update()
    {
        PropellerBone.transform.Rotate(0, 0, RotateSpeed * Time.deltaTime);
    }
}
