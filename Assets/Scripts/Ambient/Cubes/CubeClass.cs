using System;
using NaughtyAttributes;
using static Cube;

[Serializable]
public class CubeClass
{
	public bool IsLoop => type == CubeType.Loop;

	[BoxGroup("Cube General Type")]
	public CubeType type;
}
