using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour {
	bool isInDriftZone = false;
	bool isDrifting = false;
	GameObject currentNode;
	LineRenderer lineRenderer;
	float car_speed = 0.16f;
	GameObject[] oldRoads = new GameObject[3];
	public bool hasCrashed = true;
	public AudioClip[] sounds;
	AudioSource audioSource;
	int score = 0;

	void Start () {
		lineRenderer = GetComponent<LineRenderer>();
		audioSource = GetComponent<AudioSource>();
	}

	void Update () 
	{
		if (!hasCrashed)
		{
			if (isInDriftZone)
			{
				if (Input.GetMouseButton(0))
				{
					if (!isDrifting)
					{
						isDrifting = true;
						lineRenderer.enabled = true;
						PlaySound(0); // drifting sound
						//transform.SetParent(currentNode.transform);
					}
				}
				else if (Input.GetMouseButtonUp(0))
				{
					isDrifting = false;
					lineRenderer.enabled = false;
					transform.SetParent(null);
					audioSource.Stop();
					score++;
					//FindCarFinalAngle();
				}
			}

			if (isDrifting)
			{
				Vector3[] points = {transform.position, currentNode.transform.position};
				lineRenderer.SetPositions(points);

				if (currentNode.transform.parent.parent.GetSiblingIndex() == 2) // left turn
				{
					currentNode.transform.localEulerAngles += new Vector3(0,0,2.7f);
				}
				else if (currentNode.transform.parent.parent.GetSiblingIndex() == 1) // right turn
				{
					currentNode.transform.localEulerAngles -= new Vector3(0,0,2.7f);
				}

				if (transform.parent != currentNode.transform)
				{
					transform.SetParent(currentNode.transform);
				}
			}
			else
			{
				CarAngle();

				transform.position += new Vector3(car_speed * -Mathf.Sin(Mathf.Deg2Rad*transform.localEulerAngles.z), car_speed * Mathf.Cos(Mathf.Deg2Rad*transform.localEulerAngles.z), 0);
			}
		}
	}

	void CarAngle()
	{
		// fixing the angle
		Vector3 currentAngles = transform.localEulerAngles;
		float angleZ = currentAngles.z;

		if (Mathf.Abs(angleZ % 90) > 45)
		{
			transform.localEulerAngles = new Vector3(currentAngles.x, currentAngles.y, Mathf.Lerp(angleZ, angleZ + (90 - (angleZ % 90)), 3f * Time.deltaTime));
		}
		else
		{
			transform.localEulerAngles = new Vector3(currentAngles.x, currentAngles.y, Mathf.Lerp(angleZ, angleZ - (angleZ % 90), 3f * Time.deltaTime));
		}
	}

	void PlaySound(int id)
	{
		audioSource.Stop();
		audioSource.clip = sounds[id];
		audioSource.Play();
	}

	// void FindCarFinalAngle()
	// {
	// 	if (Mathf.Abs(transform.localEulerAngles.z % 90) > 45)
	// 	{
	// 		carFinalAngle = (90 - (transform.localEulerAngles.z % 90));
	// 	}
	// 	else
	// 	{
	// 		carFinalAngle = transform.localEulerAngles.z - (transform.localEulerAngles.z % 90);
	// 	}
	// 	Vector3 currentAngles = transform.localEulerAngles;
	// 	gameObject.transform.DORotate(new Vector3(currentAngles.x, currentAngles.y, carFinalAngle), 1f,RotateMode.LocalAxisAdd).SetEase(Ease.InOutBack);
	// }

	void Crashed()
	{
		hasCrashed = true;
		GetComponent<SpriteRenderer>().color = new Color(0.1f, 0.1f,0.1f,1);
		transform.GetChild(0).gameObject.SetActive(false);
		transform.GetChild(2).gameObject.SetActive(true);
		transform.SetParent(null);
		PlaySound(1);
		if (score > PlayerPrefs.GetInt("HighScore"))
		{
			PlayerPrefs.SetInt("HighScore", score);
		}
		Camera.main.GetComponent<GameManager>().ShowEndScreen(score);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "node")
		{
			isInDriftZone = true;
			currentNode = other.gameObject;
		}
		if (other.gameObject.tag == "obstacle")
		{
			Crashed();
		}
	}
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.gameObject == currentNode && !isDrifting)
		{
			isInDriftZone = false;	
			isDrifting = false;
			currentNode = null;
			lineRenderer.enabled = false;
			audioSource.Stop();
			//FindCarFinalAngle();
		}
		if (other.gameObject.tag == "road")
		{
			HandleOldRoad(other.transform.parent.gameObject);
		}
	}

	void HandleOldRoad(GameObject roadObject)
	{
		bool isFull = true;
		for (int i = 0; i < oldRoads.Length; i++)
		{
			if(oldRoads[i] == null)
			{
				oldRoads[i] = roadObject;
				isFull = false;
				break;
			}
		}

		if(isFull)
		{
			Camera.main.GetComponent<PoolManager>().PutRoadBack(oldRoads[0]);
			for (int i = 1; i < oldRoads.Length; i++)
			{
				oldRoads[i - 1] = oldRoads[i];
			}
			oldRoads[oldRoads.Length - 1] = roadObject;
		}
	}
}