using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public Sprite[] carSprites;
	public GameObject car;
	public GameObject canvas;

	void Start()
	{
		car.GetComponent<SpriteRenderer>().sprite = carSprites[PlayerPrefs.GetInt("ChosenCar")];
	}

	public void ChangeCar(int id)
	{
		PlayerPrefs.SetInt("ChosenCar", id);
		car.GetComponent<SpriteRenderer>().sprite = carSprites[id];
	}

	public void StartTheGame()
	{
		canvas.GetComponent<Animator>().SetTrigger("Start");
		car.GetComponent<Car>().hasCrashed = false;
		StartCoroutine(MakeCameraFollow());
	}

	IEnumerator MakeCameraFollow()
	{
		yield return new WaitForSeconds(0.25f);
		GetComponent<CameraManager>().followCar = true;
	}

	public void Restart()
	{
		SceneManager.LoadScene("Game");
	}

	public void ShowEndScreen(int score)
	{
		canvas.transform.GetChild(3).gameObject.SetActive(true);
		canvas.transform.GetChild(3).GetChild(0).GetComponent<Text>().text = score.ToString();
		canvas.transform.GetChild(3).GetChild(1).GetComponent<Text>().text = "High Score: " + PlayerPrefs.GetInt("HighScore").ToString();
	}
}