using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Require component gerir það að því componenti
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour {

	//setjum upp vertices í lista sv ovið getum teiknað hexagonnanna
	Mesh hexMesh;
	MeshCollider meshCollider;

	static List<Vector3> vertices= new List<Vector3>();
	static List<int> triangles = new List<int> ();
	static List<Color> colors = new List<Color>();

	//List<Vector3> vertices;
	//List<int> triangles;
	//List<Color> colors;

	void Awake () {
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		meshCollider = gameObject.AddComponent<MeshCollider>();
		hexMesh.name = "Hex Mesh";
		//vertices = new List<Vector3>();
		//colors = new List<Color>();
		//triangles = new List<int>();
	}

	/**
	 * Þurfum að byrja á að cleara gamal dataið svo það sé ekki teiknað aftur
	 * 
	*/
	public void Triangulate (HexCell[] cells) {
		hexMesh.Clear();
		vertices.Clear();
		triangles.Clear();
		colors.Clear();
		for (int i = 0; i < cells.Length; i++) {
			Triangulate(cells[i]);
		}
		hexMesh.vertices = vertices.ToArray();
		hexMesh.triangles = triangles.ToArray();
		hexMesh.colors = colors.ToArray();
		hexMesh.RecalculateNormals();

		//assignum mesh við cooliderinn
		meshCollider.sharedMesh = hexMesh;
	}

	//gefur hnit til að teikna þríhyrninga
	void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3) {
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}

	//teiknar þríhyrning með punkt í miðju og tvo af hornunum
	//endurtökum 6 sinnum til 
	void Triangulate (HexCell cell) {
		for (int i = 0; i < 6; i++) {
			Vector3 center = cell.transform.localPosition;
			AddTriangle (
				center,
				center + HexMetrics.corners [i],
				center + HexMetrics.corners [i+1]
			);

			AddTriangleColor(cell.Color);
		}
	}

	void AddTriangleColor (Color color) {
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}
}
