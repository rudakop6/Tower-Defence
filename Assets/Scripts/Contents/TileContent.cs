public class TileContent : Content<TileContentType>
{
    public override void Recycle()
    {
        TileContentPool.Instance.DestroyContent(this);
    }
}

