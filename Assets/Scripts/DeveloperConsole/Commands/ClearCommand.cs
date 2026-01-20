using UnityEngine;


namespace SSPot.Scripts.DeveloperConsole.Commands
{
	[CreateAssetMenu(fileName = "ClearCommand", menuName = "DeveloperConsole/Commands/ClearCommand", order = 2)]

	public class ClearCommand : ConsoleCommand
	{
		public override bool Process(string[] args)
		{
			if (args.Length != 0)
			{
				Debug.LogError("This command takes no arguments.");
				return false;
			}

			TerminalScript.Instance.ClearLogs();
			return true;
		}
	}
}
