using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour 
{
	 public Transform target;
	 private Camera cam;

	 void Start()
	 {
		 cam = GetComponent<Camera> ();
	 }
	 void LateUpdate()
	 {
	 	if(target != null)
	 	{
			Vector3 desiredPos = target.position;
			desiredPos.y = cam.transform.position.y;
			cam.transform.position = desiredPos;
			cam.transform.rotation = Quaternion.Euler (90f, target.rotation.eulerAngles.y, 0f);
	 	}
	 }
 }
