using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Screen : MonoBehaviour
{
	public AudioSource music;
	public int startLevel;

	void Start ()
	{
		if (music) {
			music.Play ();
		}
	}

	void Update ()
	{
		if (Input.GetButtonDown ("Jump")) {
			loadNextScene ();
		}
	}

	public void loadNextScene ()
	{
		Debug.Log ("Loading Scene");
		SceneManager.LoadScene (startLevel);
	}
}
