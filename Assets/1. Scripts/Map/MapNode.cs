using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapNode : MonoBehaviour
{
    public MapNodeType type;
    public int x, y;

    public List<MapNode> connectedNodes = new List<MapNode>();
    private List<Image> lines = new List<Image>(); // ���ἱ ����Ʈ

    [SerializeField] private GameObject linePrefab; // ���ἱ�� ������ (RectTransform + Image)

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
        // Ÿ�Կ� ���� �������̳� ���� ���� ���� (���� Ȯ�� ����)
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

            // �θ�(Canvas) �������� World �� Local ��ǥ ��ȯ
            Vector3 startWorldPos = this.GetComponent<RectTransform>().position;
            Vector3 endWorldPos = targetNode.GetComponent<RectTransform>().position;

            Vector2 startLocalPos = transform.parent.InverseTransformPoint(startWorldPos);
            Vector2 endLocalPos = transform.parent.InverseTransformPoint(endWorldPos);

            Vector2 direction = (endLocalPos - startLocalPos).normalized;
            float distance = Vector2.Distance(startLocalPos, endLocalPos);

            // ���� ����
            lineRect.sizeDelta = new Vector2(distance, 5f); // Width: �Ÿ�, Height: �β�
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            lineRect.rotation = Quaternion.Euler(0, 0, angle);

            // ���������� ���� �Ÿ���ŭ �о ����
            lineRect.anchoredPosition = startLocalPos + direction * (distance * 0.5f);

            lines.Add(lineRect.GetComponent<Image>());
        }
    }
}
