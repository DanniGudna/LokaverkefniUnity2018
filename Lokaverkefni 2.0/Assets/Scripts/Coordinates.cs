﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct Coordinates {
	public int X { get; private set; }

	public int Z { get; private set; }

	//x,y,z hnitakerfi í hexagon er alltaf = 0 þannig y = -x -z
	public int Y { get{ return -X - Z;
		} }

	//ath röð
	public Coordinates (int x, int z) {
		X = x;
		Z = z;
	}

	//TODO: hvernig er kallað á þetta?
	public static Coordinates offsetCoordinates (int x, int z) {
		return new Coordinates (x -z /2, z);
	
	}

	//TODO: athuga þetta
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
