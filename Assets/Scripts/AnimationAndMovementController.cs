using Cinemachine;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AnimationAndMovementController : MonoBehaviour
{
    private Animator animator;
    private CharacterController characterController;

    private int VelocityZHash;
    private int VelocityXHash;

    private float velocityZ = 0.0f;
    private float velocityX = 0.0f;

    [SerializeField] private float accelerationZ = 1.0f;
    [SerializeField] private float accelerationX = 1.0f;
    [SerializeField] private float decelerationZ = 1.5f;
    [SerializeField] private float decelerationX = 1.5f;

    [SerializeField] private float maxWalkVelocity = 0.5f;
    [SerializeField] private float maxSprintVelocity = 1f;

    [SerializeField] private float idleSpeed = 0.0f;
    [SerializeField] private float stopLimit = 0.01f;

    private GameObject virtualCamera;

    void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        VelocityZHash = Animator.StringToHash("VelocityZ");
        VelocityXHash = Animator.StringToHash("VelocityX");

        virtualCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    void Update()
    {
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool backwardPressed = Input.GetKey(KeyCode.S);
        bool rightPressed = Input.GetKey(KeyCode.D);    
        bool sprintPressed = Input.GetKey(KeyCode.LeftShift);

        float currentMaxVelocity;
        if (sprintPressed && !backwardPressed)
        {
            currentMaxVelocity = maxSprintVelocity;
        }
        else
        {
            currentMaxVelocity = maxWalkVelocity;
        }

        ChangeVelocity(forwardPressed, backwardPressed, leftPressed, rightPressed, sprintPressed, currentMaxVelocity, maxWalkVelocity);

        animator.SetFloat(VelocityZHash, velocityZ);    
        animator.SetFloat(VelocityXHash, velocityX);

        
    }

    private void ChangeVelocity(bool forwardPressed, bool backwardPressed, bool leftPressed, bool rightPressed, bool sprintPressed, float currentMaxVelocity, float maxWalkVelocity)
    {
        // Acceleration
        if (forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * accelerationZ;
        }
        if (backwardPressed && velocityZ > -maxWalkVelocity)
        {
            velocityZ -= Time.deltaTime * accelerationZ;
        }
        if (rightPressed && velocityX < currentMaxVelocity)
        {
            velocityX += Time.deltaTime * accelerationX;
        }
        if (leftPressed && velocityX > -currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * accelerationX;
        }


        // Deceleration
        if (!forwardPressed && velocityZ > idleSpeed)
        {
            velocityZ -= Time.deltaTime * decelerationZ;
        }
        if (!backwardPressed && velocityZ < idleSpeed)
        {
            velocityZ += Time.deltaTime * decelerationZ;
        }
        if (!rightPressed && velocityX > idleSpeed)
        {
            velocityX -= Time.deltaTime * decelerationX;
        }
        if (!leftPressed && velocityX < idleSpeed)
        {
            velocityX += Time.deltaTime * decelerationX;
        }

        // Velocity reset
        if (!forwardPressed && !backwardPressed && velocityZ != 0.0f && Mathf.Abs(velocityZ) < stopLimit)
        {
            velocityZ = 0.0f;
        }
        if (!leftPressed && !rightPressed && velocityX != 0.0f && Mathf.Abs(velocityX) < stopLimit)
        {
            velocityX = 0.0f;
        }


        // Z Sprint Forward
        if (forwardPressed && sprintPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ = currentMaxVelocity;
        }
        else if (forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * decelerationZ;
            if (velocityZ > currentMaxVelocity && velocityZ < (currentMaxVelocity + stopLimit))
            {
                velocityZ = currentMaxVelocity;
            }
        }
        else if (forwardPressed && velocityZ < currentMaxVelocity && velocityZ > (currentMaxVelocity - stopLimit))
        {
            velocityZ = currentMaxVelocity;
        }

        // Z Sprint Backward (No sprint)
        if (backwardPressed && sprintPressed && velocityZ < -maxWalkVelocity)
        {
            velocityZ = -maxWalkVelocity;
        }
        else if (backwardPressed && velocityZ < -maxWalkVelocity)
        {
            velocityZ += Time.deltaTime * decelerationZ;
            if (velocityZ < maxWalkVelocity && velocityZ > (-maxWalkVelocity - stopLimit))
            {
                velocityZ = -maxWalkVelocity;
            }
        }
        else if (backwardPressed && velocityZ > -maxWalkVelocity && velocityZ < (-maxWalkVelocity + stopLimit))
        {
            velocityZ = -maxWalkVelocity;
        }

        // X Sprint Left
        if (leftPressed && !backwardPressed && sprintPressed && velocityX < -currentMaxVelocity)
        {
            velocityX = -currentMaxVelocity;
        }
        else if (leftPressed && velocityX < -currentMaxVelocity)
        {
            velocityX += Time.deltaTime * decelerationX;
            if (velocityX < -currentMaxVelocity && velocityX > (-currentMaxVelocity - stopLimit))
            {
                velocityX = -currentMaxVelocity;
            }
        }
        else if (leftPressed && velocityX > -currentMaxVelocity && velocityX < (-currentMaxVelocity + stopLimit))
        {
            velocityX = -currentMaxVelocity;
        }

        // X Sprint Right
        if (rightPressed && !backwardPressed && sprintPressed && velocityX > currentMaxVelocity)
        {
            velocityX = currentMaxVelocity;
        }
        else if (rightPressed && velocityX > currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * decelerationX;
            if (velocityX > currentMaxVelocity && velocityX < (currentMaxVelocity + stopLimit))
            {
                velocityX = currentMaxVelocity;
            }
        }
        else if (rightPressed && velocityX < currentMaxVelocity && velocityX > (currentMaxVelocity - stopLimit))
        {
            velocityX = currentMaxVelocity;
        }
    }
}
