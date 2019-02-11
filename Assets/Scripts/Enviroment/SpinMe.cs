using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
	public class SpinMe : MonoBehaviour {
		
		[SerializeField] float xRotationsPerMinute = 1f;
		[SerializeField] float yRotationsPerMinute = 1f;
		[SerializeField] float zRotationsPerMinute = 1f;
		
		float xDegreesPerFrame;
		float yDegreesPerFrame;
		float zDegreesPerFrame;
		
		void Start()
		{
			
		}
		
		void Update () {
			xDegreesPerFrame = xRotationsPerMinute / 60 * 360 * Time.deltaTime;
			transform.RotateAround (transform.position, transform.right, xDegreesPerFrame);
			
			yDegreesPerFrame = yRotationsPerMinute / 60 * 360 * Time.deltaTime;
			transform.RotateAround (transform.position, transform.up, yDegreesPerFrame);
			
			zDegreesPerFrame = zRotationsPerMinute / 60 * 360 * Time.deltaTime;
			transform.RotateAround (transform.position, transform.forward, zDegreesPerFrame);
		}
	}	
}