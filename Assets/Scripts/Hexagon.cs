using UnityEngine;
using System.Collections;

public class Hexagon : MonoBehaviour {

	public int direction; // Multiple of 60°

	public HexagonLocation location;

	private Wall[] walls;

	[SerializeField]
	private Wall firstWall;
	[SerializeField]
	private Transform[] wallTransforms;

	private GhoulScript ghoul;
	private Librarian librarian;

	[SerializeField]
	private string visibleWallMask = "0000";

	void Start () {
		
		//walls = GetComponentsInChildren<Wall>();
		walls = new Wall[wallTransforms.Length];
		ghoul = GetComponentInChildren<GhoulScript>();
		librarian = Librarian.Find();

		SetupWalls();
	}

	private void SetupWalls() {

		int visibilityMask = int.Parse(visibleWallMask);
		int counter = 1;

		for (int i = 0; i < wallTransforms.Length; i++) {

			bool visible = (counter & visibilityMask) == counter;
			counter++;

			if (!visible)
				continue;

			var wallGO = Instantiate(firstWall.gameObject);
			wallGO.SetActive(true);

			var wallTransform = wallTransforms[i];
			wallGO.transform.parent = firstWall.transform.parent;
			wallGO.transform.localPosition = wallTransform.localPosition;
			wallGO.transform.localRotation = wallTransform.localRotation;

			var wall = wallGO.GetComponent<Wall>();
			walls[i] = wall;
			wall.Hex = this;
			wall.Number = i + 1;
			wall.Setup(librarian);
		}

		firstWall.gameObject.SetActive(false);
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

	public HexagonLocation NextHexLocation() {
	
		var offset = (direction % 6) * 666 + 1;
		return HexLocationWithOffset(offset);
	}

	public HexagonLocation PrevHexLocation() {

		var offset = -((direction % 6) * 666 + 1);
		return HexLocationWithOffset(offset);
	}

	public HexagonLocation AboveLocation() {
		
		return HexLocationWithOffset(123456789);
	}

	public HexagonLocation BelowLocation() {
		
		return HexLocationWithOffset(-123456789);
	}

	private HexagonLocation HexLocationWithOffset(BigInteger offset) {

		var newNumber = location.Number + offset;

		if (newNumber > HexagonLocation.MAX) {
			newNumber -= HexagonLocation.MAX;
		} else if (newNumber < 0) {
			newNumber += HexagonLocation.MAX;
		}

		return HexagonLocation.FromNumber(newNumber);
	}
}
