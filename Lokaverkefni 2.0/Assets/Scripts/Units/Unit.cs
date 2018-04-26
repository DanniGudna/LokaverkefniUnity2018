using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

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

	public bool IsValidDestination (HexCell cell) {
		// return !cell.IsUnderwater;
		print(cell.moveCost);
		if (cell.moveCost > 19 || cell.Unit || !cell.passable) {
			return false;
		}
		return true;
	}


}
