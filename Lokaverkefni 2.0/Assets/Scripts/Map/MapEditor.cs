using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapEditor : MonoBehaviour {

	public Color[] colors;

	public HexGrid hexGrid;

	private Color activeColor;

	private int newIndex;

	public Unit unitPrefab;

	// TODO: athuga laga svo þetta sé ekki bara breytt í inspector
	public bool editMode = true;

	HexCell previousCell, searchFromCell, searchToCell;

	void Awake () {
		SelectColor(0);
	}

	void Update () {

		if (!EventSystem.current.IsPointerOverGameObject()) {
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
	HexCell GetCellUnderCursor () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			return hexGrid.GetCell(hit.point);
		}
		return null;
	}
	// TODO: mögulega þarf að bæta við boolean upp á hvort sé að breyta um lit eða finna distance
	void HandleInput () {
		HexCell currentCell = GetCellUnderCursor();
		if (currentCell) {
			if (editMode) {
				EditCell (currentCell);
				//ATH
			} else if(Input.GetKey(KeyCode.LeftShift) && searchToCell != currentCell){
				if (searchFromCell != currentCell) {	
					if (searchFromCell) {
						searchFromCell.DisableHighlight ();
					}
					searchFromCell = currentCell;
					searchFromCell.EnableHighlight (Color.white);
					if (searchToCell) {
						hexGrid.FindPath (searchFromCell, searchToCell, 24);
					}
				}
			} else if(searchFromCell && searchFromCell != currentCell){
				if (searchToCell != currentCell) {	
					searchToCell = currentCell;
					hexGrid.FindPath (searchFromCell, searchToCell, 24);
				}
			}else {
				// sýnir allar en þarf ða lagfæra ef ég vill nota þetta aftur
				// hexGrid.FindPath (currentCell);
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

	// búa til unit
	void CreateUnit () {
		HexCell cell = GetCellUnderCursor();
		if (cell && !cell.Unit) {
			Unit unit = Instantiate(unitPrefab);
			unit.transform.SetParent(hexGrid.transform, false);
			unit.Location = cell;

		}
	}

	void DestroyUnit () {
		HexCell cell = GetCellUnderCursor();
		if (cell && cell.Unit) {
			cell.Unit.Die();
		}
	}
		

	public void SetEditMode () {

		editMode = !editMode;
		print (editMode);

	}

	void EditCell (HexCell cell){
		cell.Color = activeColor;
		cell.moveCost = cell.level [newIndex];
	}
}
