using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (CapsuleCollider))]
public class RotatingCharacterController : MonoBehaviour
{

    private CapsuleCollider playerCollider;

    [SerializeField] private LayerMask groundLayers;
    [SerializeField] private int maxItteration = 5;
    public float skinThickness = 0.1f;

    private SphereCollider floorCheckCollider;
    public delegate void OnFloorHandler(bool grounded);

    public event OnFloorHandler OnFloorUpdate;

    [HideInInspector]
    public bool isGrounded = false;


    private void Start()
    {
        playerCollider = GetComponent<CapsuleCollider>();
    }

    public Vector3 Move(Vector3 moveAmount)
    {
        //Check from the bottom of the player in the direction of gravity if there is ground
        bool ground = Physics.Raycast(transform.position - transform.up * (playerCollider.height / 2 - skinThickness), Physics.gravity, 0.05f + skinThickness);

        Debug.DrawRay(transform.position - transform.up * (playerCollider.height / 2), Physics.gravity, Color.red, 0.05f);
        isGrounded = false;
        if (ground)
        {
            Debug.Log("ther");

            float gravityDotProduct = Vector3.Dot(moveAmount, -Physics.gravity);
            if (gravityDotProduct < 0)
            {
                //set vertal speed to 0
                moveAmount -= gravityDotProduct * (-Physics.gravity) / (Physics.gravity.sqrMagnitude);
            }
            isGrounded = true;
        }
        transform.position += moveAmount * Time.deltaTime;

        return moveAmount;
    }

    public void FloorUpdate(bool newGroundState)
    {
        OnFloorUpdate(newGroundState);
    }

    private void LateUpdate()
    {

    }
}
