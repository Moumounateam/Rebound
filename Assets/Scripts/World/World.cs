using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

	public UnityEvent OnWorldCreated;
	public UnityEvent OnNewChunksGenerated;

	public WorldData worldData { get; private set; }
	public bool IsWorldCreated { get; private set; }
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

	public async void GenerateWorld()
	{
		await GenerateWorld(Vector3Int.zero);
	}

	private async Task GenerateWorld(Vector3Int position)
	{
		WorldGenerationData worldGenerationData = await Task.Run(() => GetPositionsThatPlayerSees(position));

		foreach (Vector3Int pos in worldGenerationData.chunkPositionsToRemove)
		{
			WorldDataHelper.RemoveChunk(this, pos);
		}

		foreach (Vector3Int pos in worldGenerationData.chunkDataToRemove)
		{
			WorldDataHelper.RemoveChunkData(this, pos);
		}

		ConcurrentDictionary<Vector3Int, ChunkData> dataDictionary = null;
		//try
		//{
		dataDictionary = await CalculateWorldChunkData(worldGenerationData.chunkDataPositionsToCreate);
		//}
		//catch (Exception)
		//{
		//	Debug.Log("Task canceled");
		//	return;
		//}

		foreach (var calculateData in dataDictionary)
		{
			worldData.chunkDataDictionary.Add(calculateData.Key, calculateData.Value);
		}

		ConcurrentDictionary<Vector3Int, MeshData> meshDataDictionary = new ConcurrentDictionary<Vector3Int, MeshData>();
		
		List<ChunkData> dataToRender = worldData.chunkDataDictionary
			.Where(keyvaluepair => worldGenerationData.chunkPositionsToCreate.Contains(keyvaluepair.Key))
			.Select(keyvalpair => keyvalpair.Value)
			.ToList();
		meshDataDictionary = await CreateMeshDataAsync(dataToRender);

		StartCoroutine(ChunkCreationCoroutine(meshDataDictionary));
	}

	private Task<ConcurrentDictionary<Vector3Int, MeshData>> CreateMeshDataAsync(List<ChunkData> dataToRender)
	{
		ConcurrentDictionary<Vector3Int, MeshData> dictionary = new ConcurrentDictionary<Vector3Int, MeshData>();
		return Task.Run(() =>
		{

			foreach (ChunkData data in dataToRender)
			{
				//if (taskTokenSource.Token.IsCancellationRequested)
				//{
				//	taskTokenSource.Token.ThrowIfCancellationRequested();
				//}
				MeshData meshData = Chunk.GetChunkMeshData(data);
				dictionary.TryAdd(data.worldPosition, meshData);
			}

			return dictionary;
		}//taskTokenSource.Token
		);
	}

	private Task<ConcurrentDictionary<Vector3Int, ChunkData>> CalculateWorldChunkData(List<Vector3Int> chunkDataPositionsToCreate)
	{
		ConcurrentDictionary<Vector3Int, ChunkData> dictionary = new ConcurrentDictionary<Vector3Int, ChunkData>();

		return Task.Run(() =>
		{
			foreach (Vector3Int pos in chunkDataPositionsToCreate)
			{
				//if (taskTokenSource.Token.IsCancellationRequested)
				//{
				//	taskTokenSource.Token.ThrowIfCancellationRequested();
				//}
				ChunkData data = new ChunkData(chunkSize, chunkHeight, this, pos);
				ChunkData newData = terrainGenerator.GenerateChunkData(data, mapSeedOffset);
				dictionary.TryAdd(pos, newData);
			}
			return dictionary;
		}
		//taskTokenSource.Token
		);
	}

	IEnumerator ChunkCreationCoroutine(ConcurrentDictionary<Vector3Int, MeshData> meshDataDictionary)
	{
		foreach (var item in meshDataDictionary)
		{
			CreateChunk(worldData, item.Key, item.Value); //key: pos, value: data
			yield return new WaitForEndOfFrame();
		}
		if (IsWorldCreated == false)
		{
			IsWorldCreated = true;
			OnWorldCreated?.Invoke();
		}
	}

	private void CreateChunk(WorldData worldData, Vector3Int position, MeshData meshData)
	{
		GameObject chunkObject = Instantiate(chunkPrefab, position, Quaternion.identity);
		ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
		worldData.chunkDictionary.Add(position, chunkRenderer);
		chunkRenderer.initChunk(worldData.chunkDataDictionary[position]);
		chunkRenderer.UpdateChunk(meshData);
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

	internal async void LoadAdditionalChunksRequest(GameObject player)
	{
		Debug.Log("Load more chunks");
		OnNewChunksGenerated?.Invoke();
		await GenerateWorld(Vector3Int.RoundToInt(player.transform.position));
		OnNewChunksGenerated?.Invoke();
	}

	private WorldGenerationData GetPositionsThatPlayerSees(Vector3Int playerPosition)
	{
		List<Vector3Int> allChunkPositionsNeeded = WorldDataHelper.GetChunkPositionsAroundPlayer(this, playerPosition);
		List<Vector3Int> allChunkDataPositionsNeeded = WorldDataHelper.GetDataPositionsAroundPlayer(this, playerPosition);

		List<Vector3Int> chunkPositionsToCreate = WorldDataHelper.SelectPositonsToCreate(worldData, allChunkPositionsNeeded, playerPosition);
		List<Vector3Int> chunkDataPositionsToCreate = WorldDataHelper.SelectDataPositonsToCreate(worldData, allChunkDataPositionsNeeded, playerPosition);

		List<Vector3Int> chunkPositionsToRemove = WorldDataHelper.GetUnneededChunks(worldData, allChunkPositionsNeeded);
		List<Vector3Int> chunkDataToRemove = WorldDataHelper.GetUnneededData(worldData, allChunkDataPositionsNeeded);

		WorldGenerationData data = new WorldGenerationData
		{
			chunkPositionsToCreate = chunkPositionsToCreate,
			chunkDataPositionsToCreate = chunkDataPositionsToCreate,
			chunkPositionsToRemove = chunkPositionsToRemove,
			chunkDataToRemove = chunkDataToRemove,
		};
		return data;
	}

	internal void RemoveChunk(ChunkRenderer chunk)
	{
		chunk.gameObject.SetActive(false);
	}

	internal bool SetBlock(RaycastHit hit, BlockType blockType)
	{
		ChunkRenderer chunk = hit.collider.GetComponent<ChunkRenderer>();
		if (chunk == null)
			return false;

		Vector3Int pos = GetBlockPos(hit);

		WorldDataHelper.SetBlock(chunk.ChunkData.worldReference, pos, blockType);
		chunk.ModifiedByThePlayer = true;

		if (Chunk.IsOnEdge(chunk.ChunkData, pos)) //Update neighbour chunk if block destroyed is on edge
		{
			List<ChunkData> neighbourDataList = Chunk.GetEdgeNeighbourChunk(chunk.ChunkData, pos);
			foreach (ChunkData neighbourData in neighbourDataList)
			{
				ChunkRenderer chunkToUpdate = WorldDataHelper.GetChunk(neighbourData.worldReference, neighbourData.worldPosition);
				if (chunkToUpdate != null)
					chunkToUpdate.UpdateChunk();
			}
		}

		chunk.UpdateChunk();
		return true;
	}

	private Vector3Int GetBlockPos(RaycastHit hit)
	{
		Vector3 pos = new Vector3(
			 GetBlockPositionIn(hit.point.x, hit.normal.x),
			 GetBlockPositionIn(hit.point.y, hit.normal.y),
			 GetBlockPositionIn(hit.point.z, hit.normal.z)
			 );

		return Vector3Int.RoundToInt(pos);
	}

	private float GetBlockPositionIn(float pos, float normal)
	{
		if (Mathf.Abs(pos % 1) == 0.5f)
		{
			pos -= (normal / 2);
		}

		return (float)pos;
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
