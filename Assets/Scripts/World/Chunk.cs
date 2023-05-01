using System;
using UnityEngine;

public static class Chunk
{
	public static MeshData GetChunkMeshData(ChunkData chunkData)
	{
		MeshData meshData = new MeshData(true);

		//fill later
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
		if (inRange(chunkData, localPosition.x) && inRangeHeight(chunkData, localPosition.y)
			&& inRange(chunkData, localPosition.z))
		{
			int index = GetIndexFromPosition(chunkData, localPosition.x, localPosition.y,
			localPosition.z);
			chunkData.blocks[index] = block;
		}
		else
		{
			throw new Exception("Need to ask world for appropriate chunk"); //Change later
		}
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
		throw new Exception("Need to ask World for appropriate chunk"); //change later ?
	}

	private static Vector3Int GetBlockInChunkCoordinates(ChunkData chunkData, Vector3Int pos)
	{
		return new Vector3Int
		{
			x = pos.x - chunkData.worldPosition.x,
			y = pos.y - chunkData.worldPosition.y,
			z = pos.z - chunkData.worldPosition.z,
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
}