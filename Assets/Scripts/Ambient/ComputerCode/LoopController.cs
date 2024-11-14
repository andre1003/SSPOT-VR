using System;
using NaughtyAttributes;
using Photon.Pun;
using SSPot.Utilities;
using UnityEngine;
using UnityEngine.UI;

// TODO: comentar a classe e revisar algumas fun��es
namespace SSpot.Ambient.ComputerCode
{
    public class LoopController : MonoBehaviourPun
    {
        private const int MinRange = 1;
        private const int MinIterations = 2;
        
        [Serializable]
        public class LoopSettings
        {
            [AllowNesting, MinValue(MinIterations)]
            public int maxIterations = 10;
            
            [AllowNesting, MinValue(MinRange)]
            public int maxRange = 1;
        }
        
        public CodingCell ParentCell { get; set; }

        [field: BoxGroup("Current Values"), SerializeField, ReadOnly]
        private int iterations = 2, range = 1;
        
        [BoxGroup("Loop Settings"), SerializeField]
        private bool overrideGlobalSettings;
        [BoxGroup("Loop Settings"), SerializeField, ShowIf(nameof(overrideGlobalSettings))]
        private LoopSettings settings;

        [BoxGroup("Visuals"), SerializeField]
        private Text iterationsText;
        [BoxGroup("Visuals"), SerializeField]
        private Text rangeText;
        [BoxGroup("Visuals"), SerializeField]
         private GameObject Plane;
        [BoxGroup("Visuals"), SerializeField]
        private float planeSize = 0.9f;
        [BoxGroup("Visuals"), SerializeField]
        private float planeGrowthOffset = 0.05f;
        [BoxGroup("Visuals"), SerializeField]
        private GameObject IncreaseAmountButton;

        private LoopSettings Settings => overrideGlobalSettings 
            ? settings 
            : ParentCell.Computer.GlobalLoopSettings;

        [PunRPC]
        private void SetIterationsRPC(int value) => Iterations = value;
        public int Iterations
        {
            get => iterations;
            
            private set
            {
                iterations = Mathf.Clamp(value, MinIterations, Settings.maxIterations);
                iterationsText.text = iterations.ToString();
            }
        }

        [PunRPC]
        private void SetRangeRPC(int value) => Range = value;
        public int Range
        {
            get => range;
            
            private set
            {
                if (value < MinRange)
                {
                    range = MinRange;
                    gameObject.SetActive(false);
                    return;
                }
                
                range = Mathf.Clamp(value, MinRange, _cachedMaxRange);
                IncreaseAmountButton.SetActive(range < _cachedMaxRange);
                rangeText.text = range == MinRange ? "X" : "A";
                UpdatePanelScale();
            }
        }
        
        private int _cachedMaxRange;

        private void OnEnable() => RefreshEarlierPanels();

        private void OnDisable() 
        {
            RefreshEarlierPanels();
            ResetRpc();
        }

        #region Increase/Decrease
        
        public void IncreaseIterations() =>
            photonView.RPC(nameof(SetIterationsRPC), RpcTarget.AllBuffered, Iterations + 1);

        public void DecreaseIterations() =>
            photonView.RPC(nameof(SetIterationsRPC), RpcTarget.AllBuffered, Iterations - 1);

        public void IncreaseRange() => 
            photonView.RPC(nameof(SetRangeRPC), RpcTarget.AllBuffered, Range + 1);
        
        public void DecreaseRange() => 
            photonView.RPC(nameof(SetRangeRPC), RpcTarget.AllBuffered, Range - 1);
        
        #endregion

        #region Refreshing
        
        private void UpdatePanelScale()
        {
            Vector3 position = Plane.transform.localPosition;
            position.y = -(Range - 1) * (.5f + planeGrowthOffset * .5f);
            Plane.transform.localPosition = position;

            Vector3 scale = Plane.transform.localScale;
            scale.z = Range * planeSize + (Range - 1) * planeSize * planeGrowthOffset;
            Plane.transform.localScale = scale;
        }

        private void RefreshEarlierPanels()
        {
            if (!ParentCell) return;
            
            for (int i = 0; i <= ParentCell.Index; i++)
            {
                ParentCell.Computer.Cells[i].LoopController.RefreshLimits();
            }
        }
        
        private void RefreshLimits()
        {
            if (!ParentCell) return;
            
            int index = ParentCell.Index;
            int panelCount = ParentCell.Computer.Cells.Count;
            int nextPanelIndex  = ParentCell.Computer.Cells.FindIndex(index + 1, cell => cell.HasLoop);
            if (nextPanelIndex == -1) nextPanelIndex = panelCount;
            
            _cachedMaxRange = Mathf.Min(nextPanelIndex - index, Settings.maxRange);
            Range = Range;
            Iterations = Iterations;
        }
        
        #endregion

        public void ResetLoopData() => photonView.RPC(nameof(ResetRpc), RpcTarget.AllBuffered);
        
        [PunRPC]
        private void ResetRpc()
        {
            Iterations = MinIterations;
            Range = MinRange;
        }
    }
}
