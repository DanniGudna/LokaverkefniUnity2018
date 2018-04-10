using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexCell : MonoBehaviour {

	public Coordinates coordinates;

	 public RectTransform uiRect;

	int distance;

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

}
