using UnityEngine;

public class ArrowContent : Content<ArrowType>
{   
    public override void Recycle()
    {
        ArrowPool.Instance.DestroyContent(this);
    }
}

