using SSpot.Grids;

namespace SSpot.Labirinth
{
    public class LabyrinthEnd : GridObject
    {
        public override void OnSteppedOn()
        {
            LabyrinthManager.Instance?.LabyrinthSuccess();
        }
    }
}
