using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

	public HexCell Location {
		get {
			return location;
		}
		set {
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


}
