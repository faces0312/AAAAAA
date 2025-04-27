using UnityEngine;
using System.Collections.Generic;

public enum NodeType
{
    Normal,
    Elite,
    Shop,
    Event,
    Boss
}


public class MapGenerator : MonoBehaviour
{
    public int width = 5;
    public int height = 8;
    public RectTransform mapParent;
    public float xSpacing = 150f;
    public float ySpacing = 150f;

    private List<MapNode> mapNodes = new List<MapNode>();

    void Start()
    {
        MapNodeManager.Instance.Init();
        GenerateMap();
    }

    void GenerateMap()
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                NodeType type = GetNodeType(x, y);
                int id = (int)type;

                Vector3 position = new Vector3(x * xSpacing, y * ySpacing, 0);

                // 프리팹 생성 및 설정
                MapNode node = MapNodeManager.Instance.Spawn(id, Vector3.zero, Quaternion.identity);
                if (node != null)
                {
                    node.transform.SetParent(mapParent, false);
                    node.GetComponent<RectTransform>().anchoredPosition = new Vector2(position.x, position.y);
                    node.Init(x, y, type);

                    mapNodes.Add(node);
                }
            }
        }
    }

    NodeType GetNodeType(int x, int y)
    {
        if (y == 0) return NodeType.Normal;
        if (y == 6) return NodeType.Shop;
        if (y == 7) return NodeType.Boss;

        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0: return NodeType.Normal;
            case 1: return NodeType.Event;
            case 2: return NodeType.Elite;
        }

        return NodeType.Normal;
    }
}
