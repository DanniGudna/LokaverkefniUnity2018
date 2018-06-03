using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapEditor : MonoBehaviour {

	public Color[] colors;

	public Material[] material;

	public HexGrid hexGrid;

	private Color activeColor;

	private int newIndex;

	public Unit[] units;

	public Unit[] team0;

	public Unit[] team1;

	public int[] initialBlueUnitCells;

	public int[] initialRedUnitCells;



	//TODO: ekki hafa h'er heldur 'i gameManager
	protected int turn = 1;

	// TODO: athuga laga svo þetta sé ekki bara breytt í inspector
	public bool editMode = true;
	public TextChanger hoover;
	public Canvas canvasHoover;
	HexCell previousCell;

	void Awake () {
		SelectColor(0);
		canvasHoover.enabled = false;
		CreateInitalUnit (initialBlueUnitCells, 0);
		CreateInitalUnit (initialRedUnitCells, 1);
	}

	void Update () {

		if (!EventSystem.current.IsPointerOverGameObject()) {
			// auka check /ar til m'us getur ekki fari[ ut fyrir map
			if (GetCellUnderCursor () != null) {
				if (GetCellUnderCursor ().Unit != null) {
					canvasHoover.enabled = true;
					hoover.UpdateUnitInfoPanel (GetCellUnderCursor ().Unit);
				} else {
					canvasHoover.enabled = false;
				}
			}
			if (Input.GetMouseButton(0)) {
				HandleInput();
				return;
			}
			if (Input.GetKeyDown(KeyCode.U)) {
				
				CreateRandomUnit();
				return;
			}
			if (Input.GetKeyDown(KeyCode.I)) {

				DestroyUnit();
				return;
			}
		}
		previousCell = null;
	}
	//finna það sem er undir músinni
	public HexCell GetCellUnderCursor () {
		return
			hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
	}

	void HandleInput () {
		HexCell currentCell = GetCellUnderCursor();
		if (currentCell) {
			if(editMode){
			EditCell (currentCell); 
			}
			previousCell = currentCell;
		}		else {
			previousCell = null;
		}
	}

	public void SelectColor (int index) {
		activeColor = colors[index];
		newIndex = index + 1;
	}
		
	// TODO: ekki nota ?
	public void ChangeMoveCost (HexCell cell) {
		cell.moveCost = cell.level[cell.index];
	}


	/// <summary>
	/// Creates the unit.
	/// </summary>
	void CreateRandomUnit () {
		HexCell cell = GetCellUnderCursor();
		if (cell && !cell.Unit) {
			// stillir liðið sem kallinn er í, núna er þetta bara radnom fyrir bugtests
			int team = Random.Range (0, 2);

			//Unit unit = Instantiate(units[Random.Range(0, units.Length)]);
			Unit unit = Instantiate(units[(Random.Range(0, 3)) + (team*3)]);

			unit.Team = team;
			if (unit.Team == 1) {
				unit.transform.Rotate (0f, 180f, 0f);
			}
			unit.transform.SetParent(hexGrid.transform, false);
			unit.Location = cell;

		}
	}


	//ath
	void CreateInitalUnit (int[] cells, int team) {
		//if team is red than they are 3 indexes up in the unti array
		int buffer = team * 3;
		//spearmen
		for (int i = 0; i < 5; i++) {
			Unit unit = Instantiate (units [2+buffer]);
			unit.Team = team;
			unit.transform.SetParent(hexGrid.transform, false);
			unit.Location = hexGrid.cells[cells[i]];
			if (unit.Team == 1) {
				unit.transform.Rotate (0f, 180f, 0f);
			}
		}
		//Archer
		for (int i = 5; i < 8; i++) {
			Unit unit = Instantiate (units [0 + buffer]);
			unit.Team = team;
			unit.transform.SetParent(hexGrid.transform, false);
			unit.Location = hexGrid.cells[cells[i]];
			if (unit.Team == 1) {
				unit.transform.Rotate (0f, 180f, 0f);
			}
		}
		//spearmen
		for (int i = 8; i < cells.Length; i++) {
			Unit unit = Instantiate (units [1+ buffer]);
			unit.Team = team;
			unit.transform.SetParent(hexGrid.transform, false);
			unit.Location = hexGrid.cells[cells[i]];
			if (unit.Team == 1) {
				unit.transform.Rotate (0f, 180f, 0f);
			}
		}

	}
		
	/// <summary>
	/// Destroies the unit.
	/// </summary>
	void DestroyUnit () {
		HexCell cell = GetCellUnderCursor();
		if (cell && cell.Unit) {
			cell.Unit.Die();
		}
	}
		
	/// <summary>
	/// Sets the edit mode.
	/// </summary>
	public void SetEditMode () {

		editMode = !editMode;

	}

	/// <summary>
	/// Edits the cell.
	/// </summary>
	/// <param name="cell">Cell.</param>
	void EditCell (HexCell cell){
		cell.Color = activeColor;
		cell.moveCost = cell.level [newIndex];
	}

	public void quit(){
		Application.Quit ();
	}


}
