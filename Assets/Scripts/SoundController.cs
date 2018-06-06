using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour {

	[SerializeField] private AudioSource source;

	[SerializeField] private AudioClip pageTurn;
	[SerializeField] private AudioClip closeBook;
	[SerializeField] private AudioClip[] music;

	private int currentMusicIndex;

	private Coroutine musicLoop;

	void Start () {
		currentMusicIndex = 3;

		PlayMusic();
	}
 	
	public void PlayMusic() {

		if (!Settings.MusicEnabled)
			return;

		StopMusic();

		musicLoop = StartCoroutine(LoopThroughMusic());
	}

	private IEnumerator LoopThroughMusic() {

		while (true) {

			var currentMusic = music[currentMusicIndex];
			var length = currentMusic.length;
			source.PlayOneShot(currentMusic);

			yield return new WaitForSecondsRealtime(length);
			currentMusicIndex = NextMusicIndex(currentMusicIndex);
		}
	}

	private int NextMusicIndex(int currentIndex) {
		if(Application.platform == RuntimePlatform.IPhonePlayer)
			return 3;
		else
			return (currentIndex + 1) % music.Length;
	}
	
	public void PageFlip() {
		if (Settings.SoundEnabled)
			source.PlayOneShot(pageTurn);
	}

	public void BookClose() {
		if (Settings.SoundEnabled)
			source.PlayOneShot(closeBook);
	}

	public void StopMusic() {
		
		source.Stop();
		if (musicLoop != null) {
			StopCoroutine(musicLoop);
		}
	}

	public static SoundController Find() {
		return GameObject.FindGameObjectWithTag("SoundController").GetComponent<SoundController>();
	}
}
