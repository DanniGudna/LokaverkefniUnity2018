using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class HexGrid : MonoBehaviour {

	// mun fá gildi frá gameManager sem verða svo gildin, 10 er default value
	//breidd borðsins
	 int cellCountX = 10;
	// public int width;
	//hæð borðsins
	int cellCountZ = 10;

	public Material[] material;

	public int chunkCountX = 2, chunkCountZ = 1;

	PriorityQueue searchFrontier;

	int searchFrontierPhase;

	//Debug text UI ofan á reitina, vitum þannig hnitin á þeim
	public Text coordinatesPrefab;
	Canvas gridCanvas;

	public HexCell cellPrefab;
	public HexGridChunk chunkPrefab;
	HexMesh hexMesh;

	HexCell[] cells;
	HexGridChunk[] chunks;

	List<HexCell> tilesInRange;
	public List<HexCell> TilesInRange {
		get{
			return tilesInRange;
		}
	}
	List<HexCell> unitsInRange;
	public List<HexCell> UnitsInRange {
		get{
			return unitsInRange;
		}
	}

	HexCell currentPathFrom, currentPathTo, movementRange;
	public HexCell CurrentPathTo {
		get{
			return currentPathTo;
		}
	}
	bool currentPathExists;

	List<Unit> units = new List<Unit>();

	private MeshRenderer mesh;

	public bool HasPath {
		get {
			return currentPathExists;
		}
	}


	// public Color defaultColor = Color.white;
	// public Color touchedColor = Color.magenta;

	void Awake () {
		CreateMap(cellCountX, cellCountZ);
	}

	/// <summary>
	/// Creates the map.
	/// </summary>
	/// <returns><c>true</c>, if map was created, <c>false</c> otherwise.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
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

	/// <summary>
	/// Creates the chunks.
	/// </summary>
	void CreateChunks () {
		chunks = new HexGridChunk[chunkCountX * chunkCountZ];

		for (int z = 0, i = 0; z < chunkCountZ; z++) {
			for (int x = 0; x < chunkCountX; x++) {
				HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
				chunk.transform.SetParent(transform);
			}
		}
	}

	/**
	 * Creates all the cells
	 * the number of cells is defined by cellCountX and Z
	 **/
	void CreateCells () {
		cells = new HexCell[cellCountZ * cellCountX];

		for (int z = 0, i = 0; z < cellCountZ; z++) {
			for (int x = 0; x < cellCountX; x++) {
				CreateCell(x, z, i++);
			}
		}
	}


	/// <summary>
	/// Finds the distances to all the heccell from a given cell.
	/// </summary>
	/// <param name="cell">Cell.</param>
	public void FindDistancesTo (HexCell cell){
		for (int i = 0; i < cells.Length; i++) {
			cells[i].Distance = cell.coordinates.DistanceTo(cells[i].coordinates);
		}
	}
		

	/// <summary>
	/// Gets the cell.
	/// </summary>
	/// <returns>The cell.</returns>
	/// <param name="position">Position.</param>
	public HexCell GetCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		Coordinates coordinates = Coordinates.FromPosition(position);
		int index =
			coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
		return cells[index];
	}

	/// <summary>
	/// Gets the cell from coordinates.
	/// </summary>
	/// <returns>The cell from coordinates.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	public HexCell GetCellFromCoordinates (int x, int z) {
		return cells[x + z * cellCountX];
	}


	/// <summary>
	/// Gets the cell.
	/// </summary>
	/// <returns>The cell.</returns>
	/// <param name="coordinates">Coordinates.</param>
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
		
	/// <summary>
	/// Creates the cell.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	/// <param name="i">The index.</param>
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
		mesh = cell.GetComponent<MeshRenderer> ();
		mesh.materials [0] = material [0];

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

	/// <summary>
	/// Adds the cell to chunk.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	/// <param name="cell">Cell.</param>
	void AddCellToChunk (int x, int z, HexCell cell) {
		int chunkX = x / HexMetrics.chunkSizeX;
		int chunkZ = z / HexMetrics.chunkSizeZ;
		HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

		int localX = x - chunkX * HexMetrics.chunkSizeX;
		int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
		chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
	}

	/// <summary>
	/// Finds the path.
	/// </summary>
	/// <param name="fromCell">From cell.</param>
	/// <param name="toCell">To cell.</param>
	/// <param name="speed">Speed.</param>
	public void FindPath (HexCell fromCell, HexCell toCell, int speed) {
		ClearPath();
		currentPathFrom = fromCell;
		currentPathTo = toCell;
		currentPathExists = Search(fromCell, toCell, speed);
		ShowPath();

	}

	/// <summary>
	///  Finds all the tiles a unit can reach and shows them
	/// </summary>
	/// uses reachableTiles to find the tiles and highlightReach to higlight
	/// <param name="fromCell">From cell.</param>
	/// <param name="speed">Speed.</param>
	public void FindReachableTiles (HexCell fromCell, int speed) {

		ClearTilesInRange();
		tilesInRange = reachableTiles (fromCell, speed);
		HighlightTilesInRange ();

	}

	/// <summary>
	/// Finds the attackable tiles.
	/// </summary>
	/// <param name="fromCell">From cell.</param>
	/// <param name="range">Range.</param>
	public void FindAttackableTiles(HexCell fromCell, int range, int team){

		//ClearAttackable ();
		unitsInRange = attackableTiles (fromCell, range);
		HighlightAttackableTiles (team);

	}

	/// <summary>
	/// Gets the cell under the ray.
	/// </summary>
	/// <returns>The cell.</returns>
	/// <param name="ray">Ray.</param>
	public HexCell GetCell (Ray ray) {
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit)) {
			return GetCell(hit.point);
		}
		return null;
	}
		

	/// <summary>
	/// Checks if there is a path from fromCell to toCell and how long it takes with the given speed
	/// </summary>
	/// <param name="fromCell">From cell.</param>
	/// <param name="toCell">To cell.</param>
	/// <param name="speed">Speed.</param>
	bool Search (HexCell fromCell, HexCell toCell, int speed) {
		searchFrontierPhase += 2;
		if (searchFrontier == null) {
			searchFrontier = new PriorityQueue ();
		} else {
			searchFrontier.Clear ();
		}

		fromCell.EnableHighlight (Color.white);
		fromCell.SearchPhase = searchFrontierPhase;
		fromCell.Distance = 0;
		searchFrontier.Enqueue (fromCell);

		while (searchFrontier.Count > 0) {
			HexCell current = searchFrontier.Dequeue ();

			current.SearchPhase += 1;

			if (current == toCell) {
				return true;
			}
			int currentTurn = (current.Distance - 1) / speed;

			//ef við erum komin yfir það sem við náum á einni umfeðr þá hættum við leit
			if((current.Distance/speed) > 0){
				break;
			}

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor (d);
				if (neighbor == null ||
					neighbor.SearchPhase > searchFrontierPhase) {
					continue;
				}

				if (!neighbor.passable) {
					continue;
				}
					
				int moveCost = 0;
				// TODO:
				moveCost += neighbor.moveCost;

				int distance = current.Distance + moveCost;
				int turn = ( distance - 1 ) / speed;

				//eydir movementi sem kall a eftir ef hann er ekki med nog til ad fara a naesta reit
				if (turn > currentTurn) {
					distance = turn * speed + moveCost;
				}
				//ef við erum ekki búnir að skoða þenna reit áður
				if (neighbor.SearchPhase < searchFrontierPhase) {
					neighbor.SearchPhase = searchFrontierPhase;
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
					neighbor.searchHueristic = neighbor.coordinates.DistanceTo (toCell.coordinates);
					searchFrontier.Enqueue (neighbor);
				} else if (distance < neighbor.Distance) {
					int oldPriority = neighbor.SearchPriority;
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
					searchFrontier.Change (neighbor, oldPriority);
				}
			
			}
		}
		return false;
	}


	/// <summary>
	/// Shows the path.
	/// </summary>
	/// <param name="speed">Speed.</param>
	public void ShowPath (/*int speed*/) {
		if (currentPathExists) {
			HexCell current = currentPathTo;
			while (current != currentPathFrom) {
				//int turn = (current.Distance-1) / speed;
				current.EnableHighlight(Color.white);
				current = current.PathFrom;
				current.turnsToReach = 0;
			}
		}
		currentPathFrom.EnableHighlight(Color.blue);
	}

	/// <summary>
	/// Clears the path.
	/// </summary>
	public void ClearPath () {
		if (currentPathExists) {
			HexCell current = currentPathTo;
			while (current != currentPathFrom) {
				current.SetLabel(null);
				//TODO: bretua i infinity
				current.turnsToReach = 1000;
				current.DisableHighlight();
				current = current.PathFrom;
			}
			current.DisableHighlight();
			currentPathExists = false;
		}
		currentPathFrom = currentPathTo = null;
	}

	public void ShowUI (bool visible) {
		for (int i = 0; i < chunks.Length; i++) {
			chunks[i].ShowUI(visible);
		}
	}

	/// <summary>
	/// Gets the path.
	/// </summary>
	/// <returns>The path.</returns>
	public List<HexCell> GetPath () {
		if (!currentPathExists) {
			print ("ekki til");
			return null;
		}
		List<HexCell> path = ListPool<HexCell>.Get();
		for (HexCell c = currentPathTo; c != currentPathFrom; c = c.PathFrom) {
			path.Add(c);
		}
		path.Add(currentPathFrom);
		path.Reverse ();
		return path;
	}

	///<summary> <c>reachableTiles</c> Finnur alla reiti sem ákveðinn kall getur náð
	/// </summary>
	/// <returns> A list of HexCell that can be reached from a HexCell with the current speed</returns>
	/// <param name="fromcell">fromCell</param>
	/// <para name "speed"> speed</para>
	public List<HexCell> reachableTiles ( HexCell fromCell, int speed){
		List<HexCell> reachableTiles = new List<HexCell>();
		searchFrontierPhase += 2;
		if (searchFrontier == null) {
			searchFrontier = new PriorityQueue ();
		} else {
			searchFrontier.Clear ();
		}

		//fromCell.EnableHighlight (Color.white);
		fromCell.SearchPhase = searchFrontierPhase;
		fromCell.Distance = 0;
		searchFrontier.Enqueue (fromCell);

		while (searchFrontier.Count > 0) {

			HexCell current = searchFrontier.Dequeue ();

			current.SearchPhase += 1;

			int currentTurn = (current.Distance - 1) / speed;
			if (currentTurn > 0) {
				continue;
			} else if (currentTurn == 0) {
				reachableTiles.Add (current);
				//current.EnableHighlight(Color.green);
			}

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor (d);
				if (neighbor == null ||
					neighbor.SearchPhase > searchFrontierPhase) {
					continue;
				}

				if (!neighbor.passable) {
					continue;
				}

				//check ef óvina unit er fyrir
				if (neighbor.Unit != null) {
					if (neighbor.Unit.Team != fromCell.Unit.Team) {
						continue;
					}
				}

				//int distance = current.Distance;
				int moveCost = 0;
				// TODO:
				moveCost += neighbor.moveCost;

				int distance = current.Distance + moveCost;
				int turn = ( distance - 1 ) / speed;
				//eydir movementi sem kall a eftir ef hann er ekki med nog til ad fara a naesta reit
				if (turn > currentTurn) {
					distance = turn * speed + moveCost;
				}
				//ef við erum ekki búnir að skoða þenna reit áður
				if (neighbor.SearchPhase < searchFrontierPhase) {
					neighbor.SearchPhase = searchFrontierPhase;
					neighbor.Distance = distance;
					searchFrontier.Enqueue (neighbor);
				} else if (distance < neighbor.Distance) {
					int oldPriority = neighbor.SearchPriority;
					neighbor.Distance = distance;
					neighbor.PathFrom = current;
					searchFrontier.Change (neighbor, oldPriority);
				}

			}
		}
		return reachableTiles;
	}


	/// <summary>
	/// Attackables the tiles.
	/// </summary>
	/// <returns>The tiles.</returns>
	/// <param name="fromCell">From cell.</param>
	/// <param name="range">Range.</param>
	public List<HexCell> attackableTiles ( HexCell fromCell, int range){
		List<HexCell> attackableTiles = new List<HexCell>();
		//TODO: gera hagkvaemara!!!
		for (int i = 0; i < cells.Length; i++) {
			cells[i].Distance = fromCell.coordinates.DistanceTo(cells[i].coordinates);
			//cells[i].SetLabel(cells[i].Distance.ToString());
			//if (cells[i].Distance > range){
			//	continue;
			//}
			if (cells[i].Distance <= range){
				attackableTiles.Add (cells [i]);
			}

		}
		return attackableTiles;
	}


	/// <summary>
	/// Highlights cells within reach.
	/// </summary>
	public void HighlightTilesInRange( ){
		if (tilesInRange != null) {
			for (int i = 0; i < tilesInRange.Count; i++) {
				tilesInRange [i].EnableHighlight (Color.green);
			}
		}
	}

	/// <summary>
	/// Highlights cells in range.
	/// </summary>
	public void HighlightAttackableTiles(int team ){
		for (int i = 0; i < unitsInRange.Count; i++) {
			if (unitsInRange [i].Unit != null) {
				if (unitsInRange [i].Unit.Team != team) {
					unitsInRange [i].EnableHighlight (Color.red);
					unitsInRange [i].attackable = true;
				}
			}
		}
	}

	//TODO: sameina þetta og næsta fall, eins og er tilesInRange ekki public
	public void ClearTilesInRange () {
		if (tilesInRange != null) {
			for (int i = 0; i < tilesInRange.Count; i++) {
				HexCell current = tilesInRange [i];
				current.SetLabel (null);
				current.DisableHighlight ();
			}

			tilesInRange = null;
		}
			
	}

	//TODO: sameina þetta og næsta fall, eins og er tilesInRange ekki public
	public void ClearAttackableTiles () {
		if (unitsInRange!= null) {
			for (int i = 0; i < unitsInRange.Count; i++) {
				HexCell current = unitsInRange [i];
				current.SetLabel (null);
				current.attackable = false;
				current.DisableHighlight ();
			}

			unitsInRange = null;
		}

	}


	// ekki nota[ i bili mun lagfaera seinna og nota thetta
	public void ClearHiglighted (List<HexCell> highlighted ) {
		if (highlighted != null) {
			for (int i = 0; i < highlighted.Count; i++) {
				HexCell current = highlighted [i];
				current.SetLabel (null);
				current.DisableHighlight ();
				current.DisableHighlight ();
			}

			//tilesInRange = null;
		}

	}
}
