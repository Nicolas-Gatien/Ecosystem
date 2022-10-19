using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    // FIELDS
    [SerializeField] private SpawnObject[] spawnObjects;
    private Neat neat;
    public LayerMask spawnOn;
    public LayerMask spawnOff;

    // METHODS
    private void Awake()
    {
        neat = new Neat(10, 2, 1);   
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

            Color color = Random.ColorHSV(0, 1, 0, 1, 0.5f, 1, 1, 1);

            creature.neat = neat;
            creature.Genome = neat.EmptyGenome();
            creature.Genome.MutateLink(creature);
            creature.Genome.MutateLink(creature);
            creature.Mutate();
            creature.Mutate();
            creature.Mutate();
            creature.color = color;
        }
    }

    Vector2 GetRandomPosition (SpawnObject spawnObject)
    {
        Vector2 pos = new Vector2(Random.Range(spawnObject.minPos.x, spawnObject.maxPos.x), Random.Range(spawnObject.minPos.y, spawnObject.maxPos.y));
        while (Physics2D.OverlapCircle(pos, 0.1f, spawnOn) == false || Physics2D.OverlapCircle(pos, 0.5f, spawnOff) == true)
        {
            pos = new Vector2(Random.Range(spawnObject.minPos.x, spawnObject.maxPos.x), Random.Range(spawnObject.minPos.y, spawnObject.maxPos.y));
        }
        return pos;
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
