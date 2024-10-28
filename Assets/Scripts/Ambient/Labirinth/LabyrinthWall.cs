using SSpot.Grids;

namespace SSpot.Labirinth
{
    public class LabyrinthWall : GridObject
    {
        public override void OnSteppedOn()
        {
            //TODO cant step on bro stupid
            LabyrinthManager.Instance.ResetRobot();
        }
    }
}
