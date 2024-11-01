using SSpot.Grids;

namespace SSPot.Ambient.Labyrinth
{
    public class LabyrinthEnd : GridObject
    {
        public override void OnSteppedOn()
        {
            LabyrinthManager.Instance?.LabyrinthSuccess();
        }
    }
}
