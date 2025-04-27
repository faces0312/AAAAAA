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
                    Debug.Log($"노드 프리팹 로드 완료: {prefab.name} (ID: {id})");
                }
            }
            else
            {
                Debug.LogError($"프리팹에 MapNode 컴포넌트가 없음: {prefab.name}");
            }
        }
    }

    public override MapNode CreateObject(int id)
    {
        if (!prefabMap.ContainsKey(id))
        {
            Debug.LogError($"노드 프리팹을 찾을 수 없음: ID {id}");
            return null;
        }

        GameObject nodeObj = Instantiate(prefabMap[id]);
        MapNode node = nodeObj.GetComponent<MapNode>();

        if (node == null)
        {
            Debug.LogError($"생성된 오브젝트에 MapNode 컴포넌트가 없음: ID {id}");
            Destroy(nodeObj);
            return null;
        }

        return node;
    }
}