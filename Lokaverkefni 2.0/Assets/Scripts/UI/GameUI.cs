using UnityEngine;
using UnityEngine.EventSystems;

public class GameUI : MonoBehaviour {

	public HexGrid grid;

	HexCell currentCell;

	Unit selectedUnit;
	Unit selectedUnitSpeed;

	public void SetEditMode (bool toggle) {
		enabled = !toggle;
		grid.ShowUI(!toggle);
		grid.ClearPath();
		// editMode = !editMode;
	}

	bool UpdateCurrentCell () {
		HexCell cell =
			grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
		if (cell != currentCell) {
			currentCell = cell;
			return true;
		}
		return false;
	}

	void DoSelection () {
		
		grid.ClearPath();
		UpdateCurrentCell();
		if (currentCell) {
			selectedUnit = currentCell.Unit;
			//print ("upps");
			//selectedUnit.moveRange (selectedUnit.Speed, currentCell);
		}
	}

	void DoPathfinding () {
		if (UpdateCurrentCell()) {
			if (currentCell && selectedUnit.IsValidDestination(currentCell)) {
				grid.FindPath (selectedUnit.Location, currentCell, selectedUnit.Speed);
				//print ("this " + selectedUnit.Speed);
			} else {
				grid.ClearPath ();
			}
		}
	}

	void DoMove () {
		if (grid.HasPath) {
			// selectedUnit.Location = currentCell;
			selectedUnit.Travel(grid.GetPath());
			grid.ClearPath();
		}
	}

	void DoTurnMove (){
		if (grid.HasPath) {
			//currentPath 
		
		}
	}

	void Update () {
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButtonDown (0)) {
				DoSelection ();
			} else if (selectedUnit) {
				if (Input.GetMouseButtonDown (1)) {
					DoMove ();
				} else {
					DoPathfinding ();
				}
			}
		}
	}
}
