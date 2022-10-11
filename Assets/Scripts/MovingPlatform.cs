using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
	public Transform startPoint;
	public Transform endPoint;
	public float travelTime;
	private Rigidbody rb;
	private Vector3 currentPos;

	CharacterController cc;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		currentPos = Vector3.Lerp(startPoint.position, endPoint.position,
			Mathf.Cos(Time.time / travelTime * Mathf.PI * 2) * -.5f + .5f);
		rb.MovePosition(currentPos);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == (int)Layer.Player)
			cc = other.GetComponent<CharacterController>();
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.layer == (int)Layer.Player)
			cc.Move(rb.velocity * Time.deltaTime);
	}
}
