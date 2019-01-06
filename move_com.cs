using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_com : MonoBehaviour {

    public float speed = 3f;
    Rigidbody rigd;
    Vector3 movement;

	void Awake () {
        rigd = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Run(h, v);
    }

    void Run(float h, float v)
    {
        movement.Set(h, 0, v);
        movement = movement.normalized * speed * Time.deltaTime;
        rigd.MovePosition(transform.position + movement);
    }
}
