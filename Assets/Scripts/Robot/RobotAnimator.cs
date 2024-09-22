using System.Collections;
using Photon.Pun;
using SSpot.AnimatorUtilities;
using UnityEngine;

namespace SSpot.Robot
{
    [RequireComponent(typeof(Animator))]
    public class RobotAnimator : MonoBehaviourPun
    {
        [AnimatorStateName]
        [SerializeField] private HashedString idleStateName;
        
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
        
        public IEnumerator WaitForAnimationCoroutine()
        {
            //If not yet in transition, skip a frame so the transition begins 
            if (!_animator.IsInTransition(0))
                yield return null;
            
            //Wait until transition is over and we're back in the idle state
            yield return new WaitUntil(() =>
                _animator.GetCurrentAnimatorStateInfo(0).shortNameHash == idleStateName &&
                !_animator.IsInTransition(0));
        }

        public void EnableRootMotion() => _animator.applyRootMotion = true;
        public void DisableRootMotion() => _animator.applyRootMotion = false;
    }
}