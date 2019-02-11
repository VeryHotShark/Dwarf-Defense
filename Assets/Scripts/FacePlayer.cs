using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour 
{

	Transform camTransform;

	// Use this for initialization
	void Start () 
	{
		camTransform = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 dir = (camTransform.position - transform.position).normalized;
		Quaternion lookRotation = Quaternion.LookRotation(dir);
		transform.rotation = Quaternion.Slerp(transform.rotation,lookRotation, Time.deltaTime * 10f);
	}
}
