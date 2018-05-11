using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextChanger : MonoBehaviour {

	//Text selectedUnitText;
	Text target;
	//public Text selectedUnitText;
	// index for children sem eg vill eru
	// 1 = unit
	//3
	//5
	//7
	//9
	public static TextChanger instance = null;

	// Use this for initialization
	void Awake () {

	}
	


	public void UpdateUnitInfoPanel(Unit unit){
		// stillum nafnið á kallinum
		target = this.gameObject.transform.GetChild(1).GetComponent<Text> ();
		target.text = unit.Type;
		// Stillum lífið á kallinum
		target = this.gameObject.transform.GetChild(3).GetComponent<Text> ();
		target.text = unit.Health.ToString();
		// Stillum hversu langt það er þar til kallinn má hreyfa sig aftur
		target = this.gameObject.transform.GetChild(5).GetComponent<Text> ();
		// TODOD : stilla útriekningar aðferð frekar
		target.text = unit.CurrentCooldown.ToString();
		// Stillum skaðann sem kallinn gerir
		target = this.gameObject.transform.GetChild(7).GetComponent<Text> ();
		target.text = unit.Damage.ToString();
		// Stillum Rangeið hjá kallinum
		target = this.gameObject.transform.GetChild(9).GetComponent<Text> ();
		target.text = unit.Range.ToString();
		// Stillum hvað cooldownið er eftir hverja hreyfingu
		target = this.gameObject.transform.GetChild(11).GetComponent<Text> ();
		target.text = unit.Cooldown.ToString ();
		// Stillum hvaða lið kallinn er í
		target = this.gameObject.transform.GetChild(13).GetComponent<Text> ();
		target.text = unit.Team.ToString();
	}

	public void ClearTextBox(){
		// stillum nafnið á kallinum
		target = this.gameObject.transform.GetChild(1).GetComponent<Text> ();
		target.text = "";
		// Stillum lífið á kallinum
		target = this.gameObject.transform.GetChild(3).GetComponent<Text> ();
		target.text = "";
		// Stillum hversu langt það er þar til kallinn má hreyfa sig aftur
		target = this.gameObject.transform.GetChild(5).GetComponent<Text> ();
		// TODOD : stilla útriekningar aðferð frekar
		target.text = "";
		// Stillum skaðann sem kallinn gerir
		target = this.gameObject.transform.GetChild(7).GetComponent<Text> ();
		target.text = "";
		// Stillum Rangeið hjá kallinum
		target = this.gameObject.transform.GetChild(9).GetComponent<Text> ();
		target.text = "";
		// Stillum hvað cooldownið er eftir hverja hreyfingu
		target = this.gameObject.transform.GetChild(11).GetComponent<Text> ();
		target.text = "";
		// Stillum hvaða lið kallinn er í
		target = this.gameObject.transform.GetChild(13).GetComponent<Text> ();
		target.text = "";
	}


	public void UpdateTurnText (int turn){
		target = this.gameObject.transform.GetChild(14).GetComponent<Text> ();
		target.text = "Turn: " + turn.ToString ();
	}
		
}
