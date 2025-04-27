using UnityEngine;
using System.Collections.Generic;

public enum MapNodeType
{
    None,
    Normal,
    Elite,
    Shop,
    Event,
    Boss
}


public class MapGenerator : SingletonWithMono<MapGenerator>, IBaseManager
{
    public bool IsInitialized { get; set; }

    private Transform mapParent;
    private MapNode[,] nodes = new MapNode[5, 8];
    private HashSet<(int x, int y)> activeNodes = new HashSet<(int x, int y)>();

    public void Init()
    {
        mapParent = CanvasManager.Instance.GetMapParent();
        if (mapParent == null)
        {
            Debug.LogError("[MapGenerator] mapParent가 설정되지 않았습니다!");
            return;
        }

        MapNodeManager.Instance.Init();
        GenerateMap();
        IsInitialized = true;
    }

    private void GenerateMap()
    {
        float tileWidth = 150f;
        float tileHeight = 200f;
        float mapWidth = 5 * tileWidth;
        float mapHeight = 8 * tileHeight;
        Vector2 mapCenterOffset = new Vector2(mapWidth / 2f - tileWidth / 2f, mapHeight / 2f - tileHeight / 2f);

        nodes = new MapNode[5, 8];
        activeNodes.Clear();

        // 1. 8층(보스), 7층(상점), 1층(시작)만 중앙 노드 생성
        for (int y = 0; y < 8; y++)
        {
            int realFloor = 7 - y; // 실제 층 계산 (1층=0)

            if (realFloor == 0 || realFloor == 6 || realFloor == 7) // 1, 7, 8층만
            {
                int x = 2;

                MapNodeType nodeType = GetNodeTypeForFloor(y);
                int typeId = (int)nodeType;

                MapNode node = MapNodeManager.Instance.CreateObject(typeId);
                if (node == null)
                {
                    Debug.LogError($"노드 생성 실패: Floor {y}");
                    continue;
                }

                node.transform.SetParent(mapParent, false);
                node.Initialize(nodeType, x, y);
                nodes[x, y] = node;
                activeNodes.Add((x, y));

                RectTransform rect = node.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    x * tileWidth - mapCenterOffset.x,
                    (7 - y) * tileHeight - mapCenterOffset.y
                );
            }
        }

        // 2. 2~6층 노드 랜덤 추가
        for (int y = 2; y < 7; y++) // 2~6층
        {
            // 왼쪽 (x=0,1)
            List<int> leftCandidates = new List<int>();
            if (nodes[0, y] == null) leftCandidates.Add(0);
            if (nodes[1, y] == null) leftCandidates.Add(1);

            ShuffleList(leftCandidates);
            int leftCount = Random.Range(1, Mathf.Min(3, leftCandidates.Count + 1)); // 1~2개

            for (int i = 0; i < leftCount; i++)
            {
                int x = leftCandidates[i];

                MapNode leftNode = MapNodeManager.Instance.CreateObject((int)MapNodeType.Normal);
                leftNode.transform.SetParent(mapParent, false);
                leftNode.Initialize(MapNodeType.Normal, x, y);
                nodes[x, y] = leftNode;
                activeNodes.Add((x, y));

                RectTransform rect = leftNode.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    x * tileWidth - mapCenterOffset.x,
                    (7 - y) * tileHeight - mapCenterOffset.y
                );
            }

            // 오른쪽 (x=3,4)
            List<int> rightCandidates = new List<int>();
            if (nodes[3, y] == null) rightCandidates.Add(3);
            if (nodes[4, y] == null) rightCandidates.Add(4);

            ShuffleList(rightCandidates);
            int rightCount = Random.Range(1, Mathf.Min(3, rightCandidates.Count + 1)); // 1~2개

