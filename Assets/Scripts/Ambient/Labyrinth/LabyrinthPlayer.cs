using System;
using UnityEngine;
using UnityEngine.UI;

namespace SSPot.Ambient.Labyrinth
{
    [Serializable]
    public class LabyrinthPlayer : MonoBehaviour
    {
        [field: SerializeField]
        public GameObject Computer { get; private set; }
    
        [field: SerializeField]
        public GameObject Roof { get; private set; }
    
        [field: SerializeField]
        public GameObject TV { get; private set; }
    
        [field: SerializeField]
        public GameObject Instructions { get; private set; }
    
        //TODO why are we not using TMPro
        [field: SerializeField]
        public Text InstructionsText { get; private set; }
        
        /*[field: SerializeField]
        public ResetCubes ResetCubes { get; private set; }
        
        [field: SerializeField]
        public RunCubes RunCubes { get; private set; }*/
    
        public int Index { get; private set; }
    
        public bool IsLocal { get; private set; }
    
        public void Init(int index, int localIndex)
        {
            Index = index;
            IsLocal = localIndex == index;
        
            Roof.SetActive(IsLocal);
        }
    
        public void SetCoder(bool isCoder)
        {
            Computer.SetActive(isCoder);
            TV.SetActive(!isCoder);
        }
    
        public void SetInstructions(string instructions)
        {
            InstructionsText.text = instructions;
        }
    
        public void SetActive(bool isActive)
        {
            Computer.SetActive(isActive);
            Roof.SetActive(isActive);
            TV.SetActive(isActive);
            Instructions.SetActive(isActive);
        }
    }
}