using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MonsterSpawner : MonoBehaviour
{

    [SerializeField]
    private HexGrid _grid;

    [SerializeField]
    private GameObject _monsterPrefab;

    [SerializeField]
    private int _monstersToGenerate;

    [SerializeField]
    private GeneralEvent _monsterGenerated;

    [SerializeField]
    private GeneralEvent _entityPrepared;

    public void OnEntityPrepared(EventArgs args)
    {
        EntityPreparedEventArgs entityPreparedEventArgs = (EntityPreparedEventArgs)args;

        if(entityPreparedEventArgs.EntityPrepareType == EntityPrepareType.GRID && entityPreparedEventArgs.Prepared)
        {
            StartCoroutine(GenerateMonsters());
        }
    }

    private IEnumerator GenerateMonsters()
    {
        int monstersGenerated = 0; 
        List<Node> nodes = _grid.GetNodeList();
        List<GameObject> monsters = new List<GameObject>();

        while (monstersGenerated < Mathf.Min(_monstersToGenerate, nodes.Count - 1))
        {
            int randomNodeIndex = Random.Range(0, nodes.Count);

            Node n = nodes[randomNodeIndex];

            if (!n.IsOccupied())
            {
                GameObject enemyobject = Instantiate(_monsterPrefab, n.WorldPosition, Quaternion.identity, this.transform);
                MonsterMovement monsterMovement = enemyobject.GetComponent<MonsterMovement>();
                monsterMovement.SetCurrentNode(n);
                n.SetEnemyObject(enemyobject);
                monsters.Add(enemyobject);
                monstersGenerated++;
            }
            yield return null;
        }

        _monsterGenerated.Raise(new MonstersGeneratedEventArgs(monsters));
        _entityPrepared.Raise(new EntityPreparedEventArgs(EntityPrepareType.MONSTERS, true));
    }
}
