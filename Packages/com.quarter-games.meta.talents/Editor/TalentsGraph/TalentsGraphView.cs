using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class TalentsGraphView : GraphView
{
    public Action RedrawCallback;
    public TalentsGraphView()
    {
        style.flexGrow = 1;
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        this.graphViewChanged += OnGraphViewChanged;
    }


    public void ClearGraph()
    {
        graphElements.ForEach(RemoveElement);
    }
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        base.BuildContextualMenu(evt);
        switch (evt.target)
        {
            case Node node:
                evt.menu.ClearItems();
                evt.menu.AppendAction("Duplicate Talent Node", DuplicateTalent());
                break;
        }
    }

    private Action<DropdownMenuAction> DuplicateTalent()
    {
        return actionEvent =>
        {
            Undo.IncrementCurrentGroup();
            var selectionCopy = selection.ToList().OfType<TalentNode>().Where(x => x.Talent).OrderBy(x => x.Talent.CalculateIndex()).ToList();
            Dictionary<Talent, Talent> OriginalAndCopy = new();
            for (int i = 0; i < selectionCopy.Count; i++)
            {
                TalentNode node = selectionCopy[i];
                // Duplicate the Talent ScriptableObject
                var originalTalent = node.Talent;
                var assetPath = AssetDatabase.GetAssetPath(originalTalent);
                var newTalent = ScriptableObject.CreateInstance<Talent>();

                Undo.RegisterCreatedObjectUndo(newTalent, "Duplicate Talent");

                OriginalAndCopy.Add(originalTalent, newTalent);
                // Copy fields
                newTalent.TalentName = originalTalent.TalentName;
                newTalent.Complexity = originalTalent.Complexity;
                newTalent.Description = originalTalent.Description;
                newTalent.Value = originalTalent.Value;
                newTalent.Icon = originalTalent.Icon;
                newTalent.MetaMediatorKey = originalTalent.MetaMediatorKey;
                if (originalTalent.DependsOn != null)
                {
                    foreach (var dependency in originalTalent.DependsOn)
                    {
                        newTalent.DependsOn ??= new List<Talent>();
                        if (OriginalAndCopy.TryGetValue(dependency, out var newTalentCopy))
                        {
                            newTalent.DependsOn.Add(newTalentCopy);
                        }
                        else
                        {
                            newTalent.DependsOn.Add(dependency);
                        }
                    }
                }

                // Save new asset
                var newAssetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath.Replace(".asset", "_Copy.asset"));
                AssetDatabase.CreateAsset(newTalent, newAssetPath);
                EditorUtility.SetDirty(newTalent);
                var tallentLib = AssetDatabase.LoadAssetAtPath<TalentLibrary>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:TalentLibrary").First()));
                Undo.RecordObject(tallentLib, "Create Duplicate Talent");
                tallentLib.OnValidate();

                // Create new node in graph
                var newNode = CreateTalentNode(newTalent);
                // Place new node near the original
                var originalPos = node.GetPosition();
                newNode.SetPosition(new Rect(originalPos.x + 40, originalPos.y + 40, originalPos.width, originalPos.height));
                newNode.Select(this, false);
                Debug.Log($"Duplicated Talent '{originalTalent.TalentName}' to '{newTalent.TalentName}' at '{newAssetPath}'", newTalent);

            }
            AssetDatabase.SaveAssets();
            RedrawCallback?.Invoke();
        };
    }

    public TalentNode CreateTalentNode(Talent talent)
    {
        var node = new TalentNode(talent);
        AddElement(node);
        return node;
    }

    public void CreateDependencyEdge(TalentNode from, TalentNode to)
    {
        var edge = new Edge
        {
            output = to.InputPort,
            input = from.OutputPort,
        };

        edge.output.Connect(edge);
        edge.input.Connect(edge);
        AddElement(edge);
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange change)
    {
        if (change.edgesToCreate != null)
        {
            Undo.IncrementCurrentGroup();
            foreach (var edge in change.edgesToCreate)
            {

                var fromNode = edge.input.node as TalentNode;
                var toNode = edge.output.node as TalentNode;
                if (fromNode != null && toNode != null)
                {
                    Undo.RecordObject(toNode.Talent, "Add Talent Dependency");
                    // Save dependency: toNode depends on fromNode
                    if (toNode.Talent.DependsOn == null) toNode.Talent.DependsOn = new List<Talent>();
                    if (!toNode.Talent.DependsOn.Contains(fromNode.Talent)) toNode.Talent.DependsOn.Add(fromNode.Talent);
                    else
                    {
                        edge.input.Disconnect(edge);
                        edge.output.Disconnect(edge);
                        RemoveElement(edge);

                    }
                    // Mark the ScriptableObject as dirty so Unity knows to save the change
                    EditorUtility.SetDirty(toNode.Talent);
                    AssetDatabase.SaveAssets();
                }
            }
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            RedrawCallback?.Invoke();
        }

        if (change.elementsToRemove != null)
        {
            Undo.IncrementCurrentGroup();
            foreach (var element in change.elementsToRemove)
            {
                if (element is Edge edge)
                {
                    RemoveEdge(edge);
                }
                if (element is TalentNode node)
                {
                    RemoveNode(node);
                }

            }
            Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
            RedrawCallback?.Invoke();
        }

        return change;
    }

    private void RemoveNode(TalentNode node)
    {
        // Remove all edges connected to this node
        Undo.RecordObject(node.Talent, "Remove Talent Node");
        var connectedEdges = edges.ToList().Where(e => e.input.node == node || e.output.node == node).ToList();
        foreach (var edge in connectedEdges)
        {
            RemoveEdge(edge);
            RemoveElement(edge);
        }
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(node.Talent));

        var tallentLib = AssetDatabase.LoadAssetAtPath<TalentLibrary>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:TalentLibrary").First()));
        Undo.RecordObject(tallentLib, "Create Duplicate Talent");
        tallentLib.OnValidate();
    }

    private static void RemoveEdge(Edge edge)
    {
        if (edge.input == null || edge.input.node == null || edge.output == null || edge.output.node == null) return;
        var fromNode = edge.input.node as TalentNode;
        var toNode = edge.output.node as TalentNode;
        Undo.RecordObject(toNode.Talent, "Remove Talent Dependency");

        if (fromNode != null && toNode != null && toNode.Talent.DependsOn != null)
        {
            if (toNode.Talent.DependsOn.Contains(fromNode.Talent))
            {
                toNode.Talent.DependsOn.Remove(fromNode.Talent);
                EditorUtility.SetDirty(toNode.Talent);
                AssetDatabase.SaveAssets();
            }
        }
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> filtered = new();
        if (startPort.direction == Direction.Output)
        {
            filtered = ports.Where(p => p.direction == Direction.Input && p.portType == startPort.portType && startPort.node != p.node).ToList();
            filtered = filtered.Where(p => !(startPort.node as TalentNode).Talent.DependsOn.Contains((p.node as TalentNode).Talent)).ToList();
            filtered = filtered.Where(p => (startPort.node as TalentNode).Talent.CalculateIndex() == 0 || (p.node as TalentNode).Talent.CalculateIndex() <= (startPort.node as TalentNode).Talent.CalculateIndex()).ToList();
        }
        else if (startPort.direction == Direction.Input)
        {
            filtered = ports.Where(p => p.direction == Direction.Output && p.portType == startPort.portType && startPort.node != p.node).ToList();
            filtered = filtered.Where(p => !(p.node as TalentNode).Talent.DependsOn.Contains((startPort.node as TalentNode).Talent)).ToList();
            filtered = filtered.Where(p => (p.node as TalentNode).Talent.CalculateIndex() == 0 || (p.node as TalentNode).Talent.CalculateIndex() >= (startPort.node as TalentNode).Talent.CalculateIndex()).ToList();
        }
        return filtered;
    }
}