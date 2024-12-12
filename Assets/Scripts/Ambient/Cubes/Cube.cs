using System;
using System.Collections;
using NaughtyAttributes;
using SSpot.Robot;

[Serializable]
public class Cube
{
	public enum CubeType
	{
		Null = 0,
		Begin = 1,
		End = 2,
		Left = 3,
		Right = 4,
		Forward = 5,
		Loop = 6
	}

	[BoxGroup("Cube General Type")]
	public CubeType type;

	public Cube(CubeType type)
	{
		this.type = type;
	}

	//Consider using polymorphism
	public IEnumerator ExecuteCoroutine(RobotData robot)
	{
		switch (type)
		{
			case CubeType.Begin:
				if (robot.Animator.IsBroken)
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
