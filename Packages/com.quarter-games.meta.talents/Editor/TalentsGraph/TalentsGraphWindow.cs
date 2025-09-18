using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class TalentsGraphWindow : GraphViewToolWindow
{
    protected override string ToolName => "Talents Graph";

    private TalentsGraphView _graphView;

    [MenuItem("QG/Talents Graph", priority = 1001)]
    public static TalentsGraphWindow OpenWindow()
    {
        var window = GetWindow<TalentsGraphWindow>();
        window.titleContent = new GUIContent("Talents Graph");
        window.Show();
        return window;
    }

    new protected void OnEnable()
    {
        base.OnEnable();
        CreateGraphView();
        PopulateGraph();
        titleContent = new GUIContent("Talents Graph");
        CreateTopToolBar();
        CreateSideMenuForTalentCreation();
    }

    private void CreateSideMenuForTalentCreation()
    {
        var listOfMetaKeys = AssetDatabase.FindAssets("t:MetaMediationKey");
        var metaKeys = new List<MetaMediationKey>();
        foreach (var key in listOfMetaKeys)
        {
            var GUID = AssetDatabase.GUIDToAssetPath(key);
            metaKeys.Add(AssetDatabase.LoadAssetAtPath<MetaMediationKey>(GUID));
        }
        var sideMenu = new VisualElement
        {
            style =
            {
                width = 200,
                flexShrink = 0,
                flexDirection = FlexDirection.Column,
                borderRightWidth = 1,
                borderRightColor = new Color(0.5f, 0.5f, 0.5f, 0.5f),
                backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.3f),
            }
        };
        var title = new Label("Create New Talent")
        {
            style =
            {
                unityFontStyleAndWeight = FontStyle.Bold,
                fontSize = 14,
                marginTop = 10,
                marginBottom = 10,
                alignSelf = Align.Center
            }
        };
        sideMenu.Add(title);
        rootVisualElement.Add(sideMenu);
        for (int i = 0; i < metaKeys.Count; i++)
        {
            var key = metaKeys[i];
            var button = new Button(() => CreateNewTalent(key))
            {
                text = key.name,
                tooltip = $"Create a new Talent with Meta Mediation Key: {key.name}",
                style =
                {
                    marginTop = 2,
                    marginBottom = 2,
                    marginLeft = 5,
                    marginRight = 5,
                    height = 30,
                    unityTextAlign = TextAnchor.MiddleCenter,
                    backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f),
                    color = Color.white,
                }
            };
            sideMenu.Add(button);
        }

    }
    public void CreateNewTalent(MetaMediationKey key)
    {
        var listOfCurrentTalents = AssetDatabase.FindAssets("t:Talent").ToList().Select(x => AssetDatabase.LoadAssetAtPath<Talent>(AssetDatabase.GUIDToAssetPath(x))).ToList();
        var talentsOfSameKey = listOfCurrentTalents.Where(x => x.MetaMediatorKey == key).ToList();
        var originalTalent = talentsOfSameKey.FirstOrDefault();
        Talent newTalent = ScriptableObject.CreateInstance<Talent>();
        Undo.RegisterCreatedObjectUndo(newTalent, "Duplicate Talent");
        var newAssetPath = "";
        if (originalTalent == null)
        {
            Debug.LogWarning("No existing Talent found to use as a template. Please fill a Talent manually first.", this);
            newAssetPath = AssetDatabase.GenerateUniqueAssetPath(EditorUtility.SaveFilePanel("Select Folder to Save New Talent", "Assets", "New Talent", "asset"));
            if (string.IsNullOrEmpty(newAssetPath))
            {
                Undo.ClearUndo(newTalent);
                return;
            }
            newAssetPath = "Assets\\" + System.IO.Path.GetRelativePath("Assets", newAssetPath);
            newTalent.MetaMediatorKey = key;
        }
        else
        {
            var originalPath = AssetDatabase.GetAssetPath(originalTalent);
            // Copy fields
            newTalent.TalentName = originalTalent.TalentName;
            newTalent.Complexity = originalTalent.Complexity;
            newTalent.Description = originalTalent.Description;
            newTalent.Value = originalTalent.Value;
            newTalent.Icon = originalTalent.Icon;
            newTalent.MetaMediatorKey = originalTalent.MetaMediatorKey;

            // Save new asset
            newAssetPath = AssetDatabase.GenerateUniqueAssetPath(originalPath.Replace(".asset", "_Copy.asset"));

            Debug.Log($"Duplicated Talent '{originalTalent.TalentName}' to '{newTalent.TalentName}' at '{newAssetPath}'", newTalent);
        }
        AssetDatabase.CreateAsset(newTalent, newAssetPath);
        EditorUtility.SetDirty(newTalent);

        var tallentLib = AssetDatabase.LoadAssetAtPath<TalentLibrary>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:TalentLibrary").First()));
        Undo.RecordObject(tallentLib, "Create Duplicate Talent");
        if (tallentLib) tallentLib.OnValidate();
        PopulateGraph();
    }

    private void CreateTopToolBar()
    {
        var toolbar = new Toolbar();
        var refreshButton = new Button(() => PopulateGraph())
        {
            text = "Refresh",
            tooltip = "Refresh the graph to reflect any changes in Talent assets."
        };
        toolbar.Add(refreshButton);
        rootVisualElement.RemoveAt(0); // Remove existing toolbar if any
        rootVisualElement.Add(toolbar);
    }

    private void CreateGraphView()
    {
        Undo.undoRedoPerformed += PopulateGraph;
        _graphView = new TalentsGraphView
        {
            name = "Talents Graph View"
        };
        _graphView.RedrawCallback += PopulateGraph;
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }

    private void PopulateGraph()
    {
        _graphView.ClearGraph();

        // Find all Talent assets in the project
        string[] guids = AssetDatabase.FindAssets("t:Talent");
        var talents = new List<Talent>();
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var talent = AssetDatabase.LoadAssetAtPath<Talent>(path);
            if (talent != null)
                talents.Add(talent);
        }

        // Create nodes
        var nodeMap = new Dictionary<Talent, TalentNode>();
        foreach (var talent in talents)
        {
            var node = _graphView.CreateTalentNode(talent);
            nodeMap[talent] = node;
        }

        // Create edges for dependencies
        foreach (var talent in talents)
        {
            if (talent.DependsOn == null || talent.DependsOn.Count == 0) continue;
            foreach (var dep in talent.DependsOn)
            {
                if (dep == null) continue;
                if (nodeMap.TryGetValue(dep, out var depNode) && nodeMap.TryGetValue(talent, out var talentNode))
                {
                    _graphView.CreateDependencyEdge(depNode, talentNode);
                }
            }
        }

        // --- Straight left-to-right layout using CalculateIndex() ---
        float nodeWidth = 400f;
        float nodeHeight = 250f;
        float horizontalSpacing = 40f;
        float verticalSpacing = 20f;
        float startY = 0f;
        float startX = 0f;

        // Order talents by their CalculateIndex()
        var orderedTalents = new List<Talent>(talents);
        orderedTalents.Sort((a, b) => a.CalculateIndex().CompareTo(b.CalculateIndex()));

        var positions = new Dictionary<Talent, Vector2>();
        foreach (var talent in orderedTalents)
        {
            int index = talent.CalculateIndex();
            int horIndex = CalculateHorizontalDepSibling(talent, talents);
            float x = startX + horIndex * (horizontalSpacing + nodeWidth);
            float y = startY - index * (verticalSpacing + nodeHeight);
            positions[talent] = new Vector2(x, y);
        }

        foreach (var kvp in positions)
        {
            if (nodeMap.TryGetValue(kvp.Key, out var node))
            {
                node.SetPosition(new Rect(kvp.Value, new Vector2(nodeWidth, nodeHeight)));
            }
        }
    }

    private int CalculateHorizontalDepSibling(Talent talent, List<Talent> talents)
    {
        int index = 1;

        for (int i = 0; i < talents.Count; i++)
        {
            if (talents[i] == talent) break;
            if (talent.CalculateIndex() == talents[i].CalculateIndex()) index++;
        }
        return index;

    }

    protected override void OnGraphViewChanging() { }

    protected override void OnGraphViewChanged()
    {
        // Save dependency changes back to assets if you want live editing
        // (Implementation similar to UpgradesGraphWindow, but for List<Talent>)
    }
}