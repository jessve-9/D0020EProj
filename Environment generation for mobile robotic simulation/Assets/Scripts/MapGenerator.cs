using UnityEngine;
using System.Collections;
using System;

public class MapGenerator : MonoBehaviour { 
	public enum MapType {ShortCycle, BigPictureView};
	public MapType mapType;

	int mapWidth = 250;
	int mapLength = 250;

	public float noiseScaleGround;

	public int octavesGround;
	[Range(0,1)]
	public float persistanceGround;
	public float lacunarityGround;

	public int seedGround;
	public Vector2 offsetGround;

    public float heightMultiplierGround;

    public float heightOffsetGround;


    public float noiseScaleHills;

	public int octavesHills;
	[Range(0,1)]
	public float persistanceHills;
	public float lacunarityHills;

	public int seedHills;
	public Vector2 offsetHills;

    public float heightMultiplierHills;

    public float heightOffsetHills;

	public bool autoUpdate;

    public float heightOffsetElevated;

	//public TerrainType[] regions;

	[Range(0,45)]
	public float tiltZ;

	[Range(0,45)]
	public float tiltX;

	public bool useFalloff;
	float[,] falloffMap;

	public void GenerateMap() {

		if(mapLength <= 0){
			mapLength = 1;
		}
		else if(mapWidth <= 0){
			mapWidth = 1;
		}

		int scaledMapLength = Convert.ToInt32(mapLength * 1);		//Fixes scale from unity to pybullet. Now it is 1:1 meters
		int scaledMapWidth = Convert.ToInt32(mapWidth * 1);

		float[,] noiseGround = Noise.GenerateNoiseMap (scaledMapWidth, scaledMapLength, seedGround, noiseScaleGround, octavesGround, persistanceGround, lacunarityGround, offsetGround, heightMultiplierGround, heightOffsetGround);
        float[,] noiseHills = Noise.GenerateNoiseMap (scaledMapWidth, scaledMapLength, seedHills, noiseScaleHills, octavesHills, persistanceHills, lacunarityHills, offsetHills, heightMultiplierHills, heightOffsetHills);

		float[,] elevatedGround = PlaneFunctions.PlaneElevated(noiseGround, heightOffsetElevated);
		noiseHills = PlaneFunctions.AddFalloffMap(noiseHills, falloffMap);
		noiseHills = PlaneFunctions.PlaneTexture(noiseHills, noiseGround);
        float[,] combinedMap = PlaneFunctions.CombineMaxValues(noiseGround, noiseHills);
        combinedMap = PlaneFunctions.CombineMinValues(combinedMap, elevatedGround);
        float maxHeight = float.MinValue;
		combinedMap = PlaneFunctions.CreateTiltZ(combinedMap, tiltZ);
		combinedMap = PlaneFunctions.CreateTiltX(combinedMap, tiltX);

        //Get highets value in combinedMap
		for (int z = 0; z < scaledMapLength; z++) {
			for (int x = 0; x < scaledMapWidth; x++) {
				maxHeight = Mathf.Max(maxHeight, combinedMap[x,z]); 
			}
		}

        Color darkBrown = new Color(0.34f,0.21f,0.08f,1f);
        Color grey = new Color(0.24f,0.24f,0.24f,1f);

        Color[] colourMap = new Color[scaledMapWidth * scaledMapLength];
		for (int z = 0; z < scaledMapLength; z++) {
			for (int x = 0; x < scaledMapWidth; x++) {
				colourMap[z*scaledMapWidth+x] = Color.Lerp(darkBrown, grey, (combinedMap[x,z] - heightOffsetGround)/(maxHeight-heightOffsetGround));
			}
		}

		MapDisplay display = FindObjectOfType<MapDisplay> ();
		display.DrawMesh (CreateMesh.CreateShape (combinedMap), TextureGenerator.TextureFromColourMap (colourMap, scaledMapWidth, scaledMapLength));

	}

	void OnValidate() {
		if (mapWidth < 1) {
			mapWidth = 1;
		}
		if (mapLength < 1) {
			mapLength = 1;
		}
		if (lacunarityGround < 1) {
			lacunarityGround = 1;
		}
		if (octavesGround < 0) {
			octavesGround = 0;
		}
        if (lacunarityHills < 1) {
			lacunarityHills = 1;
		}
		if (octavesHills < 0) {
			octavesHills = 0;
		}

		falloffMap = FalloffGenerator.GenerateFalloffMap(mapWidth, mapLength);
	}

	public void RandomizeOnMapType() {
		if (mapType == MapType.ShortCycle){
			mapWidth = 250;
			mapLength = 250;

			System.Random rnd = new System.Random();
			//Ground
			noiseScaleGround = 0.06f;
			octavesGround = 8;
			persistanceGround = 1f;
			lacunarityGround = 1.03f;
			seedGround = rnd.Next(0, 99999);
			heightMultiplierGround = 0.2f;
			heightOffsetGround = 0f;

			//Elevated ground
			heightOffsetElevated = 12f;

			//Hills
			noiseScaleHills = 120f;
			octavesHills = 4;
			persistanceHills = 0.12f;
			lacunarityHills = 3.2f;
			seedHills = rnd.Next(0, 99999);
			heightMultiplierHills = 40f;
			heightOffsetHills = -23f;
		}
	}
}

/*
[System.Serializable]
public struct TerrainType {
	public string name;
	public float height;
	public Color colour;
}
*/