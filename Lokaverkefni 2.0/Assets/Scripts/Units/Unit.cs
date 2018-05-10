using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

	List<HexCell> pathToTravel;

	public HexGrid hexGrid;

	// hraði hermanns 
	protected int speed;
	//líf hermanns
	protected int health;
	// skaði hermanns
	protected int damage;
	// hversu langt þarf að líða millli hreyfinga
	protected int cooldown;
	// staðan á cooldowni á hermanni, þarf að vera minni en turn númer
	protected int currentCooldown;
	//lengd sem Unit nær að gera damage
	protected int range;
	//er hægt að hreyfa kallinn?
	protected bool movable;
	// type of unit
	protected string type;

	public int Speed {
		get {
			return speed;
		}
	}

	public int Health{
		get {
			return health;
		}

		set{ 
			health = health - value;
		}
	}

	public int Damage {
		 get {
			return damage;
		}
	}

	public int Cooldown {
		get {
			return cooldown;
		}
	}

	public int Range {
		get {
			return range;
		}
	}

	public string Type {
		get {
			return type;
		}
	}

	public int CurrentCooldown {
		get {
			return currentCooldown;
		}

		set { 
			currentCooldown = value + cooldown;
		}
	}

	public Unit(){
		//default fyrir allt er 1
		speed = 1;
		health = 1;
		damage = 1;
		cooldown = 1;
		range = 1;
	}

	public void setSpeed(int newSpeed){
		speed = newSpeed;
	}

	void Awake () {
		//this.gameObject.transform.GetChild(0);
		//this.GetComponent<
	}

	/// <summary>
	/// Gets or sets the location.
	/// </summary>
	/// <value>The location.</value>
	public HexCell Location {
		get {
			return location;
		}
		set {
			if (location) {
				location.Unit = null;
			}
			location = value;
			value.Unit = this;
			transform.localPosition = value.Position;
		}
	}

	HexCell location;

	public void ValidateLocation () {
		transform.localPosition = location.Position;
	}

	// hreinsa objectið þegar það deyr

	public void Die () {
		location.Unit = null;
		Destroy(gameObject);
	}
	/// <summary>
	/// Determines whether this instance is valid destination to the specified cell.
	/// </summary>
	/// <returns><c>true</c> if this instance is valid destination to the specified cell; otherwise, <c>false</c>.</returns>
	/// <param name="cell">Cell.</param>
	public bool IsValidDestination (HexCell cell) {
		// return !cell.IsUnderwater;
		if ( cell.Unit || !cell.passable) {
			return false;
		}
		return true;
	}
	/// <summary>
	/// Travel the specified path.
	/// </summary>
	/// <param name="path">Path.</param>
	public void Travel (List<HexCell> path) {
		int turnNodes = 0;
		for (int i = 0; i < path.Count - 1; i++) {
			if (path [i].turnsToReach == 0) {
				turnNodes++;
			}
		}
		Location = path[turnNodes];
		pathToTravel = path;
		StopAllCoroutines();
		StartCoroutine(TravelPath(turnNodes + 1));
	}
		
	// TODO: breyta
	const float travelSpeed = 4f;

	/// <summary>
	/// Travels the path.
	/// </summary>
	/// <returns>The path.</returns>
	/// <param name="range">Range.</param>
	IEnumerator TravelPath (int range) {

		float t = Time.deltaTime * travelSpeed;;
		//for (int i = 1; i < pathToTravel.Count; i++) {
		for (int i = 1; i < range; i++) {
			Vector3 a = pathToTravel[i - 1].Position;
			Vector3 b = pathToTravel[i].Position;
			for (; t < 1f; t += Time.deltaTime * travelSpeed) {
				transform.localPosition = Vector3.Lerp(a, b, t);
				yield return null;
			}
			t -= 1f;
		}
		//transform.localPosition = location.Position;

		ListPool<HexCell>.Add(pathToTravel);
		pathToTravel = null;
	}

	void OnEnable () {
		if (location) {
			transform.localPosition = location.Position;
		}
	}

	public void moveRange(int sp, HexCell location){
		//List<HexCell> = Range;
		int xStart = location.coordinates.X - sp;
		if (xStart < 0) {
			xStart = 0;
		}
		int xEnd = location.coordinates.X + sp;
		//int yStart = Mathf.Max (-sp, (-location.coordinates.X - sp));
		//int yEnd = Mathf.Min(sp, (-location.coordinates.X + sp));
		for( int i = xStart; i <= xEnd; i++){
			int yStart = Mathf.Max (-sp, (i - sp));
			int yEnd = Mathf.Min(sp, (-i + sp));
			for (int j= yStart; j <= yEnd; j++) {
				int z = -i - j;

				HexCell cellInRange = hexGrid.GetCellFromCoordinates (i, j);

				if (cellInRange != null) {
					
					cellInRange.Color = Color.green;
				}
			}
		}
	} 


	//public void updateCooldown(Unit unit){
	//	unit.currentCooldown += unit.cooldown;
	//}

	public void takeDamage(int damage){
		this.Health = damage;
	}


}
