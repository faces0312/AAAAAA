using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class MapNodeManager : BaseObjectManager<MapNodeManager, MapNode>
{
    private Dictionary<int, GameObject> prefabMap = new Dictionary<int, GameObject>();

    public override void Init()
    {
        base.Init();
        LoadPrefabs();
    }

    private void LoadPrefabs()
    {
        GameObject[] nodePrefabs = Resources.LoadAll<GameObject>("Prefabs/Map");

        foreach (var prefab in nodePrefabs)
        {
            MapNode nodeComponent = prefab.GetComponent<MapNode>();

            if (nodeComponent != null)
            {
                int id = (int)nodeComponent.type;
                if (!prefabMap.ContainsKey(id))
                {
                    prefabMap.Add(id, prefab);
                    Debug.Log($"��� ������ �ε� �Ϸ�: {prefab.name} (ID: {id})");
                }
            }
            else
            {
                Debug.LogError($"�����տ� MapNode ������Ʈ�� ����: {prefab.name}");
            }
        }
    }

    public override MapNode CreateObject(int id)
    {
        if (!prefabMap.ContainsKey(id))
        {
            Debug.LogError($"��� �������� ã�� �� ����: ID {id}");
            return null;
        }

        GameObject nodeObj = Instantiate(prefabMap[id]);
        MapNode node = nodeObj.GetComponent<MapNode>();

        if (node == null)
        {
            Debug.LogError($"������ ������Ʈ�� MapNode ������Ʈ�� ����: ID {id}");
            Destroy(nodeObj);
            return null;
        }

        return node;
    }
}