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
        [SerializeField] private float transitionDuration = 0.1f;

        [Header("Clips")]
        [SerializeField] private AnimationClip idleClip;
        [SerializeField] private AnimationClip walkClip;
        [SerializeField] private AnimationClip walkBackClip;
        [SerializeField] private AnimationClip turnLeftClip;
        [SerializeField] private AnimationClip turnRightClip;
        [SerializeField] private AnimationClip brokenClip;

        private PlayableGraph _graph;
        private AnimationMixerPlayable _outputMixer;
        private AnimationClipPlayable _oneShotPlayable;

        private const int IdleIndex = 0;
        private const int WalkIndex = 1;
        private const int WalkBackIndex = 2;
        private const int TurnLeftIndex = 3;
        private const int TurnRightIndex = 4;
        private const int BrokenIndex = 5;
        private const int OneShotIndex = 6;
        
        public bool IsBroken => Mathf.Approximately(_outputMixer.GetInputWeight(BrokenIndex), 1f);
        
        public bool IsIdle => Mathf.Approximately(_outputMixer.GetInputWeight(IdleIndex), 1f);

        public IEnumerator WaitForIdle() => new WaitUntil(() => IsIdle);
        
        private void Awake()
        {
            _graph = PlayableGraph.Create(nameof(RobotAnimator));
            var output = AnimationPlayableOutput.Create(_graph, "Output", GetComponent<Animator>());
            _outputMixer = AnimationMixerPlayable.Create(_graph, 7);
            output.SetSourcePlayable(_outputMixer);

            _outputMixer.ConnectInput(IdleIndex, AnimationClipPlayable.Create(_graph, idleClip), 0);
            _outputMixer.ConnectInput(WalkIndex, AnimationClipPlayable.Create(_graph, walkClip), 0);
            _outputMixer.ConnectInput(WalkBackIndex, AnimationClipPlayable.Create(_graph, walkBackClip), 0);
            _outputMixer.ConnectInput(TurnLeftIndex, AnimationClipPlayable.Create(_graph, turnLeftClip), 0);
            _outputMixer.ConnectInput(TurnRightIndex, AnimationClipPlayable.Create(_graph, turnRightClip), 0);
            _outputMixer.ConnectInput(BrokenIndex, AnimationClipPlayable.Create(_graph, brokenClip), 0);
            _outputMixer.SetInputWeight(BrokenIndex, 1f);
            
            _graph.Play();
        }

        public void SetBroken(bool broken) => StartCoroutine(SetBrokenCoroutine(broken));
        public IEnumerator SetBrokenCoroutine(bool broken) =>
            SmoothTransitionCoroutine(
                from: broken ? IdleIndex : BrokenIndex,
                to: broken ? BrokenIndex : IdleIndex
            );

        public void StartWalking(bool backwards) => StartCoroutine(StartWalkingCoroutine(backwards));
        public IEnumerator StartWalkingCoroutine(bool backwards) => SmoothTransitionCoroutine(IdleIndex, backwards ? WalkBackIndex : WalkIndex);
        
        public void StopWalking(bool backwards) => StartCoroutine(StopWalkingCoroutine(backwards));
        public IEnumerator StopWalkingCoroutine(bool backwards) => SmoothTransitionCoroutine(backwards ? WalkBackIndex : WalkIndex, IdleIndex);
        
        
        public void TurnLeft() => StartCoroutine(TurnLeftCoroutine());
        public IEnumerator TurnLeftCoroutine() => PlayOneShotCoroutine(turnLeftClip);
        
        public void TurnRight() => StartCoroutine(TurnRightCoroutine());
        public IEnumerator TurnRightCoroutine() => PlayOneShotCoroutine(turnRightClip);
        
        public void PlayOneShot(AnimationClip clip) => StartCoroutine(PlayOneShotCoroutine(clip));
        public IEnumerator PlayOneShotCoroutine(AnimationClip clip)
        {
            _oneShotPlayable = AnimationClipPlayable.Create(_graph, clip);
            _outputMixer.ConnectInput(OneShotIndex, _oneShotPlayable, 0);

            yield return SmoothTransitionCoroutine(IdleIndex, OneShotIndex);
            yield return new WaitForSeconds(clip.length - 2 * transitionDuration);
            yield return SmoothTransitionCoroutine(OneShotIndex, IdleIndex);
            
            _oneShotPlayable.Destroy();
        }

        private IEnumerator SmoothTransitionCoroutine(int from, int to)
        {
            return CoroutineUtilities.SmoothCoroutine(transitionDuration, t =>
            {
                _outputMixer.SetInputWeight(from, 1f - t);
                _outputMixer.SetInputWeight(to, t);
            });
        }

        public void Reset()
        {
            StopAllCoroutines();
            if (_oneShotPlayable.IsValid())
            {
                _outputMixer.DisconnectInput(OneShotIndex);
                _oneShotPlayable.Destroy();
            }

            for (int i = 0; i < _outputMixer.GetInputCount(); i++)
            {
                float weight = i == BrokenIndex ? 1 : 0;
                _outputMixer.SetInputWeight(i, weight);
            }
        }
    }
}