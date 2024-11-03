using SSpot.Grids;
using SSpot.Level;

namespace SSPot.Grids
{
    public class Checkpoint : GridObject
    {
        public override void OnSteppedOn()
        {
            base.OnSteppedOn();
            
            LevelManager.Instance.Robot.Mover.SetOriginalPosition(GridPosition, Facing);
        }
    }
}
