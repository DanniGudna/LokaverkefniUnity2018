using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour {

	//breidd borðsins
	 int cellCountX;
	// public int width;
	//hæð borðsins
	int cellCountZ;
	//public int height;

	public int chunkCountX = 4, chunkCountZ = 3;


	//Debug text UI ofan á reitina, vitum þannig hnitin á þeim
	public Text coordinatesPrefab;
	Canvas gridCanvas;

	public HexCell cellPrefab;
	public HexGridChunk chunkPrefab;
	HexMesh hexMesh;

	HexCell[] cells;
	HexGridChunk[] chunks;


	public Color defaultColor = Color.white;
	public Color touchedColor = Color.magenta;

	//látum alla hexagons í fylki til að geta notað þá betur
	//búm til alla reitina sem við þurfum
	void Awake () {
		//náum í canvas objectið og látum inn gildi í hann
		gridCanvas = GetComponentInChildren<Canvas>();
		//náum í meshið og notum það til að teikna reitina
		hexMesh = GetComponentInChildren<HexMesh> ();

		cellCountX = chunkCountX * HexMetrics.chunkSizeX;
		cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

		CreateChunks();
		CreateMap ();

	}

	void Start () {
		hexMesh.Triangulate(cells);
	}


	void CreateMap () {
		cells = new HexCell[cellCountZ * cellCountX];

		for (int z = 0, i = 0; z < cellCountZ; z++) {
			for (int x = 0; x < cellCountX; x++) {
				CreateCell(x, z, i++);
			}
		}
	}

	void CreateChunks () {
		chunks = new HexGridChunk[chunkCountX * chunkCountZ];

		for (int z = 0, i = 0; z < chunkCountZ; z++) {
			for (int x = 0; x < chunkCountX; x++) {
				HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
				chunk.transform.SetParent(transform);
			}
		}
	}
		
	/* gamalt ekki notað lengur
	public void ColorCell (Vector3 position, Color color) {
		position = transform.InverseTransformPoint(position);
		Coordinates coordinates = Coordinates.FromPosition(position);
		Debug.Log("touched at " + coordinates.ToString());
		int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
		HexCell cell = cells[index];
		cell.color = color;
		//cell.color = touchedColor;
		hexMesh.Triangulate(cells);
	}
	*/

	//end



	/**
	 * 
	 * x, y og z eru hnit hvers cell fyrir sig
	 *  
	**/
	void CreateCell (int x, int z, int i) {
		Vector3 position;
		//notum hexagon gildin úr hexmetrics til að finna offsetin
		position.x = (x + z * 0.5f - z/2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		//cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = Coordinates.offsetCoordinates(x, z);
		cell.color = defaultColor;



		//stillum nágranna
		//þetta stillir að reiturinn til vinstri ( vestur W) sé nágranni, viljum ekki númer 0
		//fallið okkar í hex direction mun still svon nágranna til austurs með þessu
		if (x > 0) {
			cell.SetNeighbor (HexDirection.W, cells [i - 1]);
		}
		//stillum NE-SW
		if (z > 0) {
			//TODO: breyta í %
			if ((z & 1) == 0) {
				cell.SetNeighbor (HexDirection.SE, cells [i - cellCountX]);
				if (x > 0) {
					cell.SetNeighbor (HexDirection.SW, cells [i - cellCountX - 1]);
				}
			} else {
				cell.SetNeighbor (HexDirection.SW, cells [i - cellCountX]);
				if (x < cellCountX - 1) {
					cell.SetNeighbor (HexDirection.SE, cells [i - cellCountX + 1]);
				}
			}


		}


		int y = -x - z;
		//stillum nafnið á nýja objectinu
		cell.name= "x: " + x + " y: " + y +" z: " + z;

		//development canvas sem teiknar hnitin á reitina
		// drawMarkers(x,z,position, cell);

		AddCellToChunk (x, z, cell);

		Text label = Instantiate<Text> (coordinatesPrefab);
		// label.text = cell.coordinates.ToStringOnSeparateLines ();
		cell.uiRect = label.rectTransform;
	}


	void AddCellToChunk (int x, int z, HexCell cell) {
		int chunkX = x / HexMetrics.chunkSizeX;
		int chunkZ = z / HexMetrics.chunkSizeZ;
		HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

		int localX = x - chunkX * HexMetrics.chunkSizeX;
		int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
		chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
	}
		

	//skrifar textann á hvert cell, notað í development
	void drawMarkers (int x, int y, Vector3 position, HexCell cell){
		Text label = Instantiate<Text> (coordinatesPrefab);
		label.rectTransform.SetParent(gridCanvas.transform, false);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		//TODO: breyta , ekki taka inn cell og nota gamla mögulega
		// label.text = cell.coordinates.ToStringOnSeparateLines();
		//label.text = x.ToString() + "\n" + y.ToString();

	}


	public void FindDistancesTo (HexCell cell) {
		for (int i = 0; i < cells.Length; i++) {
			cell.coordinates.DistanceTo(cells[i].coordinates);
		}
	}

	/*public HexCell GetCell (Coordinates coordinates) {
		int z = coordinates.Z;
		if (z < 0 || z >= cellCountZ) {
			return null;
		}
		int x = coordinates.X + z / 2;
		if (x < 0 || x >= cellCountX) {
			return null;
		}
		return cells[x + z * cellCountX];
	}*/

	public HexCell GetCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		Coordinates coordinates = Coordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
		return cells[index];
	}

	// re triangulate cells
	public void Refresh () {
		hexMesh.Triangulate (cells);
	}

		
}
