using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SSPot
{
	public class TerminalScript : MonoBehaviour
	{
		public static TerminalScript Instance { get; private set; }

		public GameObject logTextPrefab; // Assign a Text or TextMeshProUGUI prefab
		public Transform contentParent; // Content GameObject inside the Scroll View
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
			string message = $"[{type}] {logString}";

			GameObject logItem = Instantiate(logTextPrefab, contentParent);
			TextMeshProUGUI textMeshProUGUI = logItem.GetComponentInChildren<TextMeshProUGUI>();

			textMeshProUGUI.enableWordWrapping = true;
			textMeshProUGUI.text = message;

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
