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

        [SerializeField, BoxGroup("Message")] 
        private GameObject nextLevelMessage, connectionMessage;
        
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

        public void OpenLoadingScene(bool showConnectionMessage = false)
        {
            connectionMessage.SetActive(showConnectionMessage);
            nextLevelMessage.SetActive(!showConnectionMessage);
            
            loadSceneCanvas.SetActive(true);
        }
        
        public void CloseLoadingScene() => loadSceneCanvas.SetActive(false);
        

        public void LoadMainMenu() => LoadScene(mainMenu.BuildIndex);

        public void LoadLobby() => LoadScene(lobby.BuildIndex);
        
        public void LoadTutorial() => LoadScene(tutorial.BuildIndex);

        public void LoadFirstLevel() => LoadScene(firstLevel.BuildIndex + 2);

        public void LoadPreviousScene()
        {
            int index = CurrentScene.buildIndex;
            int prevIndex = index - 1;
            if (prevIndex < 0)
            {
                Debug.Log("No more scenes to load.");
                return;
            }
            
            LoadScene(prevIndex);
        }
        
        public void LoadNextScene()
        {
            int index = CurrentScene.buildIndex;
            int nextIndex = index + 1;
            if (nextIndex >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.Log("No more scenes to load.");
                return;
            }
            
            LoadScene(CurrentScene.buildIndex + 1);
        }

        private Coroutine _loadCoroutine;
        private void LoadScene(int buildIndex)
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
            bool isToLobby = buildIndex == lobby.BuildIndex;
            OpenLoadingScene(showConnectionMessage: isToLobby);
            yield return new WaitForEndOfFrame();
            yield return null;
            
            if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel(buildIndex);
            }
            
            while (PhotonNetwork.LevelLoadingProgress < 1)
            {
                progressBar.value = PhotonNetwork.LevelLoadingProgress;
                yield return null;
            }
            loadSceneCanvas.SetActive(false);
            
            _loadCoroutine = null;
        }

        private void SetMessageForIndex(int buildIndex)
        {
            
        }
    }
}