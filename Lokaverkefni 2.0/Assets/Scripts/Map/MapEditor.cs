using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapEditor : MonoBehaviour {

	public Color[] colors;

	public HexGrid hexGrid;

	private Color activeColor;

	// TODO: athuga laga svo þetta sé ekki bara breytt í inspector
	public bool editMode = false;

	HexCell previousCell;

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
			} else {
				hexGrid.FindDistancesTo (currentCell);
			}


			previousCell = currentCell;
		}		else {
			previousCell = null;
		}
	}

	public void SelectColor (int index) {
		activeColor = colors[index];
	}

	void EditCell (HexCell cell){
		cell.color = activeColor;
		hexGrid.Refresh ();
	}
}
