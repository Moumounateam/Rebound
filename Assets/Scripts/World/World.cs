using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class World : MonoBehaviour 
{
	public int mapSizeInChunks = 6;
	public int chunkSize = 16, chunkHeight = 100;
	public GameObject chunkPrefab;

	public TerrainGenerator terrainGenerator;
	public Vector2Int mapSeedOffset;
	Dictionary<Vector3Int, ChunkData> chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>();
	Dictionary<Vector3Int, ChunkRenderer> chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>();

	public UnityEvent OnWorldCreated;
	public UnityEvent OnNewChunksGenerated;

	public void GenerateWorld()
	{
		chunkDataDictionary.Clear();
		foreach (ChunkRenderer chunk in chunkDictionary.Values)
		{
			Destroy(chunk.gameObject);
		}
		chunkDictionary.Clear();

		for (int x = 0; x < mapSizeInChunks; x++)
		{
			for (int z = 0; z < mapSizeInChunks; z++)
			{

				ChunkData data = new ChunkData(chunkSize, chunkHeight, this, new Vector3Int(x * chunkSize, 0, z * chunkSize));
				//GenerateVoxels(data);
				ChunkData newData = terrainGenerator.GenerateChunkData(data, mapSeedOffset);
				chunkDataDictionary.Add(newData.worldPosition, newData);
			}
		}

		foreach (ChunkData data in chunkDataDictionary.Values)
		{
			MeshData meshData = Chunk.GetChunkMeshData(data);
			GameObject chunkObject = Instantiate(chunkPrefab, data.worldPosition, Quaternion.identity);
			ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
			chunkDictionary.Add(data.worldPosition, chunkRenderer);
			chunkRenderer.initChunk(data);
			chunkRenderer.UpdateChunk(meshData);
		}
		OnWorldCreated?.Invoke();
	}

	internal BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
	{
		Vector3Int pos = Chunk.ChunkPositionFromBlockCoords(this, x, y, z);
		ChunkData containerChunk = null;

		chunkDataDictionary.TryGetValue(pos, out containerChunk);

		if (containerChunk == null)
			return BlockType.Nothing;
		Vector3Int blockInCHunkCoordinates = Chunk.GetBlockInChunkCoordinates(containerChunk, new Vector3Int(x, y, z));
		return Chunk.GetBlockFromChunkCoordinates(containerChunk, blockInCHunkCoordinates);
	}
	internal void LoadAdditionalChunksRequest(GameObject player)
	{
		Debug.Log("Load more chunks");
		OnNewChunksGenerated?.Invoke();
		//GenerateWorld(Vector3Int.RoundToInt(player.transform.position));
		//OnNewChunksGenerated?.Invoke();
	}
}
