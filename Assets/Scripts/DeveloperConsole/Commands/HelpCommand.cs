using UnityEngine;

namespace SSPot.Scripts.DeveloperConsole.Commands
{
	[CreateAssetMenu(fileName = "HelpCommand", menuName = "DeveloperConsole/Commands/HelpCommand", order = 3)]

	public class HelpCommand : ConsoleCommand
    {
		public override bool Process(string[] args)
		{
			if (args.Length != 0)
			{
				Debug.LogError("This command takes no arguments.");
				return false;
			}

			Debug.Log("[Command] Available Commands:\n" +
				"1. help - Displays this help message.\n" +
				"2. clear - Clears the console logs.\n" +
				"3. changeScene <sceneName> - Changes the current scene to the specified scene name.\n" +
				// Add more commands here as needed
				"Use 'help <command>' for more information on a specific command.");

			return true;
		}
	}
}
