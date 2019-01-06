using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_vr : MonoBehaviour {

    private CharacterController controller;
    public float speed = 1.5f;
    Rigidbody rigdbody;
    Vector3 movement;

    void Awake()
    {
        rigdbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
        {
            Vector2 InputPoint = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);
            float h = InputPoint.x;
            float v = InputPoint.y;
            Run(h, v);
        }
    }

    void Run(float h, float v)
    {
        movement.Set(h, 0, v);
        movement = movement.normalized * speed * Time.deltaTime;
        rigdbody.MovePosition(transform.position + movement);
    }
}
