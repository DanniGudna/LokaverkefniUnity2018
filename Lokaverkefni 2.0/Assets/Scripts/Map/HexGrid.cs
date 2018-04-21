using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour {

	//breidd borðsins
	 int cellCountX = 10;
	// public int width;
	//hæð borðsins
	int cellCountZ = 10;
	//public int height;

	public int chunkCountX = 2, chunkCountZ = 1;

	PriorityQueue searchFrontier;

	//Debug text UI ofan á reitina, vitum þannig hnitin á þeim
	public Text coordinatesPrefab;
	Canvas gridCanvas;

	public HexCell cellPrefab;
	public HexGridChunk chunkPrefab;
	HexMesh hexMesh;

	HexCell[] cells;
	HexGridChunk[] chunks;


	// public Color defaultColor = Color.white;
	// public Color touchedColor = Color.magenta;

	void Awake () {
		CreateMap(cellCountX, cellCountZ);
	}

	public bool CreateMap (int x, int z) {

		if (chunks != null) {
			for (int i = 0; i < chunks.Length; i++) {
				Destroy(chunks[i].gameObject);
			}
		}

		cellCountX = x;
		cellCountZ = z;
		chunkCountX = cellCountX / HexMetrics.chunkSizeX;
		chunkCountZ = cellCountZ / HexMetrics.chunkSizeZ;
		CreateChunks();
		CreateCells();
		return true;
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

	void CreateCells () {
		cells = new HexCell[cellCountZ * cellCountX];

		for (int z = 0, i = 0; z < cellCountZ; z++) {
			for (int x = 0; x < cellCountX; x++) {
				CreateCell(x, z, i++);
			}
		}
	}
		

	public HexCell GetCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		Coordinates coordinates = Coordinates.FromPosition(position);
		int index =
			coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
		return cells[index];
	}

	public HexCell GetCell (Coordinates coordinates) {
		int z = coordinates.Z;
		if (z < 0 || z >= cellCountZ) {
			return null;
		}
		int x = coordinates.X + z / 2;
		if (x < 0 || x >= cellCountX) {
			return null;
		}
		return cells[x + z * cellCountX];
	}
		

	void CreateCell (int x, int z, int i) {
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.localPosition = position;
		cell.coordinates = Coordinates.offsetCoordinates(x, z);


		//movemoentCost stilling
		cell.moveCost = cell.level [cell.index];


		//stillum nágranna
		//þetta stillir að reiturinn til vinstri ( vestur W) sé nágranni, viljum ekki númer 0
		//fallið okkar í hex direction mun still svon nágranna til austurs með þessu
		if (x > 0) {
			cell.SetNeighbor(HexDirection.W, cells[i - 1]);
		}
		if (z > 0) {
			if ((z & 1) == 0) {
				cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
				if (x > 0) {
					cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
				}
			}
			else {
				cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
				if (x < cellCountX - 1) {
					cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
				}
			}
		}

		int y = -x - z;
		//stillum nafnið á nýja objectinu
		cell.name= "x: " + x + " y: " + y +" z: " + z;

		Text label = Instantiate<Text>(coordinatesPrefab);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		cell.uiRect = label.rectTransform;


		AddCellToChunk(x, z, cell);
	}

	void AddCellToChunk (int x, int z, HexCell cell) {
		int chunkX = x / HexMetrics.chunkSizeX;
		int chunkZ = z / HexMetrics.chunkSizeZ;
		HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

		int localX = x - chunkX * HexMetrics.chunkSizeX;
		int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
		chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
	}

	public void FindPath (HexCell fromCell, HexCell toCell, int speed) {
		StopAllCoroutines ();
		StartCoroutine (Search (fromCell, toCell, speed));
	}
		

	IEnumerator Search (HexCell fromCell, HexCell toCell, int speed) {
		if (searchFrontier == null) {
			searchFrontier = new PriorityQueue ();
		} else {
			searchFrontier.Clear ();
		}
		for (int i = 0; i < cells.Length; i++) {
			cells [i].Distance = int.MaxValue;
			cells [i].DisableHighlight ();
		}

		fromCell.EnableHighlight (Color.white);
		toCell.EnableHighlight (Color.red);
		WaitForSeconds delay = new WaitForSeconds (1 / 60f);
		// List<HexCell> frontier = new List<HexCell> ();
		fromCell.Distance = 0;
		searchFrontier.Enqueue (fromCell);
		// frontier.Add (fromCell);

		while (searchFrontier.Count > 0) {
			yield return delay;
			HexCell current = searchFrontier.Dequeue ();
			// frontier.RemoveAt (0);

			if (current == toCell) {
				current = current.PathFrom;
				while (current != fromCell) {
					current.EnableHighlight (Color.blue);
					current = current.PathFrom;
				}
				break;
			}
			int currentTurn = current.Distance / speed;

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor (d);
				if (neighbor == null) {
					continue;
				}

				if (!neighbor.passable) {
					continue;
				}
				int distance = current.Distance;
				// int moveCost;
				// TODO:
				distance += neighbor.moveCost;

				int turn = distance / speed;
				//ef við erum ekki búnir að skoða þenna reit áður
				if (neighbor.Distance == int.MaxValue) {
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
					neighbor.searchHueristic = neighbor.coordinates.DistanceTo (toCell.coordinates);
					//frontier.Add (neighbor);
					searchFrontier.Enqueue (neighbor);
				} else if (distance < neighbor.Distance) {
					int oldPriority = neighbor.SearchPriority;
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
					searchFrontier.Change (neighbor, oldPriority);
				}
					
				// frontier.Sort((x , y) => x.SearchPriority.CompareTo(y.SearchPriority));
			
			}
		}
	}


	/*
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
		
}
