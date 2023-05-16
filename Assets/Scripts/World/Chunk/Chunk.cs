using System;
using System.Collections.Generic;
using UnityEngine;
public static class Chunk
{
	public static MeshData GetChunkMeshData(ChunkData chunkData)
	{
		MeshData meshData = new MeshData(true);

		LoopThroughTheBlocks(chunkData, (x, y, z) => meshData = BlockHelper.GetMeshData(chunkData, x, y, z, meshData, chunkData.blocks[GetIndexFromPosition(chunkData, x, y, z)]));
		return meshData;
	}

	private static Vector3Int GetPositionFromIndex(ChunkData chunkData, int index)
	{
		int x = index % chunkData.chunkSize;
		int y = (index / chunkData.chunkSize) % chunkData.chunkHeight;
		int z = index / (chunkData.chunkSize * chunkData.chunkHeight);
		return new Vector3Int(x, y, z);
	}

	//in chunk coordinate system
	private static bool inRange(ChunkData chunkData, int axisCoordinate)
	{
		if (axisCoordinate < 0 || axisCoordinate >= chunkData.chunkSize)
			return false;
		return true;
	}

	//in chunk coordinate system
	private static bool inRangeHeight(ChunkData chunkData, int yCoordinate)
	{
		if (yCoordinate < 0 || yCoordinate >= chunkData.chunkHeight)
			return false;
		return true;
	}

	public static void setBlock(ChunkData chunkData, Vector3Int localPosition, BlockType block)
	{
		if (inRange(chunkData, localPosition.x) && inRangeHeight(chunkData, localPosition.y) && inRange(chunkData, localPosition.z))
		{
			int index = GetIndexFromPosition(chunkData, localPosition.x, localPosition.y, localPosition.z);
			chunkData.blocks[index] = block;
		}
		else
			WorldDataHelper.SetBlock(chunkData.worldReference, localPosition + chunkData.worldPosition, block);
	}

	public static BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, Vector3Int chunkCoordinates)
	{
		return GetBlockFromChunkCoordinates(chunkData, chunkCoordinates.x, chunkCoordinates.y, chunkCoordinates.z);
	}

	public static BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
	{
		if (inRange(chunkData, x) && inRangeHeight(chunkData, y) && inRange(chunkData, z))
		{
			int index = GetIndexFromPosition(chunkData, x, y, z);
			return chunkData.blocks[index];
		}
		return chunkData.worldReference.GetBlockFromChunkCoordinates(chunkData, chunkData.worldPosition.x + x, chunkData.worldPosition.y + y, chunkData.worldPosition.z + z);
	}

	public static Vector3Int GetBlockInChunkCoordinates(ChunkData chunkData, Vector3Int pos)
	{
		return new Vector3Int
		{
			x = pos.x - chunkData.worldPosition.x,
			y = pos.y - chunkData.worldPosition.y,
			z = pos.z - chunkData.worldPosition.z
		};
	}

	private static int GetIndexFromPosition(ChunkData chunkData, int x, int y, int z)
	{
		return (x + chunkData.chunkSize * y + chunkData.chunkSize * chunkData.chunkHeight * z);
	}

	public static void LoopThroughTheBlocks(
		ChunkData chunkData,
		Action<int, int, int> actionToPerform)
	{
		for (int index = 0; index < chunkData.blocks.Length; index++)
		{
			var position = GetPositionFromIndex(chunkData, index);
			actionToPerform(position.x, position.y, position.z);
		}
	}
	internal static Vector3Int ChunkPositionFromBlockCoords(World world, int x, int y, int z)
	{
		Vector3Int pos = new Vector3Int
		{
			x = Mathf.FloorToInt(x / (float)world.chunkSize) * world.chunkSize,
			y = Mathf.FloorToInt(y / (float)world.chunkHeight) * world.chunkHeight,
			z = Mathf.FloorToInt(z / (float)world.chunkSize) * world.chunkSize
		};
		return pos;
	}
	internal static bool IsOnEdge(ChunkData chunkData, Vector3Int worldPosition)
	{
		Vector3Int chunkPosition = GetBlockInChunkCoordinates(chunkData, worldPosition);
		if (
			chunkPosition.x == 0 || chunkPosition.x == chunkData.chunkSize - 1 ||
			chunkPosition.y == 0 || chunkPosition.y == chunkData.chunkHeight - 1 ||
			chunkPosition.z == 0 || chunkPosition.z == chunkData.chunkSize - 1
			)
			return true;
		return false;
	}

	internal static List<ChunkData> GetEdgeNeighbourChunk(ChunkData chunkData, Vector3Int worldPosition)
	{
		Vector3Int chunkPosition = GetBlockInChunkCoordinates(chunkData, worldPosition);
		List<ChunkData> neighboursToUpdate = new List<ChunkData>();
		if (chunkPosition.x == 0)
		{
			neighboursToUpdate.Add(WorldDataHelper.GetChunkData(chunkData.worldReference, worldPosition - Vector3Int.right));
		}
		else if (chunkPosition.x == chunkData.chunkSize - 1)
		{
			neighboursToUpdate.Add(WorldDataHelper.GetChunkData(chunkData.worldReference, worldPosition + Vector3Int.right));
		}
		if (chunkPosition.y == 0)
		{
			neighboursToUpdate.Add(WorldDataHelper.GetChunkData(chunkData.worldReference, worldPosition - Vector3Int.up));
		}
		else if (chunkPosition.y == chunkData.chunkHeight - 1)
		{
			neighboursToUpdate.Add(WorldDataHelper.GetChunkData(chunkData.worldReference, worldPosition + Vector3Int.up));
		}
		if (chunkPosition.z == 0)
		{
			neighboursToUpdate.Add(WorldDataHelper.GetChunkData(chunkData.worldReference, worldPosition - Vector3Int.forward));
		}
		else if (chunkPosition.z == chunkData.chunkSize - 1)
		{
			neighboursToUpdate.Add(WorldDataHelper.GetChunkData(chunkData.worldReference, worldPosition + Vector3Int.forward));
		}
		return neighboursToUpdate;
	}
}