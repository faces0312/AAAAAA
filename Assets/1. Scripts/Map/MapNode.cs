using UnityEngine;
using UnityEngine.UI;

public class MapNode : MonoBehaviour
{
    public int x;
    public int y;
    public NodeType type;

    public void Init(int x, int y, NodeType type)
    {
        this.x = x;
        this.y = y;
        this.type = type;

        // ��: �ؽ�Ʈ�� �̹��� ����
        var label = GetComponentInChildren<Text>();
        if (label != null)
            label.text = type.ToString();
    }
}
