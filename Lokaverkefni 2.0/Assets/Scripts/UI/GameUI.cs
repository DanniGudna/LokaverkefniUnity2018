﻿using UnityEngine;
using UnityEngine.EventSystems;

public class GameUI : MonoBehaviour {

	public HexGrid grid;

	HexCell currentCell;
	MapEditor map;
	public TextChanger text;
	private int currentTeamTurn = 0;

	public AudioClip[] selectedVoicelines;
	public AudioClip[] walkingVoicelines;
	public AudioClip[] attackingVoicelines;
	public AudioClip meleeFight;
	public AudioClip meleeFightDeath;
	public AudioClip rangedFight;
	public AudioClip rangedFightDeath;


	Unit selectedUnit;
	Unit selectedUnitSpeed;

	// TODO: sameina
	//bool hasMoved = false;
	//bool hasAttacked = false;
	bool attacking = false;

	protected int turn = 0;

	/**
	 * updates the next turn
	 *  TODO: move to a gamemanager
	 **/
	public void updateTurn(){
		turn++;
		text.UpdateTurnText (turn);
		currentTeamTurn = turn % 2;
		print (currentTeamTurn);
		// kalla a newTurn
	}

	protected void newTurn(){
		// TODO: breyta hvaða lið er að hreyfa sig
		//hasMoved = false;
		//hasAttacked = false;
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
		grid.ClearTilesInRange ();
		grid.ClearAttackableTiles ();


		print (currentTeamTurn);
		if (UpdateCurrentCell()) {
			if (currentCell.Unit != null && currentCell.Unit.Team == currentTeamTurn) {
				selectedUnit = currentCell.Unit;
				text.UpdateUnitInfoPanel (selectedUnit);
				if (selectedUnit.CurrentCooldown < turn + 1) {
					SoundManager.instance.PlayRandomVoiceline (selectedVoicelines);
					if (selectedUnit != null) {
						grid.FindReachableTiles (currentCell, selectedUnit.Speed);
						grid.FindAttackableTiles (currentCell, selectedUnit.Range, selectedUnit.Team);
					}
				} else {
					selectedUnit = null;
					currentCell = null;
				}
			}
		} else{
			//deselectum unitinn
			currentCell = null;
		}
	}

	void DoPathfinding () {
		grid.ClearAttackableTiles ();
		grid.HighlightTilesInRange();
		if (UpdateCurrentCell()) {
			if (currentCell && selectedUnit.IsValidDestination(currentCell)) {
				grid.FindPath (selectedUnit.Location, currentCell, selectedUnit.Speed);
				grid.FindAttackableTiles (grid.CurrentPathTo, selectedUnit.Range, selectedUnit.Team);
				// TODO: finna betri lausn ekki kalla 2 #á þessi föll #í hvert skipti
				grid.HighlightTilesInRange();
				grid.HighlightAttackableTiles ( selectedUnit.Team);
				grid.ShowPath ();


			} else {
				//grid.ClearPath ();
				// TODO: ekki leita aftur geyma upplysingarnar
				//grid.highlightReach();
			}
		}
	}

	/// <summary>
	/// Does the move.
	/// </summary>
	void DoMove () {
		if (grid.HasPath) {
			// selectedUnit.Location = currentCell;
			SoundManager.instance.PlayRandomVoiceline (walkingVoicelines);
			selectedUnit.Travel(grid.GetPath());
			// uppfæra cooldown á kall sem var að hreyfast 
			selectedUnit.CurrentCooldown = turn;

			grid.ClearTilesInRange ();
			grid.ClearPath();
			grid.ClearAttackableTiles ();
			selectedUnit = null;
			text.ClearTextBox ();
		}
	}

	void DoAttackMove (Unit target) {
		SoundManager.instance.PlayRandomVoiceline (attackingVoicelines);
		if (grid.HasPath) {
			// selectedUnit.Location = currentCell;
			//SoundManager.instance.PlayRandomVoiceline (attackingVoicelines);
			selectedUnit.Travel(grid.GetPath());


		}
		// check for safety
		if(target != null){
			//target.takeDamage (selectedUnit.Damage);
			target.Health = selectedUnit.Damage;
		}

		// lets play the appropriate sound
		if(selectedUnit is Archer){
			if(target.Health < 1){
				SoundManager.instance.PlaySingleClip(rangedFightDeath);
			} else {
				SoundManager.instance.PlaySingleClip(rangedFight);
			}
		} else {
			if(target.Health < 1){
				SoundManager.instance.PlaySingleClip(meleeFightDeath);
			} else {
				SoundManager.instance.PlaySingleClip(meleeFight);
			}
		}

		checkForDeath (target);
		selectedUnit.CurrentCooldown = turn + 1;

		grid.ClearTilesInRange ();
		grid.ClearPath();
		grid.ClearAttackableTiles ();
		

		selectedUnit = null;
		text.ClearTextBox ();
	}

	void checkForDeath(Unit unit){
		if (unit.Health < 1) {
			unit.Die ();
		}
	}
		

	void Update () {
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Input.GetMouseButtonDown (0)) {;
				DoSelection ();
			} else if (selectedUnit) {
				if (Input.GetMouseButtonDown (1)) {
					// Ef kall er ekki buinn ad hreyfa sig getur hann ekki gert aras
					// TODO: hvad ef kall vill ekki hrefa sig?
					if (grid.GetCell (Camera.main.ScreenPointToRay (Input.mousePosition)) == grid.CurrentPathTo) {
						DoMove ();
						updateTurn ();
						
					} else if (grid.GetCell (Camera.main.ScreenPointToRay (Input.mousePosition)).attackable) {
						HexCell cellTarget = grid.GetCell (Camera.main.ScreenPointToRay (Input.mousePosition));
						Unit target = cellTarget.Unit;
						DoAttackMove (target);
						updateTurn ();
					} else {
						DoPathfinding ();
						//grid.FindAttackableTiles (grid.CurrentPathTo, selectedUnit.Range);
					}
					
				}
			}
		}
	}
}
