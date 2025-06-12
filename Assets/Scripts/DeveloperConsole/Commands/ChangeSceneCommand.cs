using SSPot.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SSPot.Scripts.DeveloperConsole.Commands
{
	[CreateAssetMenu(fileName = "ChangeSceneCommand", menuName = "DeveloperConsole/Commands/ChangeSceneCommand", order = 1)]

	public class ChangeSceneCommand : ConsoleCommand
	{
		public override bool Process(string[] args)
		{
			if (args.Length == 0)
			{
				Debug.LogError("No scene name provided.");
				return false;
			}

			if (!int.TryParse(args[0], out int sceneName))
			{
				Debug.LogError($"Invalid scene name: {args[0]}");
				return false;
			}

			SceneManager.LoadScene(sceneName);
			//SceneLoader.Instance.LoadScene(sceneName);
			Debug.Log($"Changing scene to: {sceneName}");
			return true;
		}
	}
}
