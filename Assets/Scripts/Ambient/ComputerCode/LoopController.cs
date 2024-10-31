using System;
using System.Linq;
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
        public CodingCell ParentCell { get; set; }
        
        [BoxGroup("LoopSettings")]
        public int iterations;

        [BoxGroup("LoopSettings")]
        public int maxIterations = 10;

        [BoxGroup("LoopSettings")]
        public int maxRange = 6;

        [BoxGroup("LoopSettings")]
        public bool globalMaxRange = false;

        [BoxGroup("LoopSettings")]
        [ShowIf("globalMaxRange")]
        [MinValue(1)]
        public int globalRange = 1;

        [BoxGroup("Visuals")]
        public Text iterationsText;
        [BoxGroup("Visuals")]
        public Text rangeText;
        [BoxGroup("Visuals")]
        public GameObject Plane;
        [BoxGroup("Visuals")]
        public float planeSize = 0.9f;
        [BoxGroup("Visuals")]
        public int curRange = 1;
        [BoxGroup("Visuals")]
        public GameObject IncreaseAmountButton;

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
            iterations++;

            if(iterations > maxIterations)
            {
                iterations = maxIterations;
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
            iterations--;

            if(iterations < 1)
            {
                iterations = 1;
            }

            UpdateUI();
        }

        public void IncreaseRange()
        {
            if (curRange >= maxRange) 
                return;
        
            Vector3 tempVector = Plane.transform.localPosition;
            Plane.transform.localPosition = new (tempVector.x, tempVector.y - (planeSize / 2), tempVector.z);
            
            tempVector = Plane.transform.localScale;
            Plane.transform.localScale = new (tempVector.x, tempVector.y, tempVector.z * (1 + (1f / curRange)));
            
            curRange++;
            UpdateUI();
        }
        
        public void DecreaseRange()
        {
            if (curRange == 1)
                gameObject.SetActive(false);
        
            if (curRange <= 1)
                return;

            Vector3 positionVector = Plane.transform.localPosition;
            positionVector = new(positionVector.x, positionVector.y + (planeSize / 2), positionVector.z);
            Plane.transform.localPosition = positionVector;
        
            Vector3 scaleVector = Plane.transform.localScale;
            scaleVector = new(scaleVector.x, scaleVector.y, scaleVector.z * (1 - (1f / curRange)));
            Plane.transform.localScale = scaleVector;
        
            curRange--;
            UpdateUI();
        }

        [UsedImplicitly]
        public void DestroyLooper() 
        { 
            Destroy(gameObject);
        }

        private void UpdateUI()
        {
            iterationsText.text = iterations.ToString();
            IncreaseAmountButton.SetActive(curRange != maxRange);
            rangeText.text = curRange == 1 ? "X" : "A";
        }

        private void UpdateRange()
        {
            int index = ParentCell.Index;
            int panelCount = ParentCell.Computer.Cells.Count;
            int nextPanelIndex  = ParentCell.Computer.Cells.FindIndex(index + 1, cell => cell.HasLoop);
            if (nextPanelIndex == -1) nextPanelIndex = panelCount;
            
            int rangeToNext = nextPanelIndex - index;
            if (maxRange > rangeToNext) maxRange = rangeToNext;
            
            while (curRange > maxRange) 
                DecreaseRange();
            
            if (globalMaxRange && maxRange > globalRange) 
                maxRange = globalRange;
        }

        public void UpdateAllPanels()
        {
            foreach (var cell in ParentCell.Computer.Cells.Where(cell => cell.HasLoop))
            {
                cell.LoopController.UpdateRange();
                cell.LoopController.UpdateUI();
            }
        }

    }
}
