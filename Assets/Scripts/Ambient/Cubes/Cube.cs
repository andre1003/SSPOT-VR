using System;
using System.Collections;
using NaughtyAttributes;
using SSpot.Robot;

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
		Loop
	}

	[BoxGroup("Cube General Type")]
	public CubeType type;

	public Cube(CubeType type) {  this.type = type; }

	//Consider using polymorphism
	public IEnumerator ExecuteCoroutine(Robot robot)
	{
		switch (type)
		{
			case CubeType.Begin:
				yield return robot.Animator.SetBrokenCoroutine(false);
				break;
			case CubeType.Left:
				yield return robot.Mover.TurnLeftCoroutine();
				break;
			case CubeType.Right:
				yield return robot.Mover.TurnRightCoroutine();
				break;
			case CubeType.Forward:
				yield return robot.Mover.MoveForwardCoroutine();
				break;
		}
	}
}
