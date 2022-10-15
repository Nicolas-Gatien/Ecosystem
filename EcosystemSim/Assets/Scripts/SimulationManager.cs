using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    // FIELDS
    [SerializeField] private SpawnObject[] spawnObjects;
    private Neat neat;

    // METHODS
    private void Awake()
    {
        neat = new Neat(9, 2, 1);   
    }

    void Start()
    {
        foreach (SpawnObject item in spawnObjects)
        {
            for (int i = 0; i < item.initialSpawnAmount; i++)
            {
                SpawnPrefab(item);
            }
        }
    }

    void Update()
    {
        foreach (SpawnObject item in spawnObjects)
        {
            if (!item.recureSpawn)
            {
                continue;
            }

            if (item.timeBeforeNextSpawn < 0)
            {
                SpawnPrefab(item);
                item.timeBeforeNextSpawn = item.timeBtwRecurringSpawn;
            }else
            {
                item.timeBeforeNextSpawn -= Time.deltaTime;
            }
        }
    }

    void SpawnPrefab(SpawnObject item)
    {

        GameObject obj = Instantiate(item.prefab, GetRandomPosition(item), Quaternion.identity);
        Creature creature = obj.GetComponent<Creature>();

        if (creature != null)
        {
            creature.neat = neat;
            creature.Genome = neat.EmptyGenome();
            creature.Genome.MutateLink();
        }
    }

    Vector2 GetRandomPosition (SpawnObject spawnObject)
    {
        return new Vector2(Random.Range(spawnObject.minPos.x, spawnObject.maxPos.x), Random.Range(spawnObject.minPos.y, spawnObject.maxPos.y));
    }
}

[System.Serializable]
public class SpawnObject
{
    public GameObject prefab;
    public int initialSpawnAmount;

    public bool recureSpawn;
    public float timeBtwRecurringSpawn;
    public float timeBeforeNextSpawn;

    public Vector2 minPos;
    public Vector2 maxPos;
}
