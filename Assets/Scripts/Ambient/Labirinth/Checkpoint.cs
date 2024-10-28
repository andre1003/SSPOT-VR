using SSpot.Grids;

namespace SSpot.Labirinth
{
    public class Checkpoint : GridObject
    {
        public override void OnSteppedOn()
        {
            LabyrinthManager.Instance.SetCheckpoint(this);
        }
    }
}
