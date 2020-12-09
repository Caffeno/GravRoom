using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorDetector : MonoBehaviour
{
    // Start is called before the first frame update

    private SphereCollider floorCheckCollider;
    [HideInInspector] public LayerMask groundLayers;

    private RotatingCharacterController characterController;
    void Start()
    {
        characterController = GetComponentInParent<RotatingCharacterController>();
    }
    [HideInInspector] public bool isGrounded = false;
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Hello World");

        if ((groundLayers.value & collision.collider.gameObject.layer) != 0 && isGrounded == false)
        {
            isGrounded = true; 
            characterController.FloorUpdate(isGrounded);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isGrounded)
        {
            Collider[] overlaping = Physics.OverlapSphere(transform.position, floorCheckCollider.radius, groundLayers);
            if (overlaping.Length < 0)
            {
                return;
            }
            else
            {
                isGrounded = false;
                characterController.FloorUpdate(isGrounded);
            }
        }
    }
}
