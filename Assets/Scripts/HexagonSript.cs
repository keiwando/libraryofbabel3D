using UnityEngine;
using System.Collections;

public class HexagonSript : MonoBehaviour {

	public int direction;	//0-5
	public GameObject prevHex;
	public GameObject nextHex;

	public GameObject bookwall1;
	public GameObject bookwall2;
	public GameObject bookwall3;
	public GameObject bookwall4;
	private GameObject[] bookwalls;

	[SerializeField] private GhoulScript ghoul;
	public bool bugFixed;

	// Use this for initialization
	void Start () {
		bookwalls = new GameObject[4] {bookwall1,bookwall2,bookwall3,bookwall4};
		//bugFixed = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public GameObject[] getBookWalls(){
		return bookwalls;
	}

	public void disableWalls(){
		for(int i = 0; i < 4; i++){ 
			bookwalls[i].SetActive(false);
		}
		if(nextHex != null){
			nextHex.GetComponent<HexagonSript>().disableWalls();
		}
	}

	public void enableWalls(){
		for(int i = 0; i < 4; i++){
			bookwalls[i].SetActive(true);
		}
		//nextHex should always be disabled
		if(nextHex != null){
			nextHex.GetComponent<HexagonSript>().disableWalls();
		}
	}

	public void respawnGhoul(){
		ghoul.respawn();
	}

	public void activateGhoul(){
		ghoul.setShouldRead(true);
	}

	public void deactivateGhoul(){
		ghoul.setShouldRead(false);
	}

	public void removeWallNumberBug(){
		//make sure wall with the name BookWall 4 has the wallnumber of 4
		bookwall4.GetComponent<WallScript>().wallNumber = 3;
		//remove mesh bug as well
		//remove last three child objects of bookwall4 (duplicate meshes)

		if(!bugFixed){
			for(int i = 1; i <= 6; i++){
				if(i == 1 || i == 2 || i == 3 || i == 6){
					Transform child = bookwall4.transform.GetChild(bookwall4.transform.childCount - i);
					//safety name check
					string name = child.gameObject.name;
					if(name == "Combined mesh0" || name == "Combined mesh1" || name == "Combined mesh2"){
						Destroy(child.gameObject);
					}
				}
			}
			bugFixed = true;
		}
	}

	public void setBugFixed(){
		bugFixed = true;
	}

	public void removeFirstHexMesh(){
		Transform child = bookwall4.transform.GetChild(bookwall4.transform.childCount - 3);
		if(child.gameObject.name == "Combined mesh0"){
			Destroy(child.gameObject);
		}
	}
}
