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
		
	/// <summary>
	/// Gets the neighbor at a given direction.
	/// </summary>
	/// <returns>The neighbor.</returns>
	/// <param name="direction">Direction.</param>
	public HexCell GetNeighbor (HexDirection direction) {
		return neighbors[(int)direction];
	}

	/// <summary>
	/// Sets the label.
	/// </summary>
	/// <param name="text">Text.</param>
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
	/// <summary>
	/// Sets cells as neighbours if one cell is in a given direction to the other.
	/// </summary>
	/// <param name="direction">Direction.</param>
	/// <param name="cell">Cell.</param>
	public void SetNeighbor (HexDirection direction, HexCell cell) {
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}


	/// <summary>
	/// Refresh this instance.
	/// </summary>
	void Refresh () {
		if (chunk) {
			chunk.Refresh ();
		}
	}

	/// <summary>
	/// Disables the highlight.
	/// </summary>
	public void DisableHighlight () {
		Image highlight = uiRect.GetChild (0).GetComponent<Image> ();
		highlight.enabled = false;
	}

	/// <summary>
	/// Enables the highlight.
	/// </summary>
	/// <param name="color">Color.</param>
	public void EnableHighlight (Color color) {
		Image highlight = uiRect.GetChild (0).GetComponent<Image> ();
		highlight.color = color;
		highlight.enabled = true;
	}

}
