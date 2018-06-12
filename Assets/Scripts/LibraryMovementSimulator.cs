using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class LibraryMovementSimulator : MonoBehaviour {

	private const float yOffset = 7f;
	private const float xBaseOffset = 30.8f;

	[SerializeField]
	private Hexagon mainHex;
	[SerializeField]
	private Hexagon hexBelow;
	[SerializeField]
	private Hexagon hexBefore;

	[SerializeField]
	private GameObject[] movables;

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
		var currentHex = mainHex;

		currentHex.DeactivateGhoul();

		var directionAngle = Vector3.Angle(inwardVector, dif);

		var destHex = directionAngle < 90 ? currentHex : hexBefore;

		var outCollider = destHex.transform.Find("Hallway Trigger").GetComponent<Collider>();

		if (directionAngle < 90) {
			// going into hex
			librarian.MovedToNextRoom();
			MovedToNextRoom();
		} else {
			// going out of hex
			librarian.MovedToPreviousRoom();
			MovedToPreviousRoom();
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

		var currentHex = mainHex;

		currentHex.DeactivateGhoul();

		Hexagon nextHex = currentHex;
		Hexagon lastHex = currentHex;

		if (dif.y > 0) {
			librarian.MovedToRoomAbove();
			MovedToRoomAbove();

			nextHex = hexBelow;
			lastHex = currentHex;

		} else {
			librarian.MovedToRoomBelow();
			MovedToRoomBelow();

			nextHex = currentHex;
			lastHex = hexBelow;
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

	private void MovedToNextRoom() {

		ShiftOnSameFloor(1);
	}

	private void MovedToPreviousRoom() {
		
		ShiftOnSameFloor(-1);
	}

	private void MovedToRoomAbove() {
		ShiftToDifferentFloor(1);
	}

	private void MovedToRoomBelow() {
		ShiftToDifferentFloor(-1);
	}

	private void ShiftOnSameFloor(int direction) {

		var offset = direction * (mainHex.transform.position - hexBefore.transform.position);
	
		var angle = Mathf.PI * direction / 3f; // = Mathf.PI * (direction * 60f) / (180f);
		var dX = Mathf.Sin(angle) * xBaseOffset;
		var dZ = Mathf.Cos(angle) * xBaseOffset;

		dX = offset.x;
		dZ = offset.z;

		foreach (var obj in movables) {

			var pos = obj.transform.position;
			pos.x += dX;
			pos.z += dZ;
			obj.transform.position = pos;
		}
	}

	private void ShiftToDifferentFloor(int offset) {

		var rotation = offset * -60.0f;

		foreach (var obj in movables) {

			obj.transform.RotateAround(mainHex.transform.position, Vector3.up, rotation);

			var pos = obj.transform.position;
			// Shift all of the movable objects against the logical movement direction of the librarian
			pos.y += -offset * yOffset;
			obj.transform.position = pos;
		}
	}

	public static LibraryMovementSimulator Find() {

		return GameObject.FindGameObjectWithTag("LibraryMovementSimulator").GetComponent<LibraryMovementSimulator>();
	}
}

