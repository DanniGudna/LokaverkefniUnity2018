using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

	List<HexCell> pathToTravel;

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

	public void Travel (List<HexCell> path) {
		Location = path[path.Count - 1];
		pathToTravel = path;
		StopAllCoroutines();
		StartCoroutine(TravelPath());
	}
		
	// TODO: breyta
	const float travelSpeed = 4f;

	IEnumerator TravelPath () {

		float t = Time.deltaTime * travelSpeed;;
		for (int i = 1; i < pathToTravel.Count; i++) {
			Vector3 a = pathToTravel[i - 1].Position;
			Vector3 b = pathToTravel[i].Position;
			for (; t < 1f; t += Time.deltaTime * travelSpeed) {
				transform.localPosition = Vector3.Lerp(a, b, t);
				yield return null;
			}
			t -= 1f;
		}
		transform.localPosition = location.Position;

		ListPool<HexCell>.Add(pathToTravel);
		pathToTravel = null;
	}

	void OnEnable () {
		if (location) {
			transform.localPosition = location.Position;
		}
	}


}
