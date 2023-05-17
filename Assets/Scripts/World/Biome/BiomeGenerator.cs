using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeGenerator : MonoBehaviour
{
	public int waterThreshold = 50;
	public NoiseSettings biomeNoiseSettings;
	public DomainWarping domainWarping;
	public bool WarpingSwitch = true;
	public BlockLayerHandler startLayerHandler;
	public TreeGenerator treeGenerator;
	public List<BlockLayerHandler> additionalLayersHandlers;

	internal TreeData GetTreeData(ChunkData data, Vector2Int mapSeedOffset)
	{
		if (treeGenerator == null)
			return new TreeData();
		return treeGenerator.GenerateTreeData(data, mapSeedOffset);
	}

	public ChunkData ProcessChunkColumn(ChunkData data, int x, int z, Vector2Int mapSeedOffset, int? terrainHeightNoise)
	{
		biomeNoiseSettings.worldOffset = mapSeedOffset;
		int groundPosition;
		if (terrainHeightNoise.HasValue == false)
			groundPosition = GetSurfaceHeightNoise(data.worldPosition.x + x, data.worldPosition.z + z, data.chunkHeight);
		else
			groundPosition = terrainHeightNoise.Value;
		for (int y = data.worldPosition.y; y < data.worldPosition.y + data.chunkHeight; y++)
		{
			startLayerHandler.Handle(data, x, y, z, groundPosition, mapSeedOffset);
		}
		foreach (var layer in additionalLayersHandlers)
		{
			layer.Handle(data, x, data.worldPosition.y, z, groundPosition, mapSeedOffset);
		}
		return data;
	}

	public int GetSurfaceHeightNoise(int x, int z, int chunkHeight)
	{
		float terrainHeight;
		if (WarpingSwitch)
			terrainHeight = domainWarping.GenerateDomainNoise(x, z, biomeNoiseSettings);
		else
			terrainHeight = NoiseGenerator.OctavePerlin(x, z, biomeNoiseSettings);

		terrainHeight = NoiseGenerator.Redistribution(terrainHeight, biomeNoiseSettings);
		int surfaceHeight = NoiseGenerator.RemapValue01ToInt(terrainHeight, 0, chunkHeight);
		return surfaceHeight;
	}
}
