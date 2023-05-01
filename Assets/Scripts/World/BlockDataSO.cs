using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[CreateAssetMenu(fileName = "BlockData", menuName = "Data/Block Data")]
public class BlockDataSO : ScriptableObject
{
	public float textureSizeX;
	public float textureSizeY;
	public List<TextureData> textureDataList;
}

[Serializable]
public class TextureData
{
	public BlockType blockType;
	public Vector2Int up;
	public Vector2Int down;
	public Vector2Int side;
	public bool isSolid = true;
	public bool generatesCollider = true;
}