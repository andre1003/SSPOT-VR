using SSPot.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SSPot.Menu
{
    public class GlobalPlatformController : Singleton<GlobalPlatformController>
    {
        // Is platform a PC?
        public bool isOnPc = true;

        [SerializeField] private float playerSetupDelay = .25f; 


        protected override void Awake()
        {
            base.Awake();
            if (Instance != this) return;
            
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Wait 0.25 seconds to setup platform. This is needed, because player spawn have a delay
            StartCoroutine(CoroutineUtilities.WaitThen(playerSetupDelay, SetupPlatform));
        }

        /// <summary>
        /// Setup game platform for all players.
        /// </summary>
        private void SetupPlatform()
        {
            // Get all players GameObject
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            // Loop players, setting the proper game platform
            foreach(GameObject player in players)
            {
                player.GetComponent<GamePlatformController>().SetGamePlatform(isOnPc);
            }
        }
    }
}
