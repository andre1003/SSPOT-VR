using System.Collections;
using Photon.Pun;
using SSpot.AnimatorUtilities;
using SSpot.Utilities;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace SSpot.Robot
{
    [RequireComponent(typeof(Animator))]
    public class RobotAnimator : MonoBehaviourPun
    {
        [SerializeField] private float clipTransitionLength = 0.1f;
        
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
        private PlayableGraph _graph;
        private AnimatorControllerPlayable _controller;
        private AnimationMixerPlayable _outputMixer;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            var controller = _animator.runtimeAnimatorController;
            _animator.runtimeAnimatorController = null;
            
            _graph = PlayableGraph.Create(nameof(RobotAnimator));
            var output = AnimationPlayableOutput.Create(_graph, "Output", _animator);
            _outputMixer = AnimationMixerPlayable.Create(_graph, 2);
            output.SetSourcePlayable(_outputMixer);
            
            _controller = AnimatorControllerPlayable.Create(_graph, controller);
            _outputMixer.ConnectInput(0, _controller, 0);
            _outputMixer.SetInputWeight(0, 1);
            
            _graph.Play();
        }

        private void OnDestroy() => _graph.Destroy();

        public void BeginWalking() => _controller.SetBool(walk, true);
        public void StopWalking() => _controller.SetBool(walk, false);

        public void TurnLeft() => _controller.SetTrigger(turnLeft);
        
        public void TurnRight() => _controller.SetTrigger(turnRight);

        private bool _isPlayingClip;
        public IEnumerator PlayClipCoroutine(AnimationClip clip)
        {
            _isPlayingClip = true;
            var clipPlayable = AnimationClipPlayable.Create(_graph, clip);
            _outputMixer.ConnectInput(1, clipPlayable, 0);

            yield return SmoothTransitionCoroutine(0, 1);
            yield return new WaitForSeconds(clip.length - 2 * clipTransitionLength);
            yield return SmoothTransitionCoroutine(1, 0);
            
            _outputMixer.DisconnectInput(1);
            clipPlayable.Destroy();
            _isPlayingClip = false;
        }
        
        public IEnumerator WaitForAnimationCoroutine()
        {
            //If is playing a clip, we can wait until it's over.
            if (_isPlayingClip)
            {
                yield return new WaitWhile(() => _isPlayingClip);
                yield break;
            }
            
            //If not yet in transition, skip a frame so the transition begins 
            if (!_controller.IsInTransition(0))
                yield return null;
            
            //Wait until transition is over and we're back in the idle state
            yield return new WaitUntil(() =>
                _controller.GetCurrentAnimatorStateInfo(0).shortNameHash == idleStateName &&
                !_controller.IsInTransition(0));
        }

        public void EnableRootMotion() => _animator.applyRootMotion = true;
        public void DisableRootMotion() => _animator.applyRootMotion = false;

        private IEnumerator SmoothTransitionCoroutine(int from, int to)
        {
            return CoroutineUtilities.SmoothCoroutine(clipTransitionLength, t =>
            {
                _outputMixer.SetInputWeight(from, 1f - t);
                _outputMixer.SetInputWeight(to, t);
            });
        }
    }
}