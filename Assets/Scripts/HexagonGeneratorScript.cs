﻿using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections;

public class HexagonGeneratorScript : MonoBehaviour {

	public FirstPersonController fpsController;

	public LibrarianScript librarian;

	public CombineChildren parentCombine;

	public GameObject hexagon;	//firstHexagon

	public HexagonScript mainHex;
	public HexagonScript hexBelow;
	public HexagonScript hexBefore;

	public float heightDifference;
	public float xDiagDif;	
	public float zDiagDif;
	public float zVertDif;
	private const float rotation = 60f;

	private GameObject lastHex;	//the last spawned hexagon
	private const int numHexAbove = 4;	//number of Hexagons that should be spawned above current hex
	private const int numHexBelow = 3;	//number of Hexagons that should be spawned below current hex
	private const int hexColNumber = numHexAbove + numHexBelow + 1;	//number of hexagons in a column
	private GameObject currentHex;	//the hexagon the librarian is currently in
	private int spawnDistance;	//the distance from the librarian that hexagons spawn and despawn 

	private GameObject[] currentHexColumn;
	private GameObject[] currentHexRow;
	private const int currentHexnumberInRow = 1;

	//trigger stuff
	private Vector3 posBefore;
	private Vector3 posAfter;


	void Start () {
		lastHex = null;

		initializeArrays();

		//createFirstRow();

		//generateHexagonColumn();

		//wallbugCurrentHex
		hexagon.GetComponent<HexagonScript>().setBugFixed();
		hexagon.GetComponent<HexagonScript>().activateGhoul();

		//Invoke("firstHexBug",2f);

		//updateEnabledBookWalls();		//  <-- Nullpointer
		//parentCombine.Combine();
	}

	IEnumerator wait(System.Action complete){
		yield return new WaitForSeconds(1);
	}

	private void firstHexBug(){
		hexagon.GetComponent<HexagonScript>().removeFirstHexMesh();
	}

	public void TriggerEntered(Collider collider){
		posBefore = fpsController.transform.position;
	}

	public void TriggerLeft(Collider collider, Vector3 inwardVector) {

		posAfter = fpsController.transform.position;
		Vector3 dif = posAfter - posBefore;
		if (dif.magnitude <= 3)
			return; // The player hasn't moved through the trigger

		// deactivate ghoul
		var currentHex = mainHex;// currentHexRow[currentHexnumberInRow].GetComponent<HexagonScript>();

		currentHex.deactivateGhoul();

		var directionAngle = Vector3.Angle(inwardVector, dif);

		var destHex = directionAngle < 90 ? currentHex : hexBefore; // currentHex.nextHex;
		//HexagonScript destHex = destHexGO.GetComponent<HexagonScript>();

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

		currentHex.respawnGhoul();
		currentHex.activateGhoul();
		currentHex.removeWallNumberBug();
	}

	public void StairTriggerLeft(Collider collider) {

		var playerPos = fpsController.transform.position;
		var dif = playerPos - posBefore;

		if (dif.magnitude <= 3)
			return; // The player hasn't moved through the trigger

		var currentHex = mainHex; //currentHexRow[currentHexnumberInRow].GetComponent<HexagonScript>();

		currentHex.deactivateGhoul();

		HexagonScript nextHex = currentHex;
		HexagonScript lastHex = currentHex;

		if (dif.y > 0) {
			librarian.movedToRoomAbove();

			nextHex = hexBelow; // currentHexColumn[numHexBelow - 1].GetComponent<HexagonScript>();
			lastHex = currentHex;
			//fpsController.transform.Rotate(Vector3.up, -60.0f);
			//fpsController.yRotationOffset -= 60.0f;
			//librarian.transform.RotateAround(librarian.transform.
		} else {
			librarian.movedToRoomBelow();

			nextHex = currentHex;
			lastHex = hexBelow; // currentHexColumn[numHexBelow - 1].GetComponent<HexagonScript>();
			//fpsController.yRotationOffset += 60.0f;
			//fpsController.transform.Rotate(Vector3.up, 60.0f);
		}

		var outCollider = nextHex.transform.Find("Staircase Trigger").GetComponent<Collider>();

		var currentParent = fpsController.transform.parent;

		fpsController.transform.SetParent(lastHex.transform);

		var localPos = fpsController.transform.localPosition;
		fpsController.transform.SetParent(nextHex.transform, false);
		fpsController.transform.localPosition = localPos;

		fpsController.transform.SetParent(currentParent, true);

		var currentColRotOffset = Quaternion.Inverse(collider.transform.rotation) * fpsController.transform.rotation;
		fpsController.rotationOffset *= (Quaternion.Inverse(Quaternion.Inverse(outCollider.transform.rotation) * fpsController.transform.rotation) * currentColRotOffset);

		currentHex.respawnGhoul();
		currentHex.activateGhoul();
		currentHex.removeWallNumberBug();
	}

