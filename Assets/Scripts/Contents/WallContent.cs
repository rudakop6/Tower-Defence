public class WallContent : TileContent
{
    public override void Recycle()
    {
        TileContentPool.Instance.DestroyContent(this);
        TowerContent.Recycle();
    }
    private TowerContent _towerContent;
    public TowerContent TowerContent
    {
        get => _towerContent;
        set
        {
            if (_towerContent != null)
                _towerContent.Recycle();

            _towerContent = value;
            if (_towerContent.Type != TowerType.Empty)
            {
                _towerContent.transform.localPosition = Helper.GetPositionChildren(transform, _towerContent);
            }
        }
    }
}

