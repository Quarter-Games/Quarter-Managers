using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Talent Library", menuName = "TD/Meta/Talents/Library")]
public class TalentLibrary : ScriptableObject, IList<Talent>
{
    [SerializeField] List<Talent> talents = new();
    public int StartPrice = 100;
    public float regularPriceMultiplier = 1.1f;
    public float bigPriceMultiplier = 2f;
    #region IList Implementation
    public int Count => talents.Count;
    public bool IsReadOnly => false;
    public Talent this[int index] { get => talents[index]; set => talents[index] = value; }
    public void Add(Talent item) => talents.Add(item);
    public void Clear() => talents.Clear();
    public bool Contains(Talent item) => talents.Contains(item);
    public void CopyTo(Talent[] array, int arrayIndex) => talents.CopyTo(array, arrayIndex);
    public int IndexOf(Talent item) => talents.IndexOf(item);
    public void Insert(int index, Talent item) => talents.Insert(index, item);
    public bool Remove(Talent item) => talents.Remove(item);
    public void RemoveAt(int index) => talents.RemoveAt(index);

    // Custom enumerator for graph traversal (topological order)
    public IEnumerator<Talent> GetEnumerator()
    {
        return new TalentGraphEnumerator(talents);
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private class TalentGraphEnumerator : IEnumerator<Talent>
    {
        private readonly List<Talent> talents;
        private readonly List<Talent> sortedTalents;
        private int index = -1;

        public TalentGraphEnumerator(List<Talent> talents)
        {
            this.talents = talents;
            sortedTalents = TopologicalSort(talents);
        }

        public Talent Current => sortedTalents[index];
        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            index++;
            return index < sortedTalents.Count;
        }

        public void Reset()
        {
            index = -1;
        }

        public void Dispose() { }

        // Kahn's algorithm for topological sort
        private static List<Talent> TopologicalSort(List<Talent> talents)
        {
            var inDegree = new Dictionary<Talent, int>();
            var graph = new Dictionary<Talent, List<Talent>>();

            foreach (var talent in talents)
            {
                if (!inDegree.ContainsKey(talent))
                    inDegree[talent] = 0;
                foreach (var dep in talent.DependsOn)
                {
                    if (!graph.ContainsKey(dep))
                        graph[dep] = new List<Talent>();
                    graph[dep].Add(talent);
                    if (!inDegree.ContainsKey(talent))
                        inDegree[talent] = 0;
                    inDegree[talent]++;
                }
            }

            // Find nodes with no dependencies (in-degree 0)
            var queue = new Queue<Talent>(talents.Where(t => (!inDegree.ContainsKey(t) || inDegree[t] == 0)));
            var result = new List<Talent>();

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (!result.Contains(node))
                    result.Add(node);
                if (graph.ContainsKey(node))
                {
                    foreach (var neighbor in graph[node])
                    {
                        inDegree[neighbor]--;
                        if (inDegree[neighbor] == 0)
                            queue.Enqueue(neighbor);
                    }
                }
            }

            // If result.Count != talents.Count, there is a cycle or missing nodes
            return result;
        }
    }
    #endregion
    public void OnValidate()
    {
#if UNITY_EDITOR

        // Find all Talent assets in the same folder and subfolders as this TalentLibrary
        string libraryPath = UnityEditor.AssetDatabase.GetAssetPath(this);
        string folderPath = System.IO.Path.GetDirectoryName(libraryPath);

        // Search for all Talent assets in the folder and subfolders
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Talent", new[] { folderPath });
        var foundTalents = new List<Talent>();
        foreach (string guid in guids)
        {
            string assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            Talent talent = UnityEditor.AssetDatabase.LoadAssetAtPath<Talent>(assetPath);
            if (talent != null)
                foundTalents.Add(talent);
        }

        // Ensure every found talent is in the talents list
        foreach (var talent in foundTalents)
        {
            if (!talents.Contains(talent))
                talents.Add(talent);
        }

        // Optionally, remove talents that are not found in the folder anymore
        talents.RemoveAll(t => !foundTalents.Contains(t));
#endif
    }

}

