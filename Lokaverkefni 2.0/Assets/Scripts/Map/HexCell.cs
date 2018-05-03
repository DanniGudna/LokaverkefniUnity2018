using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexCell : MonoBehaviour {

	public Coordinates coordinates;

	public RectTransform uiRect;

	int distance;

	// TODO: ekki hafa þetta public bara í develop
	public bool passable = true;

	public int[] level;

	public int moveCost;

	public int index;

	public bool attackable;

	public int turnsToReach;

	public int SearchPhase { get; set; }

	public Unit Unit { get; set; }

	//public Vector3 serial;

	//todo end

	//geymir upplysingar um pathið sem er tekið á milli tveggja cells
	public HexCell PathFrom { get; set;}

	//search hueristic fyrir hvert cell
	public int searchHueristic { get; set;}

	public int SearchPriority {
		get {
			return distance + searchHueristic;
		}
	}

	//notað til að gera keðju af listum
	public HexCell NextWithSamePriority { get; set; }


	public Color Color {
		get {
			return color;
		}
		set {
			if (color == value) {
				return;
			}
			color = value;
			Refresh();
		}
	}

	Color color;

	public HexGridChunk chunk;

	[SerializeField]
	HexCell[] neighbors;

	public int Distance {
		get {
			return distance;
		}
		set {
			distance = value;
			//UpdateDistanceLabel();
		}
	}
		

	public HexCell GetNeighbor (HexDirection direction) {
		return neighbors[(int)direction];
	}

	/*void UpdateDistanceLabel () {
		UnityEngine.UI.Text label = uiRect.GetComponent<UnityEngine.UI.Text>();
		label.text = distance == int.MaxValue ? "" :distance.ToString ();
	}  */

	public void SetLabel (string text) {
		UnityEngine.UI.Text label = uiRect.GetComponent<Text>();
		label.text = text;
	}

	public Vector3 Position {
		get {
			return transform.localPosition;
		}
	}


	//ef cell B er nágranni cell A þá er cell A líka nágranna cell B getum still báða í einu
	public void SetNeighbor (HexDirection direction, HexCell cell) {
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}

	void Refresh () {
		if (chunk) {
			chunk.Refresh ();
		}
	}

	// TODO: sameina
	public void DisableHighlight () {
		Image highlight = uiRect.GetChild (0).GetComponent<Image> ();
		highlight.enabled = false;
	}

	public void EnableHighlight (Color color) {
		Image highlight = uiRect.GetChild (0).GetComponent<Image> ();
		highlight.color = color;
		highlight.enabled = true;
	}

}
