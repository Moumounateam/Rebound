using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
	public int waterThreshold = 50;
	public NoiseSettings biomeNoiseSettings;

	public ChunkData ProcessChunkColumn(ChunkData data, int x, int z, Vector2Int mapSeedOffset)
	{
		biomeNoiseSettings.worldOffset = mapSeedOffset;
		int groundPosition = GetSurfaceHeightNoise(data.worldPosition.x + x, data.worldPosition.z + z, data.chunkHeight);

		for (int y = 0; y < data.chunkHeight; y++)
		{
			BlockType voxelType = BlockType.Dirt;
			if (y > groundPosition)
			{ 
				if (y < waterThreshold)
				{
					voxelType = BlockType.Water;
				}
				else
				{
					voxelType = BlockType.Air;
				}
			}
			else if (y == groundPosition && y < waterThreshold)
			{
				voxelType = BlockType.Sand;
			}
			else if (y == groundPosition)
			{
				voxelType = BlockType.Grass_Dirt;
			}
			Chunk.setBlock(data, new Vector3Int(x, y, z), voxelType);
		}
		return data;
	}

	private int GetSurfaceHeightNoise(int x, int z, int chunkHeight)
	{
		//if(useDomainWarping == false)
		//{
		//	terrainHeight = MyNoise.OctavePerlin(x, z, biomeNoiseSettings);
		//}
		//else
		//{
		//	terrainHeight = domainWarping.GenerateDomainNoise(x, z, biomeNoiseSettings);
		//}

		float terrainHeight = NoiseGenerator.OctavePerlin(x, z, biomeNoiseSettings);
		terrainHeight = NoiseGenerator.Redistribution(terrainHeight, biomeNoiseSettings);
		int surfaceHeight = NoiseGenerator.RemapValue01ToInt(terrainHeight, 0, chunkHeight);
		return surfaceHeight;
	}
}
