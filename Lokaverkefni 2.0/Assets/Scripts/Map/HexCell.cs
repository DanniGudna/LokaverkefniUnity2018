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

	//todo end

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
			UpdateDistanceLabel();
		}
	}
		

	public HexCell GetNeighbor (HexDirection direction) {
		return neighbors[(int)direction];
	}

	void UpdateDistanceLabel () {
		UnityEngine.UI.Text label = uiRect.GetComponent<UnityEngine.UI.Text>();
		label.text = distance == int.MaxValue ? "" :distance.ToString ();
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
