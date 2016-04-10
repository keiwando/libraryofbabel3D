using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour {

	[SerializeField] private AudioSource audio;

	[SerializeField] private AudioClip pageTurn;
	[SerializeField] private AudioClip closeBook;
	[SerializeField] private AudioClip[] music;

	private int currentMusic;
	private float time;
	private float lastTimeStop;

	private int musicNumber;

	void Start () {
		currentMusic = 3;
		lastTimeStop = Time.realtimeSinceStartup;
		musicNumber = music.Length;

		if(PlayerPrefs.GetInt("MUSIC") == 1)
			audio.PlayOneShot(music[currentMusic]);
	}
	
	// Update is called once per frame
	void Update () {
		playMusic();
	}

	private void playMusic(){
		if(PlayerPrefs.GetInt("MUSIC") == 1){
			time = Time.realtimeSinceStartup - lastTimeStop;
			if(time >= music[currentMusic].length + 4){
				currentMusic = (currentMusic + 1) % musicNumber;
				audio.PlayOneShot(music[currentMusic]);
				lastTimeStop = Time.realtimeSinceStartup;
			}
		}
	}

	public void startMusic(){
		currentMusic = (currentMusic + 1) % musicNumber;
		audio.PlayOneShot(music[currentMusic]);
		lastTimeStop = Time.realtimeSinceStartup;
	}
	
	public void pageFlip(){
		audio.PlayOneShot(pageTurn);
	}

	public void bookClose(){
		audio.PlayOneShot(closeBook);
	}

	public void stopMusic(){
		audio.Stop();
	}
}
