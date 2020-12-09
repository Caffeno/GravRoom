using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour
{

    [SerializeField] private float gravityStrength = 9.8f;

    public delegate void OnGravityHandler(Vector3 gravityDirection);

    public event OnGravityHandler OnGravityShift;

    private Vector3 gravity;

    private void Update()
    {
        Vector3 gravityDirection = Vector3.zero;
        if (Input.GetButtonDown("1")) { gravityDirection = -transform.up; }
        else if (Input.GetButtonDown("2")) { gravityDirection = transform.up; }
        else if (Input.GetButtonDown("3")) { gravityDirection = transform.right; }
        else if (Input.GetButtonDown("4")) { gravityDirection = -transform.right; }
        else if (Input.GetButtonDown("5")) { gravityDirection = transform.forward; }
        else if (Input.GetButtonDown("6")) { gravityDirection = -transform.forward; }
        gravity = gravityDirection * gravityStrength;
        if (gravityDirection != Vector3.zero && gravity != Physics.gravity)
        {
            Physics.gravity = gravity;
            OnGravityShift(gravityDirection);
        }
    }
}
