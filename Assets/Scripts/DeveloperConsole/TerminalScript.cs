using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SSPot
{
	enum TerminalLogType
	{
		Command,
		Error,
		Assert,
		Warning,
		Log,
		Exception
	}

	public class TerminalScript : MonoBehaviour
	{
		public static TerminalScript Instance { get; private set; }

		public GameObject logTextPrefab; 
		public Transform contentParent; 
		public ScrollRect scrollRect;
		public int maxLogs = 100;

		private Queue<GameObject> logEntries = new Queue<GameObject>();

		void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else
			{
				Destroy(gameObject);
			}
		}

		void OnEnable()
		{
			Application.logMessageReceived += HandleLog;
		}

		void OnDisable()
		{
			Application.logMessageReceived -= HandleLog;
		}

		void HandleLog(string logString, string stackTrace, LogType type)
		{
			TerminalLogType terminalLogType;
			

			switch (type)
			{
				case LogType.Error:
					terminalLogType = TerminalLogType.Error;
					break;
				case LogType.Assert:
					terminalLogType = TerminalLogType.Assert;
					break;
				case LogType.Warning:
					terminalLogType = TerminalLogType.Warning;
					break;
				case LogType.Exception:
					terminalLogType = TerminalLogType.Exception;
					logString += $"\n{stackTrace}";
					break;
				default:
					terminalLogType = TerminalLogType.Log;
					break;
			}

			if (type == LogType.Log)
			{
				string[] words = logString.Split(' ');
				if (words.Length > 0 && words[0].StartsWith("/"))
				{
					terminalLogType = TerminalLogType.Command;
				}
				else if(words.Length > 0 && words[0] == "[Command]")
				{
					terminalLogType = TerminalLogType.Command;
					logString = logString.Replace("[Command]", "").Trim();
				}
				else
				{
					terminalLogType = TerminalLogType.Log;
				}
			}

			string message = $"[{terminalLogType}] {logString}";

			GameObject logItem = Instantiate(logTextPrefab, contentParent);
			TextMeshProUGUI textMeshProUGUI = logItem.GetComponentInChildren<TextMeshProUGUI>();

			textMeshProUGUI.enableWordWrapping = true;
			textMeshProUGUI.text = message;

			switch (terminalLogType)
			{
				case TerminalLogType.Command:
					textMeshProUGUI.color = Color.blue;
					break;
				case TerminalLogType.Error:
					textMeshProUGUI.color = Color.red;
					break;
				case TerminalLogType.Assert:
					textMeshProUGUI.color = Color.yellow;
					break;
				case TerminalLogType.Warning:
					textMeshProUGUI.color = Color.yellow;
					break;
				case TerminalLogType.Exception:
					textMeshProUGUI.color = Color.magenta;
					break;
				default:
					textMeshProUGUI.color = Color.white;
					break;
			}

			logEntries.Enqueue(logItem);
			if (logEntries.Count > maxLogs)
			{
				Destroy(logEntries.Dequeue());
			}

			Canvas.ForceUpdateCanvases();
			scrollRect.verticalNormalizedPosition = 0f;
		}

		public void ClearLogs()
		{
			foreach (GameObject logEntry in logEntries)
			{
				Destroy(logEntry);
			}
			logEntries.Clear();
			Canvas.ForceUpdateCanvases();
			scrollRect.verticalNormalizedPosition = 0f;
		}
	}
}
