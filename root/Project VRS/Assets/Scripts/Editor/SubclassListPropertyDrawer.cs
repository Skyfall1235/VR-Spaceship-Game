using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

[CustomPropertyDrawer(typeof(SubclassListAttribute))]
public class SubclassListPropertyDrawer : PropertyDrawer
{
    const string FilePathForUIBuilderTree = "Assets/Scripts/Editor/UXML/SubclassList.uxml";
    public VisualTreeAsset UIBuilderTree;
    bool m_init = true;
    SubclassListAttribute m_attributeData;
    Dictionary<string, Type> m_derivingTypes = new Dictionary<string, Type>();
    VisualElement m_root = new VisualElement();
    Type m_selectedType = null;
    void OnEnable(SerializedProperty property)
    {
        UIBuilderTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(FilePathForUIBuilderTree);
        UIBuilderTree.CloneTree(m_root);
        m_attributeData = attribute as SubclassListAttribute;
        DropdownField dropdownMenu = m_root.Query<DropdownField>("TypeSelectionDropdown");
        VisualElement objectPropertiesContainer = m_root.Query<VisualElement>("ObjectProperties");
        dropdownMenu.choices.Clear();
        //Get all non-abstract sub-types and add them to a dictionary where they can be looked up by name
        m_derivingTypes = GetDerivingTypes(m_attributeData.Type).ToDictionary(value => value.Name, value => value);
        //Add type names to dropdown
        m_derivingTypes.ToList().ForEach(entry => dropdownMenu.choices.Add(entry.Key));
        //try to find the type and pass it to selected type when the value is changed
        dropdownMenu.RegisterValueChangedCallback(value =>
        {
            m_derivingTypes.TryGetValue(value.newValue, out m_selectedType);
            if (m_selectedType != null)
            {
                property.managedReferenceValue = Activator.CreateInstance(m_selectedType);
                objectPropertiesContainer.Clear();
                DrawUI(m_selectedType, property, objectPropertiesContainer);
                property.serializedObject.ApplyModifiedProperties();
            }
        }
        );
    }
    void OnDisable()
    {

    }
    void OnDestroy()
    {

    }
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        CheckInit(property);
        DropdownField dropdownMenu = m_root.Query<DropdownField>("TypeSelectionDropdown");
        VisualElement objectPropertiesContainer = m_root.Query<VisualElement>("ObjectProperties");
        if(property.managedReferenceValue != null)
        {
            m_selectedType = property.managedReferenceValue.GetType();
            dropdownMenu.value = dropdownMenu.choices[dropdownMenu.choices.IndexOf(m_selectedType.Name)];
            DrawUI(m_selectedType, property, objectPropertiesContainer);
        }
        return m_root;
    }

    #region Get custom property drawer for type
    /// <summary>
    /// Get all property drawers in the project
    /// </summary>
    /// <returns>All property drawers in the projects a types</returns>
    IEnumerable<System.Type> AllPropertyDrawers()
    {
        //List<System.Type> drawers = new List<Type>();
        foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (Type t in ass.GetTypes())
            {
                if (t.IsSubclassOf(typeof(PropertyDrawer)))
                {
                    yield return t;
                }
            }
        }
    }
    /// <summary>
    /// Gets a custom property drawer given a type
    /// </summary>
    /// <param name="target">The type to find the property drawer for</param>
    /// <returns>The type of the property drawer found if there is one. Otherwise null</returns>
    System.Type GetCustomPropertyDrawerFor(System.Type target)
    {
        foreach (Type drawer in AllPropertyDrawers())
        {
            foreach (Attribute attribute in Attribute.GetCustomAttributes(drawer))
            {
                if (attribute is CustomPropertyDrawer cpd && cpd.GetFieldValue<Type>("m_Type") == target)
                {
                    return drawer;
                }
            }
        }

        return null;
    }
    /// <summary>
    /// Gets a custom property drawer given a type and returns out saying whether child classes should use this drawer
    /// </summary>
    /// <param name="target">The type to find the property drawer for</param>
    /// <param name="childrenUseDrawer">whether child classes should use this drawer</param>
    /// <returns>The type of the property drawer found if there is one. Otherwise null</returns>
    System.Type GetCustomPropertyDrawerFor(System.Type target, out bool childrenUseDrawer)
    {
        foreach (Type drawer in AllPropertyDrawers())
        {
            foreach (Attribute attribute in Attribute.GetCustomAttributes(drawer))
            {
                if (attribute is CustomPropertyDrawer cpd && cpd.GetFieldValue<Type>("m_Type") == target)
                {
                    childrenUseDrawer = cpd.GetFieldValue<bool>("m_UseForChildren");
                    return drawer;
                }
            }
        }
        childrenUseDrawer = false;
        return null;
    }
    #endregion
    /// <summary>
    /// Gets all types that derive from a given type
    /// </summary>
    /// <param name="typeToFindDerivingTypesFrom">The type that all items returned should derive from</param>
    /// <returns>All types that derive from the given type</returns>
    IEnumerable<Type> GetDerivingTypes(Type typeToFindDerivingTypesFrom)
    {
        return (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
         from type in domainAssembly.GetTypes()
         where typeToFindDerivingTypesFrom.IsAssignableFrom(type) && !type.IsAbstract
         select type);
    }
    /// <summary>
    /// Tries to draw from a custom property drawer and defaults to a standard setup otherwise
    /// </summary>
    /// <param name="typeToDrawUIFor">The type of object you desire to draw a UI sor</param>
    /// <param name="property">The SerializedProperty for handling data</param>
    /// <param name="objectPropertiesContainer">The VisualElement to put the drawn properties into</param>
    void DrawUI(Type typeToDrawUIFor, SerializedProperty property, VisualElement objectPropertiesContainer)
    {
        //TODO: Implement type caching to speed this code up
        if(typeToDrawUIFor == null)
        {
            return;
        }
        Type customPropertyDrawerForType = GetCustomPropertyDrawerFor(typeToDrawUIFor);
        Type customPropertyDrawerForParent = null;
        for(Type parentType = typeToDrawUIFor;  parentType == null || customPropertyDrawerForParent == null; parentType = parentType.BaseType)
        {
            Type parentTypeDrawer = GetCustomPropertyDrawerFor(parentType, out bool childrenUseDrawer);
            if (parentTypeDrawer != null && childrenUseDrawer) 
            {
                customPropertyDrawerForParent = parentTypeDrawer;
            }
        }

        //Draw direct defined custom property drawer
        if (customPropertyDrawerForType != null)
        {
            object customDrawerInstance = Activator.CreateInstance(customPropertyDrawerForType);
            FieldInfo fieldInfo = property.GetFieldInfoAndStaticType(out Type t);
            customPropertyDrawerForType.GetField("m_FieldInfo", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(customDrawerInstance, fieldInfo);
            objectPropertiesContainer.Add((VisualElement)customPropertyDrawerForType.GetMethod("CreatePropertyGUI").Invoke(customDrawerInstance, new object[] { property }));
        }
        //Draw the parent defined property drawer
        else if (customPropertyDrawerForParent != null)
        {
            object customDrawerInstance = Activator.CreateInstance(customPropertyDrawerForParent);
            FieldInfo fieldInfo = property.GetFieldInfoAndStaticType(out Type t);
            customPropertyDrawerForParent.GetField("m_FieldInfo", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(customDrawerInstance, fieldInfo);
            objectPropertiesContainer.Add((VisualElement)customPropertyDrawerForParent.GetMethod("CreatePropertyGUI").Invoke(customDrawerInstance, new object[] { property }));
        }
        //Make one up
        else
        {
            foreach (SerializedProperty childProperty in property.GetChildren())
            {
                PropertyField fieldToAdd = new PropertyField();
                fieldToAdd.BindProperty(childProperty);
                objectPropertiesContainer.Add(fieldToAdd);
            }
        }
    }
    #region OnEnable OnDisable and OnDestroy event handling
    void CheckInit(SerializedProperty property)
    {
        if (m_init)
        {
            Enable(property);
        }
    }
    ~SubclassListPropertyDrawer()
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
        m_init = false;
        EditorApplication.playModeStateChanged += PlayModeStateChanged;
        Selection.selectionChanged += SelectionChanged;
        OnEnable(property);
    }

    public void Disable()
    {
        OnDisable();
        EditorApplication.playModeStateChanged -= PlayModeStateChanged;
        Selection.selectionChanged -= SelectionChanged;
        m_init = true;
    }

    public void Destroy()
    {
        OnDestroy();
        EditorApplication.playModeStateChanged -= PlayModeStateChanged;
        Selection.selectionChanged -= SelectionChanged;
        m_init = true;
    }
    #endregion
}