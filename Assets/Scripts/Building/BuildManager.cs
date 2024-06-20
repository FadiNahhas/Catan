using System.Collections;
using System.Collections.Generic;
using Helpers;
using Map;
using Sirenix.OdinInspector;
using UnityEngine;

public class BuildManager : Singleton<BuildManager>
{
    [TabGroup("Prefabs")] public GameObject settlementPrefab;

    public void Build(BuildingType type, int player, Vector3 pos)
    {
        Instantiate(settlementPrefab, pos, Quaternion.identity);
    }
}
