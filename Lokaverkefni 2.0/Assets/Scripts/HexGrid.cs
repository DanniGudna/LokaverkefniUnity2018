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
		if (Input.GetMouseButton(0)) {
			HandleInput();
		}
	}

	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			TouchCell(hit.point);
		}
	}

	void TouchCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		Coordinates coordinates = Coordinates.FromPosition(position);
		Debug.Log("touched at " + coordinates.ToString());
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
		//stillum nafnið á nýja objectinu
		cell.name= "y: " + z + " x: " + x;
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
