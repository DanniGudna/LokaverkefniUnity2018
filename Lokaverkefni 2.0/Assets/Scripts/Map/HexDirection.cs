using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HexDirection {
	//TODO: læra'a enum
		NE, E, SE, SW, W, NW
}

public static class HexDirectionExtensions {


	//opposite direction er þrem frá hjá fyrstu þrem og þrem til baka fyrir seinstu þrjá
	public static HexDirection Opposite (this HexDirection direction) {
		return (int)direction < 3 ? (direction + 3) : (direction - 3);
	}
}