	/*public void triggerLeft(Collider collider,Vector3 inwardVector){

		TriggerLeft(collider, inwardVector);
		return;

		posAfter = fpsController.transform.position;
		Vector3 dif = posAfter - posBefore;
		//dif magnitude has to be bigger than 3 for the librarian to have moved through one side and come out the other
		if(dif.magnitude > 3){

			//deactivate ghoul
			currentHexRow[currentHexnumberInRow].GetComponent<HexagonScript>().deactivateGhoul();

			//compare dif-Vector to inwardVector
			if(Vector3.Angle(inwardVector,dif) < 90){
				//going into hex
				//last hexagon has to move to front
				GameObject lastHex = currentHexRow[0];
				shiftArrayObjectsBackward(currentHexRow);
				currentHexRow[3] = lastHex;
				//update next hex
				currentHexRow[2].GetComponent<HexagonScript>().nextHex = lastHex;
				lastHex.GetComponent<HexagonScript>().nextHex = null;
				//move actual position
				moveHexagon(lastHex,lastHex.GetComponent<HexagonScript>().direction,4);
				//moveHexagonColumn
				moveHexagonColumn((lastHex.GetComponent<HexagonScript>().direction));

				//tell librarian
				librarian.movedToNextRoom();
			}else{
				//going out of hex
				//first hexagon has to move to end
				GameObject firstHex = currentHexRow[3];
				shiftArrayObjectsForward(currentHexRow);
				currentHexRow[0] = firstHex;
				//update Next Hex
				currentHexRow[3].GetComponent<HexagonScript>().nextHex = null;
				firstHex.GetComponent<HexagonScript>().nextHex = currentHexRow[1];
				//move actual position
				moveHexagon(firstHex,firstHex.GetComponent<HexagonScript>().direction,-4);
				//moveHexagonColumn
				moveHexagonColumn((firstHex.GetComponent<HexagonScript>().direction + 3) % 6);

				//tell librarian
				librarian.movedToPreviousRoom();
			}
			//respawn ghouls in current room
			HexagonScript currentHexagon = currentHexRow[currentHexnumberInRow].GetComponent<HexagonScript>();
			currentHexagon.respawnGhoul();
			currentHexagon.activateGhoul();
			currentHexagon.removeWallNumberBug();


			//DEBUG
			//listArrayItems(currentHexRow);
		}
	}*/

