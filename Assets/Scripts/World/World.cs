using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour 
{
	public int mapSizeInChunks = 6;
	public int chunkSize = 16, chunkHeight = 100;
	public int waterThreshold = 50;
	public float noiseScale = 0.03f;
	public GameObject chunkPrefab;

	Dictionary<Vector3Int, ChunkData> chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>();
	Dictionary<Vector3Int, ChunkRenderer> chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>();

}
