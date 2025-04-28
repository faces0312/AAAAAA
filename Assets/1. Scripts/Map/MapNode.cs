using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapNode : MonoBehaviour
{
    public MapNodeType type;
    public int x, y;

    public List<MapNode> connectedNodes = new List<MapNode>();
    private List<Image> lines = new List<Image>(); // 연결선 리스트

    [SerializeField] private GameObject linePrefab; // 연결선용 프리팹 (RectTransform + Image)

    public Button nodeButton;

    public void Initialize(MapNodeType type, int x, int y)
    {
        if (nodeButton == null)
            nodeButton = GetComponent<Button>();

        nodeButton.onClick.RemoveAllListeners();
        nodeButton.onClick.AddListener(() => MapGenerator.Instance.OnMoveToNode(this));
        this.type = type;
        this.x = x;
        this.y = y;
        UpdateUI();
    }

    private void UpdateUI()
    {
        // 타입에 따라 아이콘이나 색상 변경 가능 (추후 확장 가능)
    }

    public void ConnectTo(MapNode targetNode)
    {
        if (targetNode == null)
            return;

        if (!connectedNodes.Contains(targetNode))
            connectedNodes.Add(targetNode);

        if (linePrefab != null)
        {
            GameObject lineObj = Instantiate(linePrefab, this.transform.parent);
            RectTransform lineRect = lineObj.GetComponent<RectTransform>();

            // 부모(Canvas) 기준으로 World → Local 좌표 변환
            Vector3 startWorldPos = this.GetComponent<RectTransform>().position;
            Vector3 endWorldPos = targetNode.GetComponent<RectTransform>().position;

            Vector2 startLocalPos = transform.parent.InverseTransformPoint(startWorldPos);
            Vector2 endLocalPos = transform.parent.InverseTransformPoint(endWorldPos);

            Vector2 direction = (endLocalPos - startLocalPos).normalized;
            float distance = Vector2.Distance(startLocalPos, endLocalPos);

            // 라인 설정
            lineRect.sizeDelta = new Vector2(distance, 5f); // Width: 거리, Height: 두께
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            lineRect.rotation = Quaternion.Euler(0, 0, angle);

            // 시작점에서 절반 거리만큼 밀어서 연결
            lineRect.anchoredPosition = startLocalPos + direction * (distance * 0.5f);

            lines.Add(lineRect.GetComponent<Image>());
        }
    }
}
