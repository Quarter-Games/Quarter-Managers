using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class TalentNode : Node
{
    public Port InputPort { get; private set; }
    public Port OutputPort { get; private set; }
    public Talent Talent { get; private set; }

    public TalentNode(Talent talent)
    {
        expanded = false;
        Talent = talent;
        title = talent.TalentName;
        
        titleContainer.style.unityTextAlign = TextAnchor.MiddleCenter;
        Color nodeColor = talent.Complexity == TalentComplexity.Simple ? Color.white : new Color(0.7f, 0.7f, 1f);

        InputPort = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(Talent));
        InputPort.portName = "Depends On";
        InputPort.SetEnabled(true);

        OutputPort = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(Talent));
        OutputPort.portName = "Unlocks";
        OutputPort.style.color = Color.black;
        OutputPort.SetEnabled(true);
        titleContainer.Add(OutputPort);
        topContainer.style.flexDirection = FlexDirection.Column;
        topContainer.style.alignItems = Align.Center;
        topContainer.style.justifyContent = Justify.Center;


        // Add editable fields for Talent properties below the icon
        var fieldsContainer = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Column,
                marginTop = 4,
                marginBottom = 4,
                alignItems = Align.FlexStart,
                backgroundColor = nodeColor * 0.1f,
            }
        };
        // Draw talent icon
        if (talent.Icon != null)
        {
            Texture2D iconTexture = talent.Icon.texture;
            if (iconTexture == null)
                iconTexture = AssetPreview.GetMiniThumbnail(talent.Icon);

            if (iconTexture != null)
            {
                var rect = talent.Icon.rect;
                rect.y = iconTexture.height - rect.y - rect.height; // Invert Y axis for Unity's texture coordinates
                var iconImage = new Image
                {
                    image = iconTexture,
                    sourceRect = rect,
                    scaleMode = ScaleMode.ScaleToFit,
                    style =
                    {
                        width = 64,
                        height = 64,
                        marginBottom = 4,
                        marginTop = 4,
                        alignSelf = Align.Center,
                    }
                };
                topContainer.Add(iconImage);
            }
        }

        // TalentName
        var nameField = new TextField("Name")
        {
            value = talent.TalentName,
            multiline = false
        };
        nameField.RegisterValueChangedCallback(evt => talent.TalentName = evt.newValue);
        fieldsContainer.Add(nameField);

        // Description
        var descField = new TextField("Description")
        {
            value = talent.Description,
            multiline = true
        };
        descField.RegisterValueChangedCallback(evt => talent.Description = evt.newValue);
        fieldsContainer.Add(descField);

        // Value
        var valueField = new FloatField("Value")
        {
            value = talent.Value
        };
        valueField.RegisterValueChangedCallback(evt => talent.Value = evt.newValue);
        fieldsContainer.Add(valueField);

        // Complexity
        var complexityField = new EnumField("Complexity", talent.Complexity);
        complexityField.RegisterValueChangedCallback(evt => talent.Complexity = (TalentComplexity)evt.newValue);
        fieldsContainer.Add(complexityField);


        // MetaMediatorKey
        var metaKeyField = new ObjectField("Meta Mediation Key")
        {
            objectType = typeof(MetaMediationKey),
            value = talent.MetaMediatorKey
        };
        metaKeyField.RegisterValueChangedCallback(evt => talent.MetaMediatorKey = (MetaMediationKey)evt.newValue);
        metaKeyField.style.width = StyleKeyword.Auto;
        metaKeyField.style.flexGrow = 1;
        fieldsContainer.Add(metaKeyField);

        extensionContainer.Add(fieldsContainer);


        // Set node color based on complexity
        topContainer.style.backgroundColor = new StyleColor(nodeColor*0.5f);
        RefreshExpandedState();
        RefreshPorts();
        var _inputContainer = new VisualElement
        {
            style =
            {
                flexDirection = FlexDirection.Column,
                marginTop = 4,
                marginBottom = 4,
                alignItems = Align.Center,
                backgroundColor = nodeColor * 0.1f,
            }
        };
        _inputContainer.Add(InputPort);
        contentContainer.Add(_inputContainer);
    }
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
    }
    public override void Select(VisualElement selectionContainer, bool additive)
    {
        base.Select(selectionContainer, additive);
        Selection.activeObject = Talent;
    }
}