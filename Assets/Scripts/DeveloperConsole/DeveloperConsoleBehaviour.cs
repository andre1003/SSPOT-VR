using SSPot.Scripts.DeveloperConsole.Commands;
using TMPro;
using UnityEngine;

namespace SSPot.Scripts.DeveloperConsole
{
	public class DeveloperConsoleBehaviour : MonoBehaviour
	{
		[SerializeField] private string prefix = string.Empty;
		[SerializeField] private ConsoleCommand[] commands = new ConsoleCommand[0];

		[Header("UI")]
		[SerializeField] private GameObject uiCanvas = null;
		[SerializeField] private TMP_InputField inputField = null;

		private float pausedTimeScale;

		private static DeveloperConsoleBehaviour instance;
		private DeveloperConsole developerConsole;
		private DeveloperConsole DeveloperConsole
		{
			get
			{
				if (developerConsole != null)
				{
					return developerConsole;
				}
				return developerConsole = new DeveloperConsole(prefix, commands);
			}
		}

		private void Awake()
		{
			if (instance != null && instance != this)
			{
				Destroy(gameObject);
				return;
			}

			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		public void Update()
		{
			if (Input.GetKeyDown(KeyCode.BackQuote))
			{
				Toggle();
			}

			if (uiCanvas.activeSelf && Input.GetKeyDown(KeyCode.Return))
			{
				inputField.ActivateInputField();
				//Debug.Log(inputField.text);
				ProcessCommand(inputField.text);
			}

			if (uiCanvas.activeSelf && Input.GetKeyDown(KeyCode.Slash))
			{
				inputField.ActivateInputField();
			}
		}

		public void Toggle()
		{
			inputField.text = string.Empty;

			if (uiCanvas.activeSelf)
			{
				uiCanvas.SetActive(false);
				Time.timeScale = pausedTimeScale;
			}
			else
			{
				uiCanvas.SetActive(true);
				pausedTimeScale = Time.timeScale;
				Time.timeScale = 0f;
				inputField.ActivateInputField();
			}
		}

		public void ProcessCommand(string inputValue)
		{
			DeveloperConsole.ProcessCommand(inputValue);
			inputField.text = string.Empty;
		}
	}
}
