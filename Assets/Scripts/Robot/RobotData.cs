using Photon.Pun;
using UnityEngine;

namespace SSpot.Robot
{
    public class RobotData : MonoBehaviourPun
    {
        [SerializeField] private RobotGridMover mover;
        public RobotGridMover Mover => mover;

        [SerializeField] private RobotAnimator animator;
        public RobotAnimator Animator => animator;

        [SerializeField] private AudioSource audioSource;
        public AudioSource AudioSource => audioSource;

        private void Awake()
        {
            AssertComponent(ref mover);
            AssertComponent(ref animator);
            AssertComponent(ref audioSource);
        }

        private void AssertComponent<T>(ref T component) where T: Component
        {
            if (!component)
                component = GetComponent<T>();
            
            if (!component)
                Debug.LogError($"Missing component of type {typeof(T).Name} in {nameof(RobotData)}", gameObject);
        }

        public void ResetRobot()
        {
            mover.Reset();
            animator.Reset();
            audioSource.Stop();
        }
    }
}