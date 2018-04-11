using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapEditor : MonoBehaviour {

	public Color[] colors;

	public HexGrid hexGrid;

	private Color activeColor;

	private int newIndex;

	// TODO: athuga laga svo þetta sé ekki bara breytt í inspector
	public bool editMode = true;

	HexCell previousCell, searchFromCell, searchToCell;

	void Awake () {
		SelectColor(0);
	}

	void Update () {
		if (Input.GetMouseButton (0) && !EventSystem.current.IsPointerOverGameObject ()) {
			HandleInput ();
		} else {
			previousCell = null;
		}
	}
	// TODO: mögulega þarf að bæta við boolean upp á hvort sé að breyta um lit eða finna distance
	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			HexCell currentCell = hexGrid.GetCell (hit.point);

			if (editMode) {
				EditCell (currentCell);
				//ATH
			} else if(Input.GetKey(KeyCode.LeftShift) && searchToCell != currentCell){
				if (searchFromCell) {
					searchFromCell.DisableHighlight ();
				}
				searchFromCell = currentCell;
				searchFromCell.EnableHighlight (Color.white);
				if(searchToCell){
					hexGrid.FindPath(searchFromCell, searchToCell);
				}
			} else if(searchFromCell && searchFromCell != currentCell){
				searchToCell = currentCell;
				hexGrid.FindPath (searchFromCell, searchToCell);
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
		

	public void SetEditMode () {

		editMode = !editMode;
		print (editMode);

	}

	void EditCell (HexCell cell){
		cell.Color = activeColor;
		cell.moveCost = cell.level [newIndex];
	}
}
