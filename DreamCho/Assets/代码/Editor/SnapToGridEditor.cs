using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
public class SnapToGridEditor : Editor
{
    private const float gridSize = 0.25f; // 网格间隔

    void OnSceneGUI()
    {
        if(Application.isPlaying) return; // 只在编辑模式下生效

        Transform t = (Transform)target;

        // 记录旧位置
        Vector3 oldPos = t.position;

        // 计算新的对齐位置
        Vector3 snappedPos = new Vector3(
            Mathf.Round(oldPos.x / gridSize) * gridSize,
            Mathf.Round(oldPos.y / gridSize) * gridSize,
            Mathf.Round(oldPos.z / gridSize) * gridSize
        );

        // 只有在位置真正发生变化时才更新
        if (t.position != snappedPos)
        {
            Undo.RecordObject(t, "Snap to Grid"); // 记录操作以支持撤销
            t.position = snappedPos;
        }
    }
}
