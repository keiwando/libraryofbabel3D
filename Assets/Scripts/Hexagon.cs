using UnityEngine;
using System.Collections;

public class Hexagon : MonoBehaviour {

	public int direction; // Multiple of 60°

	private Wall[] walls;

	private GhoulScript ghoul;
	private Librarian librarian;

	[SerializeField]
	private string visibleWallMask = "0000";

	void Start () {
		
		walls = GetComponentsInChildren<Wall>();
		ghoul = GetComponentInChildren<GhoulScript>();
		librarian = GameObject.FindGameObjectWithTag("Librarian").GetComponent<Librarian>();

		SetupWalls();
	}

	private void SetupWalls() {

		int visibilityMask = int.Parse(visibleWallMask);
		int counter = 1;

		for (int i = 0; i < walls.Length; i++) {

			Wall wall = walls[i];

			bool visible = counter & visibilityMask == counter;
			counter++;

			if (visible) {
				wall.gameObject.SetActive(true);
				wall.Setup();
			} else {
				wall.gameObject.SetActive(false);
			}
		}
	}

	public void RespawnGhoul(){
		ghoul.respawn();
	}

	public void ActivateGhoul(){
		ghoul.setShouldRead(true);
	}

	public void DeactivateGhoul(){
		ghoul.setShouldRead(false);
	}
}
