using UnityEngine;

namespace SSpot.Grids
{
    public class GridStepObjectiveObject : GridObject
    {
        [SerializeField] private GameObject inactive;
        [SerializeField] private GameObject active;

        private bool _active;
        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
                inactive.SetActive(!_active);
                active.SetActive(_active);
            }
        }

        private void Awake()
        {
            Active = false;
        }

        public override void OnSteppedOn()
        {
            if (Active) return;
            
            Active = true;
            Debug.Log("Report objective stepped on");
        }
    }
}