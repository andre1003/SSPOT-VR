using System;
using NaughtyAttributes;
[Serializable]
public class Cube
{
	public enum CubeType
	{
		Null,
		Begin,
		End,
		Left,
		Right,
		Forward,
		Back, 
		Loop
	}

	[BoxGroup("Cube General Type")]
	public CubeType type;

	public Cube(CubeType type) {  this.type = type; }
}
