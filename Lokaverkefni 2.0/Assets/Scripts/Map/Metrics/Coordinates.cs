﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Coordinates {

	[SerializeField]
	private int x, z;

	public int X {
		get {
			return x;
		}
	}

	public int Z {
		get {
			return z;
		}
	}
		
	//x,y,z hnitakerfi í hexagon er alltaf = 0 þannig y = -x -z
	public int Y { get{ return -X - Z;
		} }

	//ath röð
	public Coordinates (int x, int z) {
		this.x = x;
		this.z = z;
	}

	public int DistanceTo (Coordinates other) {
		return
			((x < other.x ? other.x - x : x - other.x) +
				(Y < other.Y ? other.Y - Y : Y - other.Y) +
				(z < other.z ? other.z - z : z - other.z)) / 2;
	}


	public static Coordinates offsetCoordinates (int x, int z) {
		return new Coordinates (x -z /2, z);
	
	}
		
	public override string ToString () {
		return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
	}

	public string ToStringOnSeparateLines () {
		return X.ToString() + "\n" + Y.ToString() +"\n" + Z.ToString();
	}

	//hvaða reit erum við að snerta
	public static Coordinates FromPosition (Vector3 position) {
		float x = position.x / (HexMetrics.innerRadius * 2f);
		float y = -x;
		//þurfum að gera offset ef Z er ekki = 0
		float offset = position.z / (HexMetrics.outerRadius * 3f);
		x -= offset;
		y -= offset;
		//námundum í int til að fá hnitin
		int iX = Mathf.RoundToInt(x);
		int iY = Mathf.RoundToInt(y);
		int iZ = Mathf.RoundToInt(-x -y);

		//leiðréttum ef við erum út á enda
		if (iX + iY + iZ != 0) {
			float dX = Mathf.Abs(x - iX);
			float dY = Mathf.Abs(y - iY);
			float dZ = Mathf.Abs(-x -y - iZ);

			if (dX > dY && dX > dZ) {
				iX = -iY - iZ;
			}
			else if (dZ > dY) {
				iZ = -iX - iY;
			}
		}

		return new Coordinates(iX, iZ);
	}





}
 
