using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement.OnCharacterJump.AddListener(OnJump);
    }

    private void OnJump()
    {
        animator.SetTrigger("Jump");
    }

    private void LateUpdate()
    {
        animator.SetFloat("HorizontalSpeed", playerMovement.GetHorizontalRBVelocity().magnitude);
        animator.SetBool("IsFalling", playerMovement.rb.linearVelocity.y < 0.01f);
    }
}