	public void stairTriggerLeft(Collider collider){
		posAfter = fpsController.transform.position;
		Vector3 dif = posAfter - posBefore;
		float heightDif = posAfter.y - posBefore.y;
		
		//dif magnitude has to be bigger than 3 for the librarian to have moved through one side and come out the other
		if(dif.magnitude > 3){
			
			//deactivate ghoul
			currentHexRow[currentHexnumberInRow].GetComponent<HexagonScript>().deactivateGhoul();

			if(heightDif < 0){
				print ("going down");
				//going down
				//top hexagon has to move to bottom

				//save nextHexBelow
				GameObject nextHexBelow = currentHexColumn[numHexBelow - 1].GetComponent<HexagonScript>().nextHex;

				GameObject topHex = currentHexColumn[7];
				shiftArrayObjectsForward(currentHexColumn);
				currentHexColumn[0] = topHex;
				//move actual position
				moveHexagonUp(topHex,-1);

				//moveHexagonRowUpAndTurn
				moveHexagonRowUp(-1);

				//turn by 120
				topHex.transform.RotateAround(topHex.transform.position,Vector3.down,2*rotation);
				updateHexDirection(topHex,-2);
				GameObject nextHex = topHex.GetComponent<HexagonScript>().nextHex;
				if(nextHex != null){ 
					nextHex.transform.RotateAround(topHex.transform.position,Vector3.down,2*rotation);
					updateHexDirection(nextHex,-2);
				}

				dealWithAboveBelowNextHex(1,nextHexBelow,1);

				//tell librarian
				librarian.movedToRoomBelow();

			}else{
				print("going up");
				//going up
				//bottom hexagon has to move to top

				//save nextHexAbove
				GameObject nextHexAbove = currentHexColumn[numHexBelow + 1].GetComponent<HexagonScript>().nextHex;

				GameObject bottomHex = currentHexColumn[0];
				shiftArrayObjectsBackward(currentHexColumn);
				currentHexColumn[7] = bottomHex;
				//move actual position
				moveHexagonUp(bottomHex,1);

				//moveHexagonRowUpAndTurn
				moveHexagonRowUp(1);
				
				//turn by -120
				bottomHex.transform.RotateAround(bottomHex.transform.position,Vector3.down,-2*rotation);
				updateHexDirection(bottomHex,2);
				GameObject nextHex = bottomHex.GetComponent<HexagonScript>().nextHex;
				if(nextHex != null){
					nextHex.transform.RotateAround(bottomHex.transform.position,Vector3.down,-2*rotation);
					updateHexDirection(nextHex,2);
				}

				dealWithAboveBelowNextHex(-1,nextHexAbove,-1);

				//tell librarian
				librarian.movedToRoomAbove();
			}
			//respawn ghouls in current room
			HexagonScript currentHexagon = currentHexRow[currentHexnumberInRow].GetComponent<HexagonScript>();
			currentHexagon.respawnGhoul();
			currentHexagon.activateGhoul();
			currentHexagon.removeWallNumberBug();

			updateEnabledBookWalls();
		}
	}

	/*
	 * mode: 1 = up, else = down
	 */ 
	private void dealWithAboveBelowNextHex(int mode,GameObject nextHex,int factor){
		if(mode == 1){
			//next hex below has to go up

			//rotate by 2 * rotation
			if(nextHex != null) nextHex.transform.RotateAround(currentHexColumn[numHexBelow].transform.position,Vector3.up,factor * rotation);
			updateHexDirection(nextHex,factor);
			//move up by two heightDifferences
			Vector3 newPosition = nextHex.transform.position;
			newPosition.y += heightDifference;
			nextHex.transform.position = newPosition;
			//add as next Hex to the right Hexagon
			currentHexColumn[numHexBelow + 1].GetComponent<HexagonScript>().nextHex = nextHex;
		}else{
			//has to go down
			//rotate by 2 * rotation
			if(nextHex != null) nextHex.transform.RotateAround(currentHexColumn[numHexBelow].transform.position,Vector3.up,factor * rotation);
			updateHexDirection(nextHex,factor);
			//move up by two heightDifferences
			Vector3 newPosition = nextHex.transform.position;
			newPosition.y -= heightDifference;
			nextHex.transform.position = newPosition;
			//add as next Hex to the right Hexagon
			currentHexColumn[numHexBelow - 1].GetComponent<HexagonScript>().nextHex = nextHex;
		}
	}

	/*
	 * up: 1 = up, -1 = down
	 */ 
	private void moveHexagonUp(GameObject hex,int up){
		Vector3 newPosition = hex.transform.position;
		newPosition.y += (hexColNumber * heightDifference * up);
		hex.transform.position = newPosition;

		//recursion

		if(hex.GetComponent<HexagonScript>().nextHex != null){
			moveHexagonUp(hex.GetComponent<HexagonScript>().nextHex,up);
		}


	}

