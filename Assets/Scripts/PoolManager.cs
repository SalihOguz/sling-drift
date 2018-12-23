using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour {
	public GameObject poolObject;
	public GameObject roadsParent;
	int currentAngle = 0; // angle to put road sprite according to the previous one
	Vector3 lastRoadPut = Vector3.zero;
	int lastRoadType = 0;
	int lastRoadAngle = 0;
	int[] lastSpriteTypes = new int[2]; // last 6 sprite type, 0 = straight, 1 = right, 2 = left
	GameObject currentRoadObject;

	void Start () {
		//straightPoolParent = poolObject.transform.GetChild(0);
		Init();
	}

	void Init()
	{
		lastSpriteTypes[0] = 0;
		lastSpriteTypes[1] = 0;

		for (int i = 0; i < 4; i++)
		{
			PutSprite(ChooseSpriteToPut());
		}
	}

	int ChooseSpriteToPut()
	{
		int newSprite;
		if (lastSpriteTypes[1] == 0) // last sprite was straight
		{
			if (lastSpriteTypes[0] == 0 || UnityEngine.Random.Range(0,100) < 70) // chance of turning after straight sprite
			{
				newSprite = UnityEngine.Random.Range(1,3);
			}
			else
			{
				newSprite = 0;
			}
		}
		else // last sprite was right or left
		{
			if (UnityEngine.Random.Range(0,100) < 80) // chance of straight after turning sprite
			{
				newSprite = 0;
			}
			else
			{
				if (lastSpriteTypes[0] == lastSpriteTypes[1]) // if last two were the same, 1-1 or 2-2
				{
					newSprite = 0;
				}
				else
				{
					newSprite = UnityEngine.Random.Range(1,3);
				}
			}
		}
		
		lastSpriteTypes[0] = lastSpriteTypes[1];
		lastSpriteTypes[1] = newSprite;
		
		return newSprite;
	}


	void PutSprite(int spriteType)
	{
		currentRoadObject = poolObject.transform.GetChild(spriteType).GetChild(0).gameObject;
		currentRoadObject.transform.SetParent(roadsParent.transform.GetChild(currentRoadObject.transform.parent.GetSiblingIndex()));

		currentRoadObject.transform.localEulerAngles = new Vector3(0, 0, currentAngle);
		Vector2 spriteSize = currentRoadObject.GetComponent<SpriteRenderer>().size;
		int directionMultiplier = 1;

		// find if position need to be increased or decreased
		if (lastRoadType == 0)
		{
			if (lastRoadAngle == 90 || lastRoadAngle == -270 || lastRoadAngle == 180 || lastRoadAngle == -180)
			{
				directionMultiplier = -1;
			}
		}
		if (lastRoadType == 1)
		{
			if (lastRoadAngle == -90 || lastRoadAngle == 270 || lastRoadAngle == 180 || lastRoadAngle == -180)
			{
				directionMultiplier = -1;
			}
		}
		if (lastRoadType == 2) // must look at the currentAngle here with lastRoadType
		{
			if (lastRoadAngle == 0|| lastRoadAngle == 90 || lastRoadAngle == -270)
			{
				directionMultiplier = -1;
			}
		}

		lastRoadPut = lastRoadPut + (directionMultiplier * new Vector3(Mathf.Abs(Mathf.Sin(Mathf.Deg2Rad* currentAngle) * 11.05f), Mathf.Abs(Mathf.Cos(Mathf.Deg2Rad*currentAngle) * 11.05f), 0));
		currentRoadObject.transform.position = lastRoadPut;
		currentRoadObject.SetActive(true);

		lastRoadAngle = currentAngle % 360;
		if (spriteType == 1)
		{
			currentAngle -= 90;
		}
		else if (spriteType == 2)
		{
			currentAngle += 90;
		}
		lastRoadType = spriteType;
	}

	public void PutRoadBack(GameObject roadObject)
	{
		roadObject.SetActive(false);
		roadObject.transform.SetParent(poolObject.transform.GetChild(roadObject.transform.parent.GetSiblingIndex()));
		roadObject.transform.position = Vector3.zero;
		roadObject.transform.localEulerAngles = Vector3.zero;
		if (roadObject.transform.childCount > 1) // if has a node, reset it
		{
			roadObject.transform.GetChild(1).localEulerAngles = Vector3.zero;
		}


		PutSprite(ChooseSpriteToPut());
	}
}