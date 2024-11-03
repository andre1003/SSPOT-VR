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
        public GameObject TV { get; private set; }
    
        [field: SerializeField]
        public GameObject Instructions { get; private set; }
    
        [field: SerializeField]
        public Text InstructionsText { get; private set; }
        
        [field: SerializeField]
        public GameObject ErrorScreen { get; private set; }
    
        public int Index { get; private set; }
    
        public bool IsLocal { get; private set; }
    
        public void Init(int index, int localIndex)
        {
            Index = index;
            IsLocal = localIndex == index;
        }
    
        public void SetCoder(bool isCoder)
        {
            TV.SetActive(!isCoder);
            Computer.SetActive(isCoder);
            ErrorScreen.SetActive(isCoder);
        }
    
        public void SetInstructions(string instructions)
        {
            InstructionsText.text = instructions;
        }
    
        public void SetObjectsActive(bool isActive)
        {
            Computer.SetActive(isActive);
            TV.SetActive(isActive);
            Instructions.SetActive(isActive);
            ErrorScreen.SetActive(isActive);
        }
    }
}