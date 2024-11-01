using SSpot.Grids;

namespace SSPot.Ambient.Labyrinth
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
