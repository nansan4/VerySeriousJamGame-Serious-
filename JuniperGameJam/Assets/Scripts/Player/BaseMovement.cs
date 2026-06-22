using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]

public abstract class BaseMovement : MonoBehaviour
{
    [Header("Base - Movement")]
    public float currentMaxSpeed = 7f; //max move speed of character

    [Header("Base - Character Input")]
    public Vector2 movementInput; //2d movement from controller

    [Header("Base - Component/Object References")]
    [SerializeField] protected new Collider collider;
    public new Rigidbody rb; //public so can be read by other scripts (like animation)
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected Transform characterModel; //ref to object can contains visuals
    [SerializeField] protected Transform characterMesh; //ref to object can contains visuals

    //lets controller object give us movement instructions
    public virtual void SetMovementInput(Vector2 moveInput) //virtual keyword lets us override this func in a child class, implementation will fall back to that of parent class if not overriden
    {
        movementInput = moveInput;
    }

    //all child objects MUST implement this func (like an interface)
    protected abstract void Move();
    //all child classes can optionally implement this
    protected virtual void Rotate()
    {
        //do nothing
    }

    public virtual void Launch() //basically our jump func
    {
        //do nothing
    }

    public virtual void Fall() //Reduce height
    {
        //do nothing
    }

    public virtual void CancelLaunch()
    {
        //do nothing
    }

    public virtual void CancelFall()
    {
        //do nothing
    }

    //helper func, can't be overriden but is available to child funcs
    public Vector3 GetHorizontalRBVelocity()
    {
        return new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
    }
}
