using System;
using System.Collections;
using NaughtyAttributes;
using Photon.Pun;
using SSpot.Grids;
using SSpot.Utilities;
using UnityEngine;

namespace SSpot.Robot
{
    [RequireComponent(typeof(RobotAnimator))]
    public class RobotGridMover : MonoBehaviourPun, ILevelGridObject
    {
        [SerializeField] private bool useClipDuration; 
        
        [HideIf(nameof(useClipDuration))]
        [SerializeField] private float walkTime = 1;
        
        [HideIf(nameof(useClipDuration))]
        [SerializeField] private float turnTime = 1;
        
        public LevelGrid Grid { get; set; }
        
        public Vector2Int GridPosition { get; set; }
        
        private Vector2Int _facing;
        public Vector2Int Facing
        {
            get => _facing;
            set
            {
                value.x = Mathf.Clamp(value.x, -1, 1);
                value.y = Mathf.Clamp(value.y, -1, 1);

                if (value is {x: 0, y: 0} or {x: 1, y: 1} or {x: -1, y: -1}) 
                {
                    Debug.LogWarning($"Invalid facing {value} for robot. Correcting to {Vector2Int.right}.");
                    value = Vector2Int.right;
                }

                _facing = value;
                transform.forward = new(_facing.x, 0, _facing.y);
            }
        }
        
        bool ILevelGridObject.CanWalkThrough => false;

        bool ILevelGridObject.TriggerOnSteppedOn => true;

        private RobotAnimator _animator;

        private Vector2Int _originalFacing;
        private Vector2Int _originalGridPosition;

        public void ChangeNode(Vector2Int target) => Grid.ChangeNode(this, target);
        
        private void Awake()
        {
            Facing = new((int)transform.forward.x, (int)transform.forward.z);
            _animator = GetComponent<RobotAnimator>();
        }

        private void Start()
        {
            _originalFacing = Facing;
            _originalGridPosition = GridPosition;
        }

        #region MOVE
        
        public IEnumerator MoveForwardCoroutine() => MoveCoroutine();

        private IEnumerator MoveCoroutine()
        {
            var fromCell = GridPosition;
            var toCell = fromCell + Facing;

            if (!Grid.InGrid(toCell))
            {
                Debug.Log($"Can't walk out of grid, from {fromCell} to {toCell}");
                yield break;
            }

            if (!Grid[toCell].CanWalk)
            {
                Debug.Log($"Can't walk from {fromCell} to {toCell}");
                yield break;
            }

            Vector3 from = Grid.GetCellCenterWorld(fromCell);
            Vector3 to = Grid.GetCellCenterWorld(toCell);
            
            float duration = useClipDuration ? _animator.WalkClip.length : walkTime;
            
            _animator.StartWalking();
            yield return CoroutineUtilities.SmoothCoroutine(duration, t => transform.position = Vector3.Lerp(from, to, t));
            _animator.StopWalking();
            yield return _animator.WaitForIdle();
            
            ChangeNode(toCell);
        }
        
        #endregion
        
        #region TURN

        public IEnumerator TurnLeftCoroutine() => TurnCoroutine(left: true);
        public IEnumerator TurnRightCoroutine() => TurnCoroutine(left: false);
        
        private IEnumerator TurnCoroutine(bool left)
        {
            if (left) _animator.TurnLeft();
            else _animator.TurnRight();
            
            Vector3 originalForward = transform.forward;
            Vector2Int target = left ? RotateFacingLeft(Facing) : RotateFacingRight(Facing);
            Vector3 targetForward = new(target.x, 0, target.y);

            float duration = useClipDuration
                ? (left ? _animator.TurnLeftClip.length : _animator.TurnRightClip.length)
                : turnTime;
            
            yield return CoroutineUtilities.SmoothCoroutine(duration,
                t => transform.forward = Vector3.Slerp(originalForward, targetForward, t));
            yield return _animator.WaitForIdle();
            
            Facing = target;
        }
        
        #endregion

        public void Reset()
        {
            Facing = _originalFacing;
            Grid.ChangeNode(this, _originalGridPosition);
        }

        private static Vector2Int RotateFacingLeft(Vector2Int facing)
        {
            if (facing == Vector2Int.right) return Vector2Int.up;
            if (facing == Vector2Int.up) return Vector2Int.left;
            if (facing == Vector2Int.left) return Vector2Int.down;
            if (facing == Vector2Int.down) return Vector2Int.right;
            throw new ArgumentException();
        }
        
        private static Vector2Int RotateFacingRight(Vector2Int facing)
        {
            if (facing == Vector2Int.right) return Vector2Int.down;
            if (facing == Vector2Int.down) return Vector2Int.left;
            if (facing == Vector2Int.left) return Vector2Int.up;
            if (facing == Vector2Int.up) return Vector2Int.right;
            throw new ArgumentException();
        }
    }
}