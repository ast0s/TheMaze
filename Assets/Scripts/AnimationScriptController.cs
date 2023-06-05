using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationScriptController : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;
    int isSprintingHash;

    int VelocityHash;
    float velocityZ = 0.0f;

    [SerializeField] float acceleration = 0.5f;
    [SerializeField] float deceleration = 1f;
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isSprintingHash = Animator.StringToHash("isSprinting");
        VelocityHash = Animator.StringToHash("Velocity");
    }

    void Update()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isSprinting = animator.GetBool(isSprintingHash);

        bool forwardPressed = Input.GetKey("w");
        bool sprintPressed = Input.GetKey(KeyCode.LeftShift);

        // Walk
        if (forwardPressed && velocityZ < 0.3f)
        {
            animator.SetBool(isWalkingHash, true);
            velocityZ += Time.deltaTime * acceleration;
        }
        // Stop
        if (!forwardPressed && velocityZ > 0.0f) 
        {
            animator.SetBool(isWalkingHash, false);
            velocityZ -= Time.deltaTime * deceleration;
        }
        // Run
        if (forwardPressed && sprintPressed && velocityZ < 1.0f)
        {
            animator.SetBool(isSprintingHash, true);
            velocityZ += Time.deltaTime * acceleration;
        }
        // Stop running
        if (!forwardPressed && !sprintPressed && velocityZ > 0.0f)
        {
            animator.SetBool(isSprintingHash, false);
            velocityZ -= Time.deltaTime * deceleration;
        }
        if (!forwardPressed && velocityZ < 0.0f)
        {
            velocityZ = 0.0f;
        }

        animator.SetFloat(VelocityHash, velocityZ);
    }
}
