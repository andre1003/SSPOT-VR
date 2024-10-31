using System;
using JetBrains.Annotations;
using NaughtyAttributes;
using Photon.Pun;
using SSpot.Utilities;
using UnityEngine;
using UnityEngine.UI;

// TODO: comentar a classe e revisar algumas fun��es
namespace SSpot.Ambient.ComputerCode
{
    public class LoopController : MonoBehaviourPun
    {
        [Serializable]
        public class LoopSettings
        {
            [AllowNesting, MinValue(2)]
            public int maxIterations = 10;
            
            [AllowNesting, MinValue(1)]
            public int maxRange = 1;
        }
        
        public CodingCell ParentCell { get; set; }
        
        [field: BoxGroup("Current Values"), SerializeField, ReadOnly]
        public int Iterations { get; private set; } = 2;
        
        [field: BoxGroup("Current Values"), SerializeField, ReadOnly]
        public int Range { get; private set; } = 1;
        
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
        private GameObject IncreaseAmountButton;

        private LoopSettings Settings => overrideGlobalSettings 
            ? settings 
            : ParentCell.Computer.GlobalLoopSettings;

        private int _cachedMaxRange;

        private void OnEnable()
        {
            UpdateAllPanels();
        }

        private void OnDisable()
        {
            UpdateAllPanels();
        }
        
        [UsedImplicitly]
        public void IncreaseIterations()
        {
            photonView.RPC(nameof(IncreaseRPC), RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void IncreaseRPC()
        {
            Iterations++;

            if(Iterations > Settings.maxIterations)
            {
                Iterations = Settings.maxIterations;
            }

            UpdateUI();
        }

        [UsedImplicitly]
        public void DecreaseIterations()
        {
            photonView.RPC(nameof(DecreaseRPC), RpcTarget.AllBuffered);
        }

        [PunRPC]
        private void DecreaseRPC()
        {
            Iterations--;

            if(Iterations < 1)
            {
                Iterations = 1;
            }

            UpdateUI();
        }

        public void IncreaseRange()
        {
            if (Range >= _cachedMaxRange) 
                return;
        
            Vector3 tempVector = Plane.transform.localPosition;
            Plane.transform.localPosition = new (tempVector.x, tempVector.y - (planeSize / 2), tempVector.z);
            
            tempVector = Plane.transform.localScale;
            Plane.transform.localScale = new (tempVector.x, tempVector.y, tempVector.z * (1 + (1f / Range)));
            
            Range++;
            UpdateUI();
        }
        
        public void DecreaseRange()
        {
            if (Range == 1)
                gameObject.SetActive(false);
        
            if (Range <= 1)
                return;

            Vector3 positionVector = Plane.transform.localPosition;
            positionVector = new(positionVector.x, positionVector.y + (planeSize / 2), positionVector.z);
            Plane.transform.localPosition = positionVector;
        
            Vector3 scaleVector = Plane.transform.localScale;
            scaleVector = new(scaleVector.x, scaleVector.y, scaleVector.z * (1 - (1f / Range)));
            Plane.transform.localScale = scaleVector;
        
            Range--;
            UpdateUI();
        }

        [UsedImplicitly]
        public void DestroyLooper() 
        { 
            Destroy(gameObject);
        }

        private void UpdateUI()
        {
            iterationsText.text = Iterations.ToString();
            IncreaseAmountButton.SetActive(Range < _cachedMaxRange);
            rangeText.text = Range == 1 ? "X" : "A";
        }

        private void UpdateRange()
        {
            int index = ParentCell.Index;
            int panelCount = ParentCell.Computer.Cells.Count;
            int nextPanelIndex  = ParentCell.Computer.Cells.FindIndex(index + 1, cell => cell.HasLoop);
            if (nextPanelIndex == -1) nextPanelIndex = panelCount;
            
            int rangeToNext = nextPanelIndex - index;
            _cachedMaxRange = Mathf.Min(Settings.maxRange, rangeToNext);
            
            while (Range > _cachedMaxRange) 
                DecreaseRange();
        }

        public void UpdateAllPanels()
        {
            UpdateRange();
            UpdateUI();
        }
    }
}
