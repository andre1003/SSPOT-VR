using System.Collections;
using NaughtyAttributes;
using Photon.Pun;
using SSpot.AnimatorUtilities;
using UnityEngine;

namespace SSpot.Robot
{
    [RequireComponent(typeof(Animator))]
    public class RobotAnimator : MonoBehaviourPun
    {
        [SerializeField] private float transitionDuration = 0.25f;
        [SerializeField] private bool startBroken;
        
        [BoxGroup("Clips"), SerializeField, RobotAnimatorStateName]
        private HashedString idle, walk, turnLeft, turnRight, broken;


        public Animator Animator { get; private set; }

        private HashedString StartAnimation => startBroken ? broken : idle;
        
        public bool IsBroken => Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == broken && !Animator.IsInTransition(0);
        
        public bool IsIdle => Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == idle && !Animator.IsInTransition(0);

        public IEnumerator WaitForIdle() => new WaitUntil(() => IsIdle);
        
        private void Awake()
        {
            Animator = GetComponent<Animator>();
            Animator.Play(StartAnimation);
        }

        public void SetBroken(bool isBroken) => StartCoroutine(SetBrokenCoroutine(isBroken));
        public IEnumerator SetBrokenCoroutine(bool isBroken) => SmoothTransitionCoroutine(isBroken ? broken : idle);

        public void StartWalking() => StartCoroutine(StartWalkingCoroutine());
        public IEnumerator StartWalkingCoroutine() => SmoothTransitionCoroutine(walk);
        
        public void StopWalking() => StartCoroutine(StopWalkingCoroutine());
        public IEnumerator StopWalkingCoroutine() => SmoothTransitionCoroutine(idle);
        
        
        public void TurnLeft() => StartCoroutine(TurnLeftCoroutine());
        public IEnumerator TurnLeftCoroutine() => PlayOneShotCoroutine(turnLeft);
        
        public void TurnRight() => StartCoroutine(TurnRightCoroutine());
        public IEnumerator TurnRightCoroutine() => PlayOneShotCoroutine(turnRight);
        
        public void PlayOneShot(string targetName) => StartCoroutine(PlayOneShotCoroutine(Animator.StringToHash(targetName)));
        public void PlayOneShot(int targetHash) => StartCoroutine(PlayOneShotCoroutine(targetHash));
        public IEnumerator PlayOneShotCoroutine(int targetHash)
        {
            yield return SmoothTransitionCoroutine(targetHash);
            yield return new WaitForEndOfFrame();
            yield return WaitForAnimationEndCoroutine();
            yield return SmoothTransitionCoroutine(idle);
        }

        private IEnumerator SmoothTransitionCoroutine(int to)
        {
            Animator.CrossFade(to, transitionDuration);
            yield return WaitForTransitionEndCoroutine(to);
        }

        private IEnumerator WaitForTransitionEndCoroutine(int to) =>
            new WaitUntil(() =>
                !Animator.IsInTransition(0) &&
                Animator.GetCurrentAnimatorStateInfo(0).shortNameHash == to);

        private IEnumerator WaitForAnimationEndCoroutine() =>
            new WaitUntil(() => Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

        public void Reset()
        {
            StopAllCoroutines();
            Animator.Play(StartAnimation);
        }
    }
}