            for (int i = 0; i < rightCount; i++)
            {
                int x = rightCandidates[i];

                MapNode rightNode = MapNodeManager.Instance.CreateObject((int)MapNodeType.Normal);
                rightNode.transform.SetParent(mapParent, false);
                rightNode.Initialize(MapNodeType.Normal, x, y);
                nodes[x, y] = rightNode;
                activeNodes.Add((x, y));

                RectTransform rect = rightNode.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    x * tileWidth - mapCenterOffset.x,
                    (7 - y) * tileHeight - mapCenterOffset.y
                );
            }

            // 가운데 (x=2) 50% 확률로 추가
            if (nodes[2, y] == null && Random.value <= 0.5f)
            {
                MapNode centerNode = MapNodeManager.Instance.CreateObject((int)MapNodeType.Normal);
                centerNode.transform.SetParent(mapParent, false);
                centerNode.Initialize(MapNodeType.Normal, 2, y);
                nodes[2, y] = centerNode;
                activeNodes.Add((2, y));

                RectTransform rect = centerNode.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(
                    2 * tileWidth - mapCenterOffset.x,
                    (7 - y) * tileHeight - mapCenterOffset.y
                );
            }
        }

        // 3. 연결
        ConnectGeneratedNodes();
    }

    private void ConnectGeneratedNodes()
    {
        for (int y = 0; y < 7; y++) // 마지막층 제외
        {
            for (int x = 0; x < 5; x++)
            {
                MapNode currentNode = nodes[x, y];
                if (currentNode == null) continue;

                int nextY = y + 1;

                List<MapNode> candidates = new List<MapNode>();

                for (int nextX = 0; nextX < 5; nextX++)
                {
                    MapNode nextNode = nodes[nextX, nextY];
                    if (nextNode != null)
                    {
                        candidates.Add(nextNode);
                    }
                }

                if (candidates.Count > 0)
                {
                    candidates.Sort((a, b) => Mathf.Abs(a.x - x).CompareTo(Mathf.Abs(b.x - x)));

                    currentNode.ConnectTo(candidates[0]); // 가장 가까운 노드로 연결
                }
            }
        }

        // 추가: 다음 층 노드들이 반드시 연결 받게 보장
        for (int y = 1; y < 8; y++) // 1층부터 8층까지
        {
            for (int x = 0; x < 5; x++)
            {
                MapNode currentNode = nodes[x, y];
                if (currentNode == null) continue;

                bool hasIncoming = false;

                for (int prevX = 0; prevX < 5; prevX++)
                {
                    MapNode prevNode = nodes[prevX, y - 1];
                    if (prevNode != null && prevNode.connectedNodes.Contains(currentNode))
                    {
                        hasIncoming = true;
                        break;
                    }
                }

                // 연결이 하나도 안 들어온 경우
                if (!hasIncoming)
                {
                    List<MapNode> prevCandidates = new List<MapNode>();

                    for (int prevX = 0; prevX < 5; prevX++)
                    {
                        MapNode prevNode = nodes[prevX, y - 1];
                        if (prevNode != null)
                        {
                            prevCandidates.Add(prevNode);
                        }
                    }

                    if (prevCandidates.Count > 0)
                    {
                        prevCandidates.Sort((a, b) => Mathf.Abs(a.x - x).CompareTo(Mathf.Abs(b.x - x)));

                        prevCandidates[0].ConnectTo(currentNode); // 가장 가까운 이전 노드가 연결
                    }
                }
            }
        }
    }

    private void ShuffleList(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    private MapNodeType GetNodeTypeForFloor(int y)
    {
        int realFloor = 7 - y; // 7이 1층, 0이 8층

        if (realFloor == 0)
            return MapNodeType.Normal; // 1층 (시작)은 Normal
        if (realFloor == 6)
            return MapNodeType.Shop;   // 7층은 Shop
        if (realFloor == 7)
            return MapNodeType.Boss;   // 8층은 Boss

        // 2~6층은 랜덤 (Normal, Elite, Event)
        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0: return MapNodeType.Normal;
            case 1: return MapNodeType.Elite;
            case 2: return MapNodeType.Event;
        }

        return MapNodeType.Normal; // 기본값
    }
}
