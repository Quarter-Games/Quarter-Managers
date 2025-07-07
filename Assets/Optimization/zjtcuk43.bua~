using UnityEngine;
using System.Collections.Generic;

public class GardenOfCubes : MonoBehaviour
{
    public GameObject cubePrefab;

    private List<GameObject> garden = new List<GameObject>();
    private float timer = 0f;

    private static readonly Vector3 up = Vector3.up;
    private static readonly Color white = Color.white;
    private static readonly Color green = Color.green;
    private const string specialCubeName = "42";

    private HashSet<GameObject> specialCubes = new HashSet<GameObject>();

    void Start()
    {
        for (int i = 0; i < 300; i++)
        {
            GameObject cube = Instantiate(cubePrefab);
            cube.transform.position = new Vector3(i % 20, 0f, i / 20);

            string name = $"Cube_{i}";
            cube.name = name;

            if (name.Contains(specialCubeName))
                specialCubes.Add(cube);

            garden.Add(cube);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        float time = Time.time;

        foreach (var cube in garden)
        {
            Vector3 pos = cube.transform.position;
            pos.y += Mathf.Sin(time + pos.x) * 0.001f;
            cube.transform.position = pos;

            if (specialCubes.Contains(cube))
            {
                Debug.Log($"This cube is special: {cube.name}");
            }

            if (cube.CompareTag("Untagged"))
            {
                Renderer renderer = cube.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.Lerp(white, green, Mathf.PingPong(time, 1));
                }
            }
        }

        if (timer >= 5f)
        {
            Debug.Log("Garden is peaceful... but something is off.");
            timer = 0f;
        }
    }
}
