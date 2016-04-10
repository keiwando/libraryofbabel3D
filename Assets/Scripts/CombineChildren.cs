using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[AddComponentMenu("Mesh/Combine Children")]
public class CombineChildren : MonoBehaviour {

	private bool completed;
	[SerializeField] private bool DEBUG;

	void Start()
	{
		if(DEBUG){
			//print("START called on: " + gameObject.name + "in hex: " + transform.parent.gameObject.name);
			StartCoroutine(wait());
		}
		completed = false;

		Matrix4x4 myTransform = transform.worldToLocalMatrix;
		Dictionary<Material, List<CombineInstance>> combines = new Dictionary<Material, List<CombineInstance>>();
		MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();
		foreach (var meshRenderer in meshRenderers)
		{
			foreach (var material in meshRenderer.sharedMaterials)
				if (material != null && !combines.ContainsKey(material))
					combines.Add(material, new List<CombineInstance>());
		}
		
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		//if(DEBUG) print("Number of filters: " + meshFilters.Length);
		foreach(var filter in meshFilters)
		{
			if (filter.sharedMesh == null)
				continue;
			CombineInstance ci = new CombineInstance();
			ci.mesh = filter.sharedMesh;
			ci.transform = myTransform * filter.transform.localToWorldMatrix;
			combines[filter.GetComponent<Renderer>().sharedMaterial].Add(ci);
			if(DEBUG){
				filter.GetComponent<Renderer>().enabled = true;
			}else{
				filter.GetComponent<Renderer>().enabled = false;
			}
		}

		int counter = 0;
		//if(DEBUG) print("Number of materials: " + combines.Keys.Count);
		foreach(Material m in combines.Keys)
		{
			
			if(counter < 3){
				var go = new GameObject("Combined mesh" + counter);
				go.transform.parent = transform;
				go.transform.localPosition = Vector3.zero;
				go.transform.localRotation = Quaternion.identity;
				go.transform.localScale = Vector3.one;
				
				var filter = go.AddComponent<MeshFilter>();
				filter.mesh.CombineMeshes(combines[m].ToArray(), true, true);
				
				var renderer = go.AddComponent<MeshRenderer>();
					renderer.material = m;
			}
			if(DEBUG) counter++;

		}


		completed = true;
	}

	public bool getCompleted(){
		return completed;
	}

	private IEnumerator wait(){
		yield return new WaitForSeconds(2);
	}
}