using JetBrains.Annotations;
using UnityEngine;

namespace SSPot.Scenes
{
    public class ChangeScene : MonoBehaviour
    {
        private enum LoadTarget
        {
            NextLevel = 0,
            MainMenu = 1,
            Tutorial = 2,
            FirstLevel = 3,
            PreviousLevel = 4
        }
        
        [SerializeField]
        private LoadTarget target = LoadTarget.NextLevel;

        /// <summary>
        /// When player clicks on this object, the next level is loaded
        /// </summary>
        [UsedImplicitly]
        public void OnPointerClick()
        {
            switch (target)
            {
                case LoadTarget.MainMenu: 
                    SceneLoader.Instance.LoadMainMenu();
                    return;
                case LoadTarget.Tutorial: 
                    SceneLoader.Instance.LoadTutorial();
                    return;
                case LoadTarget.NextLevel: 
                    SceneLoader.Instance.LoadNextScene();
                    return;
                case LoadTarget.FirstLevel: 
                    SceneLoader.Instance.LoadFirstLevel();
                    return;
                case LoadTarget.PreviousLevel: 
                    SceneLoader.Instance.LoadPreviousScene();
                    return;
                default: return;
            }
        }
    }
}
