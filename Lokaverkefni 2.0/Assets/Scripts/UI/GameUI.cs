using UnityEngine;
using UnityEngine.EventSystems;

public class GameUI : MonoBehaviour {

	public HexGrid grid;

	HexCell currentCell;
	MapEditor map;

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
		grid.ClearReach ();
		grid.ClearAttackable ();




		if (UpdateCurrentCell()) {
			if (currentCell.Unit != null) {
				selectedUnit = currentCell.Unit;
				if (selectedUnit.CurrentCooldown < turn) {
					SoundManager.instance.PlayRandomVoiceline (selectedVoicelines);
					if (selectedUnit != null) {
						grid.FindReachableTiles (currentCell, selectedUnit.Speed);
					}
				} else {
					print ("upps");
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
		grid.ClearAttackable ();
		grid.HighlightReach();
		if (UpdateCurrentCell()) {
			if (currentCell && selectedUnit.IsValidDestination(currentCell)) {
				grid.FindPath (selectedUnit.Location, currentCell, selectedUnit.Speed);
				grid.FindAttackableTiles (grid.CurrentPathTo, selectedUnit.Range);

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

			grid.ClearReach ();
			grid.ClearPath();
			grid.ClearAttackable ();
			selectedUnit = null;
		}
	}

	void DoAttackMove (Unit target) {
		if (grid.HasPath) {
			// selectedUnit.Location = currentCell;
			SoundManager.instance.PlayRandomVoiceline (attackingVoicelines);
			selectedUnit.Travel(grid.GetPath());
			// uppfæra cooldown á kall sem var að hreyfast 
			selectedUnit.CurrentCooldown = turn;

			grid.ClearReach ();
			grid.ClearPath();
			grid.ClearAttackable ();

		}
		// check for safety
		if(target != null){
			target.takeDamage (selectedUnit.Damage);
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
		


		selectedUnit = null;
	}

	void checkForDeath(Unit unit){
		if (unit.Health < 1) {
			unit.Die ();
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
					} else {
				//	DoPathfinding ();
					}
				}
			}
		}
	}
}
