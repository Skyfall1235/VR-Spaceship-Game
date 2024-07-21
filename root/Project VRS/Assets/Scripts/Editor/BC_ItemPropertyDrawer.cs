using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

[CustomPropertyDrawer(typeof(BC_Item))]
public class BC_ItemPropertyDrawer : PropertyDrawer
{
    const string filePathForBC_ItemDrawerTree = "Assets/Scripts/Editor/UXML/ItemCustomInspector.uxml";
    bool init = true;
    public PreviewRenderUtility m_previewRenderUtility;
    VisualTreeAsset m_itemCustomInspectorTree;
    public virtual void OnEnable(SerializedProperty property)
    {
        if (m_previewRenderUtility != null)
            m_previewRenderUtility.Cleanup();
        m_previewRenderUtility = new PreviewRenderUtility(true);
        System.GC.SuppressFinalize(m_previewRenderUtility);
        m_itemCustomInspectorTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(filePathForBC_ItemDrawerTree);
        var camera = m_previewRenderUtility.camera;
        camera.fieldOfView = 30f;
        camera.nearClipPlane = 0.01f;
        camera.farClipPlane = 1000;
        camera.transform.position = new Vector3(2.5f, 2.5f, -2.5f);
        camera.transform.LookAt(Vector3.zero);
    }


    public virtual void OnDisable()
    {
        if (m_previewRenderUtility != null)
        {
            m_previewRenderUtility.Cleanup();
            m_previewRenderUtility = null;
        }
    }
    public virtual void OnDestroy()
    {
        if (m_previewRenderUtility != null)
        {
            m_previewRenderUtility.Cleanup();
            m_previewRenderUtility = null;
        }
    }
    
    void CreateModelDisplayForInspector(VisualElement root, SerializedProperty property)
    {
        VisualElement modelPreviewContainer = root.Query<VisualElement>("ModelPreviewContainer");
        Image image = root.Query<Image>("ModelPreviewImage");
        if (image == null)
        {
            image = new Image();
            modelPreviewContainer.Add(image);
            image.name = "ModelPreviewImage";
        }
        Mesh meshToDisplay = property.FindPropertyRelative("m_modelToDisplay").objectReferenceValue as Mesh;
        Material materialToDisplay = property.FindPropertyRelative("m_materialToDisplay").objectReferenceValue as Material;
        if (meshToDisplay == null)
        {
            image.image = null;
        }
        else
        {
            float largestBound = Mathf.Max(meshToDisplay.bounds.size.x, meshToDisplay.bounds.size.y, meshToDisplay.bounds.size.z);
            Vector3 scale = new Vector3(1 / largestBound, 1/ largestBound, 1/largestBound);
            Quaternion rotation = Quaternion.identity;
            Vector3 position =  new Vector3(0 - (meshToDisplay.bounds.center.x * scale.x), 0 - (meshToDisplay.bounds.center.y * scale.y), 0 - (meshToDisplay.bounds.center.z * scale.z));
            m_previewRenderUtility.camera.transform.LookAt(position);
            Matrix4x4 meshTransformations = Matrix4x4.TRS(position, rotation, scale);
            m_previewRenderUtility.BeginPreview(new Rect(0, 0, 512, 512), GUIStyle.none);
            m_previewRenderUtility.lights[0].transform.localEulerAngles = new Vector3(30, 30, 0);
            m_previewRenderUtility.lights[0].intensity = 2;
            m_previewRenderUtility.DrawMesh(meshToDisplay, meshTransformations, materialToDisplay, 0);
            m_previewRenderUtility.camera.Render();
            image.image = m_previewRenderUtility.EndPreview();
        }
        image.style.height = image.resolvedStyle.width;
    }
    void CreateDisplaySprite(VisualElement root, SerializedProperty property)
    {
        VisualElement spriteContainer = root.Query<VisualElement>("SpritePreviewContainer");
        Image image = spriteContainer.Query<Image>("SpritePreview");
        if(image == null)
        {
            image = new Image();
            image.name = "SpritePreview";
            image.style.marginBottom = 10;
            image.style.marginTop = 10;
            image.style.marginLeft = 10;
            image.style.marginRight = 10;
            spriteContainer.Add(image);
        }
        Sprite spriteToDisplay = property.FindPropertyRelative("m_spriteForInventory").objectReferenceValue as Sprite;
        if(spriteToDisplay != null)
        {
            image.sprite = spriteToDisplay;
        }
        else
        {
            image.sprite = null;
        }
        image.style.height = image.resolvedStyle.width;
    }
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        CheckInit(property);
        VisualElement root = new VisualElement();
        m_itemCustomInspectorTree.CloneTree(root);
        VisualElement propertyHolder = root.Query<VisualElement>("Properties");
        VisualElement modelPreviewContainer = root.Query<VisualElement>("ModelPreviewContainer");
        foreach (SerializedProperty childProperty in property.GetChildren())
        {
            PropertyField fieldToAdd = new PropertyField();
            fieldToAdd.BindProperty(childProperty);
            if (childProperty.name == "m_modelToDisplay")
            {
                modelPreviewContainer.Add(fieldToAdd);
                
                fieldToAdd.RegisterValueChangeCallback(value =>
                {
                    CreateModelDisplayForInspector(root, property);
                });

            }
            else if (childProperty.name == "m_materialToDisplay")
            {
                modelPreviewContainer.Add(fieldToAdd);
                fieldToAdd.RegisterValueChangeCallback(value =>
                {
                    CreateModelDisplayForInspector(root, property);
                });
            }
            else if (childProperty.name == "m_spriteForInventory")
            {
                VisualElement spritePreviewContainer = root.Query<VisualElement>("SpritePreviewContainer");
                spritePreviewContainer.Add(fieldToAdd);
                fieldToAdd.RegisterValueChangeCallback(value => 
                {
                    CreateDisplaySprite(root, property);
                });
            }
            else
            {
                propertyHolder.Add(fieldToAdd);
            }
        }
        CreateDisplaySprite(root, property);
        CreateModelDisplayForInspector(root, property);
        return root;

    }

    #region OnEnable OnDisable and OnDestroy event handling
    void CheckInit(SerializedProperty property)
    {
        if (init)
        {
            Enable(property);
        }
    }
    ~BC_ItemPropertyDrawer()
    {
        Destroy();
    }

    private void PlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.ExitingEditMode:
            case PlayModeStateChange.ExitingPlayMode:
                Destroy();
                break;
        }
    }

    private void SelectionChanged()
    {
        Disable();
    }
    public void Enable(SerializedProperty property)
    {
        init = false;
        EditorApplication.playModeStateChanged += PlayModeStateChanged;
        Selection.selectionChanged += SelectionChanged;
        OnEnable(property);
    }

    public void Disable()
    {
        OnDisable();
        EditorApplication.playModeStateChanged -= PlayModeStateChanged;
        Selection.selectionChanged -= SelectionChanged;
        init = true;
    }

    public void Destroy()
    {
        OnDestroy();
        EditorApplication.playModeStateChanged -= PlayModeStateChanged;
        Selection.selectionChanged -= SelectionChanged;
        init = true;
    }
    #endregion
}