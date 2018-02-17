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

	HexCell[] cells;

	//látum alla hexagons í fylki til að geta notað þá betur
	//búm til alla reitina sem við þurfum
	void Awake () {
		//
		gridCanvas = GetComponentInChildren<Canvas>();
		cells = new HexCell[height * width];

		for (int z = 0, i = 0; z < height; z++) {
			for (int x = 0; x < width; x++) {
				CreateCell(x, z, i++);
			}
		}
	}
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
		//stillum nafnið á nýja objectinu
		cell.name= "y: " + z + " x: " + x;
		//development canvas sem teiknar hnitin á reitina
		drawMarkers(x,z,position);

	}

	//skrifar textann á hvert cell, notað í development
	void drawMarkers (int x, int y, Vector3 position){
		Text label = Instantiate<Text> (coordinatesPrefab);
		label.rectTransform.SetParent(gridCanvas.transform, false);
		label.rectTransform.anchoredPosition =
			new Vector2(position.x, position.z);
		label.text = x.ToString() + "\n" + y.ToString();

	}
		
}
