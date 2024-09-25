namespace SSpot.Grids.NodeModifiers
{
    public interface IGridNodeModifier : IGameObjectProvider
    {
        void Modify(LevelGrid grid, Node node);
    }
}