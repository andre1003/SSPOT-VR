using SSpot.Grids;

namespace SSPot.Ambient.Labyrinth
{
    public class Checkpoint : GridObject
    {
        public override void OnSteppedOn()
        {
            LabyrinthManager.Instance.SetCheckpoint(this);
        }
    }
}
