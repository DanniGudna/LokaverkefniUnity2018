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

	public Unit unitPrefab;

	public Archer testPrefab;

	public Unit[] units;

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
	}

	void Update () {

		if (!EventSystem.current.IsPointerOverGameObject()) {
			// auka check /ar til m'us getur ekki fari[ ut fyrir map
			if (GetCellUnderCursor () != null) {
				if (GetCellUnderCursor ().Unit != null) {
					canvasHoover.enabled = true;
					print (GetCellUnderCursor ().Unit);
					hoover.NewSelectedUnit (GetCellUnderCursor ().Unit);
				} else {
					canvasHoover.enabled = false;
				}
			}
			if (Input.GetMouseButton(0)) {
				HandleInput();
				return;
			}
			if (Input.GetKeyDown(KeyCode.U)) {
				
				CreateUnit();
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
	void CreateUnit () {
		HexCell cell = GetCellUnderCursor();
		if (cell && !cell.Unit) {
			//Unit unit = Instantiate(unitPrefab);
			//Archer unit = Instantiate(testPrefab);
			Unit unit = Instantiate(units[Random.Range(0, units.Length)]);
			unit.transform.SetParent(hexGrid.transform, false);
			unit.Location = cell;

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


}
