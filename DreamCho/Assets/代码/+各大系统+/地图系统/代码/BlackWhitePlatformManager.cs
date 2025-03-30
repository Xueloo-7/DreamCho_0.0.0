using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlackWhitePlatformManager : MonoBehaviour
{
    struct Platform
    {
        public CompositeCollider2D collider;
        public Tilemap map;
        public float alpha;
    }

    [SerializeField] CompositeCollider2D whitePlatformCollider;
    [SerializeField] CompositeCollider2D blackPlatformCollider;
    [SerializeField] Tilemap whitePlatformSr;
    [SerializeField] Tilemap blackPlatformSr;

    Dictionary<Color, Platform> platformDic = new Dictionary<Color, Platform>();
    private Sequence sequence;

    private void Start()
    {
        // 初始化平台
        platformDic.Add(Color.white, new Platform { collider = whitePlatformCollider, map = whitePlatformSr, alpha = 0.2f });
        platformDic.Add(Color.black, new Platform { collider = blackPlatformCollider, map = blackPlatformSr, alpha = 0.5f });

        Event.onBlackWhiteSwitch += PlatformSwitch;
    }
    private void OnDisable()
    {
        Event.onBlackWhiteSwitch -= PlatformSwitch;
    }

    private void PlatformSwitch(Color color)
    {
        // 平台切换黑白
        // 当前平台颜色高亮，非当前平台颜色变淡并持续闪烁
        // 当前平台启用Collider，非当前平台禁用Collider

        foreach (var item in platformDic)
        {
            if (item.Key == color)
            {
                item.Value.map.ChangeAlpha(1);
                item.Value.collider.isTrigger = false;
                item.Value.map.GetComponent<TilemapRenderer>().sortingOrder = 0; // 当前平台置于最下层
            }
            else
            {
                FadeAnim(item.Value.map, item.Value.alpha); // 闪烁
                item.Value.collider.isTrigger = true;
                item.Value.map.GetComponent<TilemapRenderer>().sortingOrder = 1; // 非当前平台置于最上层
            }
        }
    }

    void FadeAnim(Tilemap tilemap, float startAlpha)
    {
        tilemap.ChangeAlpha(0.2f); // 设置初始颜色

        sequence.Kill(); // 停止闪烁
        sequence = DOTween.Sequence();
        Color newColor = tilemap.color;
        sequence.Append(DOVirtual.Float(startAlpha, 0, 0.3f, x => // 淡出
        {
            newColor.a = x;
            tilemap.color = newColor;
        }));
        sequence.Append(DOVirtual.Float(0, startAlpha, 0.3f, x => // 淡入
        {
            newColor.a = x;
            tilemap.color = newColor;
        }));
        sequence.AppendInterval(0.5f); // 闪烁间隔
        sequence.SetLoops(-1); // 循环
    }
}