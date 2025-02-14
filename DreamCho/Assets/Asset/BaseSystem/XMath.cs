using UnityEngine;

public static class XMath
{
    #region Vector
    /// <summary>
    /// 向量取整
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
    public static Vector2 V_Round(Vector2 dir)
    {
        return new Vector2(Mathf.RoundToInt(dir.x), Mathf.RoundToInt(dir.y));
    }
    /// <summary>
    /// 将弧度转为Z轴角度
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="angle">角度偏移量</param>
    /// <returns></returns>
    public static float VecToA(Vector2 dir, float angle = 0)
    {
        return (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) + angle;
    }
    /// <summary>
    /// 将Z轴角度转为向量
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector2 AToVec(float angle)
    {
        return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
    /// <summary>
    /// 返回被选择Z轴角度后的vector向量
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static Vector2 RotateVector(Vector2 vector, float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);

        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos
        );
    }
    /// <summary>
    /// 根据传入向量判断大体方向并返回方向的stirng提示
    /// </summary>
    /// <param name="normalizedVector"></param>
    /// <returns></returns>
    public static string V_DirectionString(Vector2 normalizedVector)
    {
        // 确保传入的向量是归一化的
        normalizedVector = normalizedVector.normalized;

        float angle = VecToA(normalizedVector);

        if (angle < 0)
        {
            angle += 360; // 将角度调整为0-360范围
        }

        if (angle >= 345f || angle < 15f)
            return "Right";
        else if (angle >= 15f && angle < 75f)
            return "Up-Right";
        else if (angle >= 75f && angle < 105f)
            return "Up";
        else if (angle >= 105f && angle < 165f)
            return "Up-Left";
        else if (angle >= 165f && angle < 195f)
            return "Left";
        else if (angle >= 195f && angle < 255f)
            return "Down-Left";
        else if (angle >= 255f && angle < 285f)
            return "Down";
        else if (angle >= 285f && angle < 345f)
            return "Down-Right";

        Debug.LogWarning("Unknown");
        return "Unknown";
    }
    #endregion

    #region Mouse And Position
    public static Vector2 ScreenCenterPos()
    {
        return Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
    }
    // 获取鼠标在世界坐标的位置
    public static Vector2 MouseWorldPos()
    {
        var mousePos = MouseScreenPos();
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        return mousePos;
    }
    public static Vector2 MouseScreenPos()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = 10;
        return mousePos;
    }
    // 计算鼠标位置相对于屏幕中心的向量
    public static Vector2 WorldDirectionBy(Vector2 worldPos)
    {
        return (MouseWorldPos() - worldPos).normalized;
    }
    public static Vector2 ScreenDirectionBy(Vector2 screenPos)
    {
        return (MouseScreenPos() - screenPos).normalized;
    }
    public static Vector3 GetCanvasMousePos(RectTransform canvasRectTransform, Camera camera)
    {
        Vector2 mouseScreenPosition = MouseScreenPos();
        // 将屏幕坐标转换为 UI 的世界坐标
        Vector3 mouseWorldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvasRectTransform,
            mouseScreenPosition,
            camera, // 你的 Canvas 所使用的 Camera
            out mouseWorldPosition
        );
        return mouseWorldPosition;
    }
    public static Vector3 GetCanvasTouchPos(RectTransform canvasRectTransform, int touchID, Camera camera)
    {
        Vector2 touchScreenPosition = default;
        for (int i = 0; i < Input.touchCount; i++)
        {
            if (Input.GetTouch(i).fingerId == touchID)
            {
                touchScreenPosition = Input.GetTouch(i).position;
            }
        }

        // 将屏幕坐标转换为 UI 的世界坐标
        Vector3 touchWorldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvasRectTransform,
            touchScreenPosition,
            camera, // 你的 Canvas 所使用的 Camera
            out touchWorldPosition
        );
        return touchWorldPosition;
    }
    public static Vector3 GetCanvasPos(RectTransform canvasRectTransform, Vector2 screenPos, Camera camera)
    {
        // 将屏幕坐标转换为 UI 的世界坐标
        Vector3 touchWorldPosition;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvasRectTransform,
            screenPos,
            camera, // 你的 Canvas 所使用的 Camera
            out touchWorldPosition
        );
        return touchWorldPosition;
    }
    #endregion

    #region Layer
    /// <summary>
    /// 检测对象层级是否包含在mask中
    /// </summary>
    /// <param name="layer">对象层级索引</param>
    /// <param name="mask">层掩码，Ref有可用的层掩码</param>
    /// <returns></returns>
    public static bool LayerIs(int layer, int mask)
    {
        if (((1 << layer) & mask) != 0) return true;
        else return false;
    }
    #endregion

    #region Float
    public static float F(float number, int decimalPlaces)
    {
        float multiplier = Mathf.Pow(10, decimalPlaces);
        return Mathf.Round(number * multiplier) / multiplier;
    }
    // 封装计算startLifetime的函数
    public static float ThresholdValue(float value, float threshold = 5f, float decayMultiplier = 0.1f)
    {
        // 调整超出threshold的部分的增幅
        if (value > threshold)
        {
            float excess = value - threshold;
            value = Mathf.Lerp(value, threshold, excess * decayMultiplier);
        }

        return value;
    }
    #endregion

    /*#region Other
    public static Enemy GetNearestEnemy(List<Enemy> enemies)
    {
        // 检查敌人列表是否为空
        if (enemies == null || enemies.Count == 0)
        {
            return null; // 如果没有敌人，返回null
        }

        // 使用float.MaxValue初始化最小距离
        float closestDistanceSquared = float.MaxValue; // 存储最近敌人位置的平方距离
        Enemy closestEnemy = null; // 存储最近敌人的引用

        // 遍历敌人列表
        foreach (Enemy enemy in enemies)
        {
            // 如果玩家敌人之间没有遮挡
            if (Method.Instance.CheckRaycastHit(Player.Pos, enemy.transform.position, Ref.ground_mask))
            {
                // 计算玩家与敌人之间的平方距离
                float distanceSquared = (enemy.transform.position - Player.Pos).sqrMagnitude;

                // 如果当前敌人距离更近，且没有障碍遮挡，则更新最近敌人
                if (distanceSquared < closestDistanceSquared)
                {
                    closestDistanceSquared = distanceSquared;
                    closestEnemy = enemy;
                }
            }
        }

        // 返回最近的敌人
        return closestEnemy;
    }
    //排序敌人列表，从近到远
    public static List<Enemy> GetSortedEnemiesByPlayerDistance(List<Enemy> enemies)
    {
        // 检查敌人列表是否为空
        if (enemies == null || enemies.Count == 0)
        {
            return new List<Enemy>(); // 如果没有敌人，返回空列表
        }

        // 使用 LINQ 对敌人列表排序
        List<Enemy> sortedEnemies = enemies
            .OrderBy(enemy =>
            {
                // 检测敌人是否被阻挡
                bool isBlocked = Method.Instance.CheckRaycastHit(Player.Pos, (Vector2)enemy.transform.position, Ref.ground_mask);
                
                // 被阻挡的优先级设为 1，否则设为 0
                int blockPriority = isBlocked ? 1 : 0;
                // 返回排序权重：先按阻挡优先级排序，再按距离排序
                return (blockPriority, (enemy.transform.position - Player.Pos).sqrMagnitude);
            })
            .ToList();

        for (int i = 0; i < sortedEnemies.Count; i++) // 移除禁用元素
        {
            if (sortedEnemies[i].gameObject.activeSelf == false)
            {
                sortedEnemies.Remove(sortedEnemies[i]);
            }
        }

        return sortedEnemies;
    }
    public static List<Enemy> GetSortedEnemiesByDistance(List<Enemy> enemies, Vector2 startPos)
    {
        // 检查敌人列表是否为空
        if (enemies == null || enemies.Count == 0)
        {
            return new List<Enemy>(); // 如果没有敌人，返回空列表
        }

        // 使用 LINQ 对敌人列表排序
        List<Enemy> sortedEnemies = enemies
        .OrderBy(enemy =>
        {
            // 只按距离排序
            return (enemy.transform.position - Player.Pos).sqrMagnitude;
        })
        .ToList();

        return sortedEnemies;
    }
    #endregion*/
}
