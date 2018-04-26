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

	HexCell previousCell;

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
		return
			hexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
	}
	// TODO: mögulega þarf að bæta við boolean upp á hvort sé að breyta um lit eða finna distance
	void HandleInput () {
		HexCell currentCell = GetCellUnderCursor();
		if (currentCell) {
			//if(editMode){
			EditCell (currentCell); 
			//}
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

	}

	void EditCell (HexCell cell){
		cell.Color = activeColor;
		cell.moveCost = cell.level [newIndex];
	}
}