	private void moveHexagonRowUp(int up){
		for(int i = 0; i < 4; i++){
			if(i != currentHexnumberInRow){
				Vector3 newPosition = currentHexRow[i].transform.position;
				newPosition.y += (heightDifference * up);
				currentHexRow[i].transform.position = newPosition;
				//rotate
				currentHexRow[i].transform.RotateAround(currentHexRow[currentHexnumberInRow].transform.position,Vector3.up,up*rotation);
				/*
				HexagonScript script = currentHexRow[i].GetComponent<HexagonScript>();
				script.direction = (script.direction + up) % 6;		//up == up * 1
				if(script.direction < 0){
					script.direction += 5;														
					print ("direction += 5");
				}
				*/
				updateHexDirection(currentHexRow[i],up);
			}
		}
		//setup next objects
		int belowOrAbove = numHexBelow - 1;
		if(up < 0)	belowOrAbove = numHexBelow + 1; 
		
		HexagonScript scr = currentHexColumn[belowOrAbove].GetComponent<HexagonScript>();
		scr.nextHex = null;
		scr.prevHex = null;
		scr = currentHexColumn[numHexBelow].GetComponent<HexagonScript>();
		scr.nextHex = currentHexRow[currentHexnumberInRow + 1];
		scr.prevHex = currentHexRow[currentHexnumberInRow - 1];

		currentHexRow[currentHexnumberInRow] = currentHexColumn[numHexBelow];
	}

	/*
	 * moves the complete column except for the hexagon that is also part of the row
	 */ 
	private void moveHexagonColumn(int direction){
		for(int i = 0; i < currentHexColumn.Length; i++){
			if(i != numHexBelow){
				GameObject hexToMove = currentHexColumn[i];
				HexagonScript script = hexToMove.GetComponent<HexagonScript>();
				moveHexagon(hexToMove,direction,1);
				if(script.nextHex != null)	moveHexagon(script.nextHex,direction,1);
			}
		}
		currentHexColumn[numHexBelow] = currentHexRow[currentHexnumberInRow];
	}

	private void updateHexDirection(GameObject hex, int factor){
		HexagonScript script = hex.GetComponent<HexagonScript>();
			script.direction = (script.direction + factor) % 6;	//-up == - up * 1
			if(script.direction < 0) script.direction += 6;
	}

	private void generateHexagonColumn(){
		for(int i = numHexBelow -1; i >= 0; i--){
			currentHexColumn[i] = generateHexagonBelow();
		}

		currentHexColumn[3] = lastHex = hexagon;

		for(int i = 4; i < numHexAbove + 4; i++){
			currentHexColumn[i] = generateHexagonAbove();
		}
	}
	
	private GameObject generateHexagonAbove(){
		GameObject copyHex;
		if(lastHex == null) copyHex = hexagon;
		else copyHex = lastHex;

		GameObject newHex = Instantiate(copyHex);
		newHex.transform.SetParent(parentCombine.transform);

		Vector3 newPos = newHex.gameObject.transform.position;
		newPos.y += heightDifference;
		newHex.gameObject.transform.position = newPos;
		newHex.gameObject.transform.RotateAround(newHex.transform.position, Vector3.down, -rotation);

		HexagonScript script = newHex.GetComponent<HexagonScript>();
		//script.removeWallNumberBug();
		script.direction = (script.direction + 1) % 6;
		script.nextHex = null;
		script.prevHex = null;
		lastHex = newHex;

		//createNextHexagon
		createNextHexagon(newHex,1);
		return newHex;
	}

	private GameObject generateHexagonBelow(){
		GameObject copyHex;
		if(lastHex == null) copyHex = hexagon;
		else copyHex = lastHex;

		GameObject newHex = Instantiate(copyHex);
		newHex.transform.SetParent(parentCombine.transform);

		Vector3 newPos = newHex.gameObject.transform.position;
		newPos.y -= heightDifference;
		newHex.gameObject.transform.position = newPos;
		newHex.gameObject.transform.RotateAround(newHex.transform.position, Vector3.down, rotation);

		HexagonScript script = newHex.GetComponent<HexagonScript>();
		//script.removeWallNumberBug();
		script.direction--;
		if(script.direction < 0) script.direction = 5;
		script.nextHex = null;
		script.prevHex = null;
		lastHex = newHex;

		//createNextHexagon
		createNextHexagon(newHex,1);
		return newHex;
	}

