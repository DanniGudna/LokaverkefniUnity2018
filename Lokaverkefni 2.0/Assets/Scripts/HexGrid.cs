using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour {

	//breidd borðsins
	public int width;
	//hæð borðsins
	public int height;

	//Debug text UI ofan á reitina, vitum þannig hnitin á þeim
	public Text coordinatesPrefab;
	Canvas gridCanvas;

	public HexCell cellPrefab;
	HexMesh hexMesh;

	HexCell[] cells;

	public Color defaultColor = Color.white;
	public Color touchedColor = Color.magenta;

	//látum alla hexagons í fylki til að geta notað þá betur
	//búm til alla reitina sem við þurfum
	void Awake () {
		//náum í canvas objectið og látum inn gildi í hann
		gridCanvas = GetComponentInChildren<Canvas>();
		//náum í meshið og notum það til að teikna reitina
		hexMesh = GetComponentInChildren<HexMesh> ();
		cells = new HexCell[height * width];

		for (int z = 0, i = 0; z < height; z++) {
			for (int x = 0; x < width; x++) {
				CreateCell(x, z, i++);
			}
		}
	}

	void Start () {
		hexMesh.Triangulate(cells);
	}


	//verður fært mouse handling
	void Update () {

	}
		

	public void ColorCell (Vector3 position, Color color) {
		position = transform.InverseTransformPoint(position);
		Coordinates coordinates = Coordinates.FromPosition(position);
		Debug.Log("touched at " + coordinates.ToString());
		int index = coordinates.X + coordinates.Z * width + coordinates.Z / 2;
		HexCell cell = cells[index];
		cell.color = color;
		//cell.color = touchedColor;
		hexMesh.Triangulate(cells);
	}

	//end



	/**
	 * 
	 * x, y og z eru hnit hvers cell fyrir sig
	 *  
	**/
	void CreateCell (int x, int z, int i) {
		Vector3 position;
		//notum hexagon gildin úr hexmetrics til að finna offsetin
		position.x = (x + z * 0.5f - z/2) * (HexMetrics.innerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.outerRadius * 1.5f);

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = Coordinates.offsetCoordinates(x, z);
		cell.color = defaultColor;

		//stillum nágranna
		//þetta stillir að reiturinn til vinstri ( vestur W) sé nágranni, viljum ekki númer 0
		//fallið okkar í hex direction mun still svon nágranna til austurs með þessu
		if (x > 0) {
			cell.SetNeighbor (HexDirection.W, cells [i - 1]);
		}
		//stillum NE-SW
		if (z > 0) {
			//TODO: breyta í %
			if ((z & 1) == 0) {
				cell.SetNeighbor (HexDirection.SE, cells [i - width]);
				if (x > 0) {
					cell.SetNeighbor (HexDirection.SW, cells [i - width - 1]);
				}
			} else {
				cell.SetNeighbor (HexDirection.SW, cells [i - width]);
				if (x < width - 1) {
					cell.SetNeighbor (HexDirection.SE, cells [i - width + 1]);
				}
			}
		}


		int y = -x - z;
		//stillum nafnið á nýja objectinu
		cell.name= "x: " + x + " y: " + y +" z: " + z;

		//development canvas sem teiknar hnitin á reitina
		drawMarkers(x,z,position, cell);



	}
		

	//skrifar textann á hvert cell, notað í development
	void drawMarkers (int x, int y, Vector3 position, HexCell cell){
		Text label = Instantiate<Text> (coordinatesPrefab);
		label.rectTransform.SetParent(gridCanvas.transform, false);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		//TODO: breyta , ekki taka inn cell og nota gamla mögulega
		label.text = cell.coordinates.ToStringOnSeparateLines();
		//label.text = x.ToString() + "\n" + y.ToString();

	}
		
}
