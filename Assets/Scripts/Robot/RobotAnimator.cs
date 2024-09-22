using Photon.Pun;
using UnityEngine;

namespace SSpot.Robot
{
    [RequireComponent(typeof(Animator))]
    public class RobotAnimator : MonoBehaviourPun
    {
        [Header("Parameter Names")]
        
        [AnimatorParamName]
        [SerializeField] private HashedString walk;
        
        [AnimatorParamName]
        [SerializeField] private HashedString turnLeft;
        
        [AnimatorParamName]
        [SerializeField] private HashedString turnRight;
        
        private Animator _animator;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void BeginWalking() => _animator.SetBool(walk, true);
        public void StopWalking() => _animator.SetBool(walk, false);

        public void TurnLeft() => _animator.SetTrigger(turnLeft);
        
        public void TurnRight() => _animator.SetTrigger(turnRight);
    }
}