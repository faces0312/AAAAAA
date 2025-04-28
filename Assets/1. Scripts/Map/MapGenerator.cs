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

    private MapNode currentNode;

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

        currentNode = nodes[2, 0]; // 1층 중앙 노드 (y=0)
        UpdateNodeInteractivity(currentNode, false); // 시작할 땐 자기 자신 비활성화

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

        // 1. 8층(보스), 7층(상점), 1층(시작) 중앙 노드 생성
        for (int y = 0; y < 8; y++)
        {
            int realFloor = y; // y값 그대로 층 번호로 사용

            if (realFloor == 0 || realFloor == 6 || realFloor == 7)
            {
                int x = 2;

                MapNodeType nodeType = GetNodeTypeForFloor(realFloor);
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
                    y * tileHeight - mapCenterOffset.y // 수정: y * tileHeight
                );
            }
        }

        // 2. 2층~7층 노드 랜덤 추가
        for (int y = 1; y < 6; y++)
        {
            // 왼쪽 (x=0,1)
            List<int> leftCandidates = new List<int> { 0, 1 };
            ShuffleList(leftCandidates);

            int leftCount = Random.Range(1, 3);
            for (int i = 0; i < leftCount; i++)
            {
                int x = leftCandidates[i];

                if (nodes[x, y] == null)
                {
                    MapNodeType randomType = GetRandomNodeType();
                    MapNode node = MapNodeManager.Instance.CreateObject((int)randomType);

                    if (node != null)
                    {
                        node.transform.SetParent(mapParent, false);
                        node.Initialize(randomType, x, y);
                        nodes[x, y] = node;
                        activeNodes.Add((x, y));

                        RectTransform rect = node.GetComponent<RectTransform>();
                        rect.anchoredPosition = new Vector2(
                            x * tileWidth - mapCenterOffset.x,
                            y * tileHeight - mapCenterOffset.y
                        );
                    }
                }
            }

            // 오른쪽 (x=3,4)
            List<int> rightCandidates = new List<int> { 3, 4 };
            ShuffleList(rightCandidates);

            int rightCount = Random.Range(1, 3);
            for (int i = 0; i < rightCount; i++)
            {
                int x = rightCandidates[i];

                if (nodes[x, y] == null)
                {
                    MapNodeType randomType = GetRandomNodeType();
                    MapNode node = MapNodeManager.Instance.CreateObject((int)randomType);

                    if (node != null)
                    {
                        node.transform.SetParent(mapParent, false);
                        node.Initialize(randomType, x, y);
                        nodes[x, y] = node;
                        activeNodes.Add((x, y));

                        RectTransform rect = node.GetComponent<RectTransform>();
                        rect.anchoredPosition = new Vector2(
                            x * tileWidth - mapCenterOffset.x,
                            y * tileHeight - mapCenterOffset.y
                        );
                    }
                }
            }

            // 가운데 (x=2) 50% 확률 추가
            if (nodes[2, y] == null && Random.value <= 0.5f)
            {
                MapNodeType randomType = GetRandomNodeType();
                MapNode node = MapNodeManager.Instance.CreateObject((int)randomType);

                if (node != null)
                {
                    node.transform.SetParent(mapParent, false);
                    node.Initialize(randomType, 2, y);
                    nodes[2, y] = node;
                    activeNodes.Add((2, y));

                    RectTransform rect = node.GetComponent<RectTransform>();
                    rect.anchoredPosition = new Vector2(
                        2 * tileWidth - mapCenterOffset.x,
                        y * tileHeight - mapCenterOffset.y
                    );
                }
            }
        }

        // 3. 연결
        ConnectGeneratedNodes();
    }

    private void ConnectGeneratedNodes()
    {
        for (int y = 0; y < 8; y++)
        {
            if (y == 0) //  1층 (y=0)
            {
                MapNode startNode = nodes[2, 0];
                if (startNode != null)
                {
                    for (int x = 0; x < 5; x++)
                    {
                        MapNode nextNode = nodes[x, 1]; // 2층(y=1)
                        if (nextNode != null)
                        {
                            startNode.ConnectTo(nextNode);
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; x < 5; x++)
                {
                    MapNode currentNode = nodes[x, y];
                    if (currentNode == null) continue;

                    int nextY = y + 1;

                    if (nextY < 8) // 범위 체크
                    {
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
                            currentNode.ConnectTo(candidates[0]);
                        }
                    }
                }
            }
        }

        // 보정 연결: 끊긴 노드 방지
        for (int y = 1; y < 8; y++)
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
                        prevCandidates[0].ConnectTo(currentNode);
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

    private MapNodeType GetNodeTypeForFloor(int floor)
    {
        if (floor == 0)
            return MapNodeType.Normal;
        if (floor == 6)
            return MapNodeType.Shop;
        if (floor == 7)
            return MapNodeType.Boss;
        return MapNodeType.Normal;
    }

    private const float NormalProbability = 0.4f;
    private const float EliteProbability = 0.3f;

    private MapNodeType GetRandomNodeType()
    {
        float rand = Random.Range(0f, 1f);

        if (rand <= NormalProbability)
            return MapNodeType.Normal;
        else if (rand <= NormalProbability + EliteProbability)
            return MapNodeType.Elite;
        else
            return MapNodeType.Event;
    }

    public void UpdateNodeInteractivity(MapNode current, bool includeSelf)
    {
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 5; x++)
            {
                MapNode node = nodes[x, y];
                if (node != null && node.nodeButton != null)
                    node.nodeButton.interactable = false;
            }
        }

        if (includeSelf && current != null && current.nodeButton != null)
            current.nodeButton.interactable = true;

        foreach (var next in current.connectedNodes)
        {
            if (next != null && next.nodeButton != null)
            {
                if (next.y == current.y + 1) // y+1로 연결된 노드만 활성화
                    next.nodeButton.interactable = true;
            }
        }
    }

    public void OnMoveToNode(MapNode targetNode)
    {
        currentNode = targetNode;
        UpdateNodeInteractivity(currentNode, false);
    }
}
