
using System;
using UnityEngine;

public static class Helper
{
    /// <summary>
    /// Расширение типа int для проверки вхождения в диапазон
    /// </summary>
    /// <returns>Возвращает true, если значение лежит между min и max</returns>
    public static bool CheckRange(this int value, int min, int max, Inclusive inclusive)
    {
        switch (inclusive)
        {
            case Inclusive.None:
                return value > min && value < max;
            case Inclusive.Minimum:
                return value >= min && value < max;
            case Inclusive.Maximum:
                return value > min && value <= max;
            default:
                return value >= min && value <= max;
        }
    }
    public static Vector3 GetPositionChildren(Transform parent, Content<TileContentType> children)
    {
        float positionY = children.OffsetY;
        return parent.localPosition + new Vector3(0, parent.localScale.y + positionY, 0);
    }
    public static Vector3 GetPositionChildren(Transform parent, Content<TowerType> children)
    {
        float positionY = children.OffsetY;
        return parent.localPosition + new Vector3(0, parent.localScale.y + positionY, 0);
    }
}

