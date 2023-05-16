using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeLayerHandler : BlockLayerHandler
{
	public float terrainHeightLimit = 25;
	protected override bool TryHandling(ChunkData chunkData, int x, int y, int z, int surfaceHeightNoise, Vector2Int mapSeedOffset)
	{
		if (chunkData.worldPosition.y < 0)
			return false;
		if (surfaceHeightNoise < terrainHeightLimit
			&& chunkData.treeData.treePositions.Contains(new Vector2Int(chunkData.worldPosition.x + x, chunkData.worldPosition.z + z)))
		{
			Vector3Int chunkCoordinates = new Vector3Int(x, surfaceHeightNoise, z);
			BlockType type = Chunk.GetBlockFromChunkCoordinates(chunkData, chunkCoordinates);
			if (type == BlockType.Grass_Dirt)
			{
				Chunk.setBlock(chunkData, chunkCoordinates, BlockType.Dirt);
				for (int i = 1; i < 5; i++)
				{
					chunkCoordinates.y = surfaceHeightNoise + i;
					Chunk.setBlock(chunkData, chunkCoordinates, BlockType.TreeTrunk);
				}
				//foreach (Vector3Int leafPosition in treeLeafesStaticLayout)
				//{
				//	chunkData.treeData.treeLeavesSolid.Add(new Vector3Int(x + leafPosition.x, surfaceHeightNoise + 5 + leafPosition.y, z + leafPosition.z));
				//}
			}
		}
		return false;
	}
}