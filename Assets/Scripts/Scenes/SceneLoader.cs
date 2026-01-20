using System.Collections;
using NaughtyAttributes;
using Photon.Pun;
using SSPot.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SSPot.Scenes
{
    public class SceneLoader : NetworkedSingleton<SceneLoader>
    {
        [SerializeField, BoxGroup("Canvas")]
        private GameObject loadSceneCanvas;
        [SerializeField, BoxGroup("Canvas")]
        private Slider progressBar;
        
        [SerializeField, BoxGroup("Scenes")]
        private SerializableScene mainMenu, lobby, tutorial, firstLevel;

        public Scene CurrentScene => SceneManager.GetActiveScene();
        
        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;
            
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.AutomaticallySyncScene = true;
            }
            
            loadSceneCanvas.SetActive(false);
        }

        public void LoadMainMenu() => LoadScene(mainMenu.BuildIndex);

        public void LoadLobby() => LoadScene(lobby.BuildIndex);
        
        public void LoadTutorial() => LoadScene(tutorial.BuildIndex);

        public void LoadFirstLevel() => LoadScene(firstLevel.BuildIndex);

        public void LoadPreviousScene()
        {
            int prevIndex = CurrentScene.buildIndex - 1;
            if (prevIndex < 0)
            {
                Debug.Log("No more scenes to load.");
                return;
            }
            
            LoadScene(prevIndex);
        }
        
        public void LoadNextScene()
        {
            int nextIndex = CurrentScene.buildIndex + 1;
            if (nextIndex >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.Log("No more scenes to load.");
                return;
            }
            
            LoadScene(CurrentScene.buildIndex + 1);
        }

        private Coroutine _loadCoroutine;
        public void LoadScene(int buildIndex)
        {
            if (PhotonNetwork.InRoom)
            {
                photonView.RPC(nameof(LoadSceneRpc), RpcTarget.AllBuffered, buildIndex);
            }
            else
            {
                LoadSceneRpc(buildIndex);
            }
        }

        [PunRPC]
        private void LoadSceneRpc(int buildIndex)
        {
            if (_loadCoroutine != null)
            {
                Debug.LogError($"Tried to load scene {buildIndex} while already loading");
                return;
            }
            
            _loadCoroutine = StartCoroutine(LoadSceneCoroutine(buildIndex));
        }


        private IEnumerator LoadSceneCoroutine(int buildIndex)
        {
            loadSceneCanvas.SetActive(true);
            
            // Make sure the loading screen is visible
            yield return new WaitForEndOfFrame();
            yield return null;
            
            // Only load the scene if we are the master client
            if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(buildIndex);
            }
            
            // Wait for the scene to load
            while (PhotonNetwork.LevelLoadingProgress < 1)
            {
                progressBar.value = PhotonNetwork.LevelLoadingProgress;
                yield return null;
            }
            loadSceneCanvas.SetActive(false);
            
            _loadCoroutine = null;

            OnSceneLoaded();
		}

        private void OnSceneLoaded()
        {
            SpeakAtStart.instance.StartSpeaking();
            Debug.LogWarning($"Scene {CurrentScene.name} loaded.");
		}
    }
}