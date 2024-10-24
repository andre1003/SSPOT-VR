using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using static Cube;

[Serializable]
public class CubeClass
{
	public bool IsLoop => type == CubeType.Loop;

	[BoxGroup("Cube General Type")]
	public CubeType type;

	[BoxGroup("LoopSettings")]
	[ShowIf("IsLoop")]
	[AllowNesting]
    public int loopNumber;

    [BoxGroup("LoopSettings")]
	[ShowIf("IsLoop")]
    [MinMaxSlider(0f, 10f)]
	public Vector2Int loopRangeIndexes = new(0, 1);

	[Serializable]
	public class LoopList { public List<Cube> loopList = new();}

    [BoxGroup("LoopSettings")]
    [ShowIf("IsLoop")]
    [AllowNesting]
    public LoopList loopList = new();

    public List<Cube> GetCommandList()
	{
		List<Cube> cubeList = new();
		if (IsLoop)
		{
			for (int i = 0; i < loopNumber; i++) cubeList.AddRange(loopList.loopList);
		}
		else 
		{
			cubeList.Add(new Cube(type));
		}
		return cubeList;
	}

	[Button]
	public void A()
	{
		Debug.Log(GetCommandList());
	}

	public void AddCube(Cube cube)
	{
		loopList.loopList.Add(cube);
	}
}
