using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Extensions
{
    public static void ChangeAlpha(this Tilemap tilemap, float alpha)
    {
        Color color = tilemap.color;
        color.a = alpha;
        tilemap.color = color;
    }
    public static void ChangeAlpha(this SpriteRenderer sr, float alpha)
    {
        Color color = sr.color;
        color.a = alpha;
        sr.color = color;
    }

    public static void SnapRotationTo90(this Transform obj)
    {
        Vector3 euler = obj.rotation.eulerAngles;

        euler.x = Mathf.Round(euler.x / 90) * 90;  // x 轴取整（如有需要）
        euler.y = Mathf.Round(euler.y / 90) * 90;  // y 轴取整
        euler.z = Mathf.Round(euler.z / 90) * 90;  // z 轴取整（如有需要）

        obj.rotation = Quaternion.Euler(euler);
    }
}
