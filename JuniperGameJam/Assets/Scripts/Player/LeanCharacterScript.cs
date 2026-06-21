using System;
using UnityEngine;

public class LeanCharacterScript : MonoBehaviour
{
    [SerializeField] private Rigidbody charRb;
    [SerializeField] private float maxLean = 30f;
   
    // Update is called once per frame
    void Update()
    {

        Vector3 MovementDirection = transform.parent.InverseTransformDirection(charRb.linearVelocity);
        float targetLeanZ = -MovementDirection.x * maxLean;
        float targetLeanX = MovementDirection.z * maxLean;

       
        Quaternion targetRotation = Quaternion.Euler(targetLeanX, 0f, targetLeanZ);

      
        transform.localRotation = Quaternion.Lerp(transform.localRotation,targetRotation,Time.deltaTime * 5f);
    }
}
