public class TileContent : Content<TileContentType>
{
    public ContentType ContentType
    {
        get
        {
            switch (Type)
            {
                case TileContentType.SpawnPoint:
                case TileContentType.Wall:
                case TileContentType.Destination:
                    return ContentType.Building;
                case TileContentType.SpawnPointProjection:
                case TileContentType.WallProjection:                
                case TileContentType.DestinationProjection:
                default:
                    return ContentType.Projection;
            }
        }

    }
    public override void Recycle()
    {
        switch (ContentType)
        {
            case ContentType.Building:
                BuildingPool.Instance.DestroyContent(this);
                break;
            case ContentType.Projection:
                ProjectionPool.Instance.DestroyContent(this);
                break;
        } 
    }
}

