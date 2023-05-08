using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "NoiseSettings", menuName = "Data/NoiseSettings")]
public class NoiseSettings : ScriptableObject
{
	public float noiseZoom;
	public float persistance;
	public int octaves;
	public Vector2Int offset;
	public Vector2Int worldOffset;
	public float redistributionModifier;
	public float exponent;
}
