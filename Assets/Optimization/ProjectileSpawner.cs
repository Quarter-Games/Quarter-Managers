using UnityEngine;
using System.Collections.Generic;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] int MaxCapacity = 20;
    public GameObject projectilePrefab;
    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject go = Instantiate(projectilePrefab);
            go.SetActive(false);
            pool.Enqueue(go);
        }
    }

    void Update()
    {
        GameObject proj = pool.Peek();
        if (!proj.activeInHierarchy)
        {
            pool.Dequeue();
            proj.transform.position = transform.position;
            proj.SetActive(true);
            pool.Enqueue(proj);
        }
        else
        {
            if (pool.Count >= MaxCapacity) return;
            GameObject newProj = Instantiate(projectilePrefab);
            newProj.transform.position = transform.position;
            newProj.SetActive(true);
            pool.Enqueue(newProj);
        }
    }
}
