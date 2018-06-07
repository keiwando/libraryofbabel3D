using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class BookUVMapper : MonoBehaviour {

	void Start () {

		Map(GetComponent<MeshFilter>().mesh);
	}

	private void Map(Mesh mesh) {

		var uv2 = new Vector2( 0, 1 );
		var uv3 = new Vector2( 1, 1 );
		var uv0 = new Vector2( 0, 0 );
		var uv1 = new Vector2( 1, 0 );

		var uvs = new List<Vector2>() {
			uv2, uv3, uv0, uv1	
		};

		mesh.SetUVs(0, uvs);
		mesh.SetUVs(1, uvs);
		//mesh.SetUVs(2, uvs);
		//mesh.SetUVs(3, uvs);
	}
}
