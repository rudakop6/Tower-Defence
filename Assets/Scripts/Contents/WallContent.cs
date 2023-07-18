public class WallContent : TileContent
{
    public override void Recycle()
    {
        TowerContent = null;
        BuildingPool.Instance.DestroyContent(this);        
    }
    private TowerContent _towerContent;
    public TowerContent TowerContent
    {
        get => _towerContent;
        set
        {
            _towerContent?.Recycle();
            _towerContent = value;

            if(_towerContent != null)
                _towerContent.transform.localPosition = Helper.GetPositionChildren(transform, _towerContent);
        }
    }
}

