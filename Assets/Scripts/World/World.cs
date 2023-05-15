using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class World : MonoBehaviour 
{
	public int mapSizeInChunks = 6;
	public int chunkSize = 16;
	public int chunkHeight = 100;
	public GameObject chunkPrefab;
	public int chunkDrawingRange = 8;

	public TerrainGenerator terrainGenerator;
	public Vector2Int mapSeedOffset;
	//Dictionary<Vector3Int, ChunkData> chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>();
	//Dictionary<Vector3Int, ChunkRenderer> chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>();

	public UnityEvent OnWorldCreated;
	public UnityEvent OnNewChunksGenerated;

	public WorldData worldData { get; private set; }
	private void Awake()
	{
		worldData = new WorldData
		{
			chunkHeight = this.chunkHeight,
			chunkSize = this.chunkSize,
			chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>(),
			chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>()
		};
	}

	public void GenerateWorld()
	{
		GenerateWorld(Vector3Int.zero);
	}

	private void GenerateWorld(Vector3Int position)
	{
		WorldGenerationData worldGenerationData = GetPositionsThatPlayerSees(position);

		//foreach (Vector3Int pos in worldGenerationData.chunkPositionsToRemove)
		//{
		//	WorldDataHelper.RemoveChunk(this, pos);
		//}

		//foreach (Vector3Int pos in worldGenerationData.chunkDataToRemove)
		//{
		//	WorldDataHelper.RemoveChunkData(this, pos);
		//}

		foreach (var pos in worldGenerationData.chunkDataPositionsToCreate)
		{
			ChunkData data = new ChunkData(chunkSize, chunkHeight, this, pos);
			ChunkData newData = terrainGenerator.GenerateChunkData(data, mapSeedOffset);
			worldData.chunkDataDictionary.Add(pos, newData);
		}

		foreach (var pos in worldGenerationData.chunkPositionsToCreate)
		{
			ChunkData data = worldData.chunkDataDictionary[pos];
			MeshData meshData = Chunk.GetChunkMeshData(data);
			GameObject chunkObject = Instantiate(chunkPrefab, data.worldPosition, Quaternion.identity);
			ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
			worldData.chunkDictionary.Add(data.worldPosition, chunkRenderer);
			chunkRenderer.initChunk(data);
			chunkRenderer.UpdateChunk(meshData);

		}
		OnWorldCreated?.Invoke();
	}



	internal BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
	{
		Vector3Int pos = Chunk.ChunkPositionFromBlockCoords(this, x, y, z);
		ChunkData containerChunk = null;

		worldData.chunkDataDictionary.TryGetValue(pos, out containerChunk);

		if (containerChunk == null)
			return BlockType.Nothing;
		Vector3Int blockInCHunkCoordinates = Chunk.GetBlockInChunkCoordinates(containerChunk, new Vector3Int(x, y, z));
		return Chunk.GetBlockFromChunkCoordinates(containerChunk, blockInCHunkCoordinates);
	}

	internal void LoadAdditionalChunksRequest(GameObject player)
	{
		Debug.Log("Load more chunks");
		OnNewChunksGenerated?.Invoke();
		GenerateWorld(Vector3Int.RoundToInt(player.transform.position));
		OnNewChunksGenerated?.Invoke();
	}
	private WorldGenerationData GetPositionsThatPlayerSees(Vector3Int playerPosition)
	{
		List<Vector3Int> allChunkPositionsNeeded = WorldDataHelper.GetChunkPositionsAroundPlayer(this, playerPosition);
		List<Vector3Int> allChunkDataPositionsNeeded = WorldDataHelper.GetDataPositionsAroundPlayer(this, playerPosition);

		List<Vector3Int> chunkPositionsToCreate = WorldDataHelper.SelectPositonsToCreate(worldData, allChunkPositionsNeeded, playerPosition);
		List<Vector3Int> chunkDataPositionsToCreate = WorldDataHelper.SelectDataPositonsToCreate(worldData, allChunkDataPositionsNeeded, playerPosition);

		//List<Vector3Int> chunkPositionsToRemove = WorldDataHelper.GetUnnededChunks(worldData, allChunkPositionsNeeded);
		//List<Vector3Int> chunkDataToRemove = WorldDataHelper.GetUnnededData(worldData, allChunkDataPositionsNeeded);

		WorldGenerationData data = new WorldGenerationData
		{
			chunkPositionsToCreate = chunkPositionsToCreate,
			chunkDataPositionsToCreate = chunkDataPositionsToCreate,
			chunkPositionsToRemove = new List<Vector3Int>(),
			chunkDataToRemove = new List<Vector3Int>(),
		};
		return data;
	}
	
	public struct WorldGenerationData
	{
		public List<Vector3Int> chunkPositionsToCreate;
		public List<Vector3Int> chunkDataPositionsToCreate;
		public List<Vector3Int> chunkPositionsToRemove;
		public List<Vector3Int> chunkDataToRemove;
	}
	public struct WorldData
	{
		public Dictionary<Vector3Int, ChunkData> chunkDataDictionary;
		public Dictionary<Vector3Int, ChunkRenderer> chunkDictionary;
		public int chunkSize;
		public int chunkHeight;
	}
}
