﻿using UnityEngine;
using UnityEngine.EventSystems;

public class GameUI : MonoBehaviour {

	public HexGrid grid;

	HexCell currentCell;
	MapEditor map;

	Unit selectedUnit;
	Unit selectedUnitSpeed;

	// TODO: sameina
	bool hasMoved = false;
	bool hasAttacked = false;
	bool attacking = false;

	protected int turn = 1;

	/**
	 * updates the next turn
	 *  TODO: move to a gamemanager
	 **/
	public void updateTurn(){
		turn++;
		print (turn);
		// kalla a newTurn
	}

	protected void newTurn(){
		// TODO: breyta hvaða lið er að hreyfa sig
		hasMoved = false;
		hasAttacked = false;
	}

	public void SetEditMode (bool toggle) {
		enabled = !toggle;
		grid.ShowUI(!toggle);
		grid.ClearPath();
		// editMode = !editMode;
	}

	/// <summary>
	/// Updates the current cell.
	/// </summary>
	/// <returns><c>true</c>, if current cell was updated, <c>false</c> otherwise.</returns>
	bool UpdateCurrentCell () {
		HexCell cell =
			grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
		if (cell != currentCell) {
			currentCell = cell;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Does the selection.
	/// </summary>
	void DoSelection () {
		
		grid.ClearPath();
		grid.ClearReach ();
		UpdateCurrentCell();
		if (currentCell) {
			selectedUnit = currentCell.Unit;
			if (selectedUnit) {
				grid.FindReachableTiles (currentCell, selectedUnit.Speed);
			}
			//if(selectedUnit.Cooldown 
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
				//grid.ClearPath ();
			}
		}
	}

	/// <summary>
	/// Does the move.
	/// </summary>
	void DoMove () {
		if (grid.HasPath) {
			// selectedUnit.Location = currentCell;
			selectedUnit.Travel(grid.GetPath());
			// uppfæra cooldown á kall sem var að hreyfast 
			selectedUnit.updateCooldown (selectedUnit);  
			grid.ClearReach ();
			grid.ClearPath();
			selectedUnit = null;
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
					// Ef kall er ekki buinn ad hreyfa sig getur hann ekki gert aras
					// TODO: hvad ef kall vill ekki hrefa sig?
					if (!attacking) {
						if (!hasMoved) {
							DoPathfinding ();
							hasMoved = true;
						// ef aftur er ytt a sama reit tha hreyfa kallinn
						} else if (grid.GetCell (Camera.main.ScreenPointToRay (Input.mousePosition)) == grid.CurrentPathTo) {
							DoMove ();
							// TODO: finna hvaða kallar eru in range
						}
					} else {
				//	DoPathfinding ();
					}
				}
			}
		}
	}
}
