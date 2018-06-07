using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class LibraryMovementSimulator : MonoBehaviour {

	[SerializeField]
	private Hexagon mainHex;
	[SerializeField]
	private Hexagon hexBelow;
	[SerializeField]
	private Hexagon hexBefore;

	[SerializeField]
	private Fire fire;

	private Librarian librarian;
	private FirstPersonController fpController;

	private Vector3 posBefore;
	private Vector3 posAfter;

	void Start() {

		librarian = Librarian.Find();
		fpController = librarian.GetComponent<FirstPersonController>();
	}
	
	public void TriggerEntered(Collider collider){
		posBefore = fpController.transform.position;
	}

	public void TriggerLeft(Collider collider, Vector3 inwardVector) {

		posAfter = fpController.transform.position;
		Vector3 dif = posAfter - posBefore;
		if (dif.magnitude <= 3)
			return; // The player hasn't moved through the trigger

		// deactivate ghoul
		var currentHex = mainHex;// currentHexRow[currentHexnumberInRow].GetComponent<Hexagon>();

		currentHex.DeactivateGhoul();

		var directionAngle = Vector3.Angle(inwardVector, dif);

		var destHex = directionAngle < 90 ? currentHex : hexBefore; // currentHex.nextHex;
		//Hexagon destHex = destHexGO.GetComponent<Hexagon>();

		var outCollider = destHex.transform.Find("Hallway Trigger").GetComponent<Collider>();

		if (directionAngle < 90) {
			// going into hex
			librarian.movedToNextRoom();
		} else {
			// going out of hex
			librarian.movedToPreviousRoom();
		}

		var currentColPos = collider.transform.position;
		var currentColOffset = librarian.transform.position - currentColPos;
		var outColPos = outCollider.transform.position;

		librarian.transform.position = outColPos + currentColOffset;

		currentHex.RespawnGhoul();
		currentHex.ActivateGhoul();
	}

	public void StairTriggerLeft(Collider collider) {

		var playerPos = fpController.transform.position;
		var dif = playerPos - posBefore;

		if (dif.magnitude <= 3)
			return; // The player hasn't moved through the trigger

		var currentHex = mainHex; //currentHexRow[currentHexnumberInRow].GetComponent<Hexagon>();

		currentHex.DeactivateGhoul();

		Hexagon nextHex = currentHex;
		Hexagon lastHex = currentHex;

		if (dif.y > 0) {
			//librarian.movedToRoomAbove();

			nextHex = hexBelow; // currentHexColumn[numHexBelow - 1].GetComponent<Hexagon>();
			lastHex = currentHex;
			//fpsController.transform.Rotate(Vector3.up, -60.0f);
			//fpsController.yRotationOffset -= 60.0f;
			//librarian.transform.RotateAround(librarian.transform.
		} else {
			//librarian.movedToRoomBelow();

			nextHex = currentHex;
			lastHex = hexBelow; // currentHexColumn[numHexBelow - 1].GetComponent<Hexagon>();
			//fpsController.yRotationOffset += 60.0f;
			//fpsController.transform.Rotate(Vector3.up, 60.0f);
		}

		var outCollider = nextHex.transform.Find("Staircase Trigger").GetComponent<Collider>();

		var currentParent = fpController.transform.parent;

		fpController.transform.SetParent(lastHex.transform);

		var localPos = fpController.transform.localPosition;
		fpController.transform.SetParent(nextHex.transform, false);
		fpController.transform.localPosition = localPos;

		fpController.transform.SetParent(currentParent, true);

		var currentColRotOffset = Quaternion.Inverse(collider.transform.rotation) * fpController.transform.rotation;
		fpController.rotationOffset *= (Quaternion.Inverse(Quaternion.Inverse(outCollider.transform.rotation) * fpController.transform.rotation) * currentColRotOffset);

		currentHex.RespawnGhoul();
		currentHex.ActivateGhoul();
	}

	public static LibraryMovementSimulator Find() {

		return GameObject.FindGameObjectWithTag("LibraryMovementSimulator").GetComponent<LibraryMovementSimulator>();
	}
}

