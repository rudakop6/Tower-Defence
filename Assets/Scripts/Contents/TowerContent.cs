public class TowerContent : Content<TowerType>
{
    public override void Recycle()
    {
        TowerPool.Instance.DestroyContent(this);
    }
}
