using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	public GameObject car;
	public bool followCar = false;

	void Update () {
		if (followCar)
		{
			transform.position = new Vector3(car.transform.position.x, car.transform.position.y, -100);
		}
	}
}