	/*
	 *	dir != -1 => creates NEXT Hexagon
	 *	dir = -1 => creates PREVIOUS Hexagon
	 */
	private GameObject createNextHexagon(GameObject hex, int dir){
		int direction = hex.GetComponent<HexagonScript>().direction;

		if(dir == -1){
			direction += 3;
			direction = direction % 6;
		}

		float moveX = 0;	//left-right
		float moveZ = 0;	//up-down
		switch(direction){
		case 0:	moveX = 0;
				moveZ = zVertDif;
			break;
		case 1: 
				moveX = xDiagDif;
				moveZ = zDiagDif;
			break;
		case 2:
				moveX = xDiagDif;
				moveZ = -zDiagDif;
			break;
		case 3:
				moveX = 0;
				moveZ = -zVertDif;
			break;
		case 4:
				moveX = -xDiagDif;
				moveZ = -zDiagDif;
			break;
		case 5:
				moveX = -xDiagDif;
				moveZ = zDiagDif;
			break;
		}

		GameObject newHex = Instantiate(hex);
		newHex.transform.SetParent(parentCombine.transform);

		Vector3 newPosition = newHex.transform.position;
		newPosition.x += moveX;
		newPosition.z += moveZ;
		newHex.transform.position = newPosition;
		newHex.GetComponent<HexagonScript>().nextHex = null;

		hex.GetComponent<HexagonScript>().nextHex = newHex;

		return newHex;
	}

	private void moveHexagon(GameObject hex, int direction, int factor){
		float moveX = 0;	//left-right
		float moveZ = 0;	//up-down
		switch(direction){
		case 0:	moveX = 0;
			moveZ = zVertDif;
			break;
		case 1: 
			moveX = xDiagDif;
			moveZ = zDiagDif;
			break;
		case 2:
			moveX = xDiagDif;
			moveZ = -zDiagDif;
			break;
		case 3:
			moveX = 0;
			moveZ = -zVertDif;
			break;
		case 4:
			moveX = -xDiagDif;
			moveZ = -zDiagDif;
			break;
		case 5:
			moveX = -xDiagDif;
			moveZ = zDiagDif;
			break;
		}

		Vector3 newPosition = hex.transform.position;
		newPosition.x += (moveX * factor);
		newPosition.z += (moveZ * factor);
		hex.transform.position = newPosition;
	}

	private void createFirstRow(){

		HexagonScript script = hexagon.GetComponent<HexagonScript>();

		currentHexRow[1] = hexagon;
		//create Hexagon next to first Hexagon
		GameObject nextHex = script.nextHex = createNextHexagon(hexagon,1);
		currentHexRow[2] = nextHex;
		//create previous hex to first hexagon;
		currentHexRow[0] = createNextHexagon(hexagon,-1);
		script.prevHex = currentHexRow[0];
		script.nextHex = currentHexRow[2];
		//create hexagon next to hexagon.next
		currentHexRow[3] = script.nextHex.GetComponent<HexagonScript>().nextHex = createNextHexagon(nextHex,1);
	}

	private void updateEnabledBookWalls(){
		//top two and bottom hexagons don't need shelves and books
		currentHexColumn[0].GetComponent<HexagonScript>().disableWalls();

		currentHexColumn[hexColNumber - 1].GetComponent<HexagonScript>().disableWalls();
		currentHexColumn[hexColNumber - 2].GetComponent<HexagonScript>().disableWalls();

		//enable the rest
		currentHexColumn[1].GetComponent<HexagonScript>().enableWalls();
		currentHexColumn[hexColNumber - 3].GetComponent<HexagonScript>().enableWalls();

	}

	private void initializeArrays(){
		currentHexColumn = new GameObject[hexColNumber];
		//there's always 4 hexagons in one row
		currentHexRow = new GameObject[4];
	}

	/*
	 * all elements have to be moved -1
	 * last array Element (array.length -1) is empty
	 */
	private void shiftArrayObjectsForward(GameObject[] array){
		for(int i = array.Length - 2; i >= 0; i--){
			array[i+1] = array[i];
		}
		array[0] = null;
	}

	/*
	 * all elements have to be moved +1
	 * first array Element is empty
	 */
	private void shiftArrayObjectsBackward(GameObject[] array){
		for(int i = 0; i < array.Length - 1; i++){
			array[i] = array[i+1];
		}
		array[array.Length - 1] = null;
	}

	private void listArrayItems(GameObject[] array){
		for(int i = 0; i < array.Length; i++){
			print(array[i].name);
		}
	}
}
