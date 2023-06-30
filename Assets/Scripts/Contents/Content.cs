using System;
using UnityEngine;

public abstract class Content<T> : MonoBehaviour where T : Enum
{
    [SerializeField]
    private MeshFilter _meshFilter;
    [SerializeField]
    private T _type;
    public T Type => _type;

    public float OffsetY { get; private set; } = 0f;

    public abstract void Recycle();
    private void Awake()
    {
        if(_meshFilter != null)
        {
            //_meshFilter = GetComponent<MeshFilter>();
            if (_meshFilter?.mesh != null)
                SetOffset();
        }              
    }

    private void SetOffset()
    {      
        Bounds bounds = _meshFilter.sharedMesh.bounds;
        float boundsOffset;

        if (_meshFilter.transform.localRotation == Quaternion.Euler(90, 0, 0))        
            boundsOffset = (bounds.extents.z - bounds.center.z);
        else        
            boundsOffset = (bounds.extents.y - bounds.center.y);
        
        OffsetY = boundsOffset * transform.localScale.y;
    }
}

