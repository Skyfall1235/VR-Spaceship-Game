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
    string filePathForUIBuilderTree = "Assets/Scripts/Editor/UXML/SubclassList.uxml";
    public VisualTreeAsset UIBuilderTree;
    SubclassListAttribute attributeData;
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement root = new VisualElement();
        UIBuilderTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(filePathForUIBuilderTree);
        UIBuilderTree.CloneTree(root);
        DropdownField dropdownMenu = root.Query<DropdownField>("TypeSelectionDropdown");
        attributeData = attribute as SubclassListAttribute;
        dropdownMenu.choices.Clear();
        VisualElement objectPropertiesContainer = root.Query<VisualElement>("ObjectProperties");

        Type selectedType = null;
        Dictionary<string, Type> derivingTypes = new Dictionary<string, Type>();

        //Get all non-abstract sub-types and add them to a dictionary where they can be looked up by name
        derivingTypes = GetDerivingTypes(attributeData.Type).ToDictionary(value => value.Name, value => value);
        //Add type names to dropdown
        derivingTypes.ToList().ForEach(entry => dropdownMenu.choices.Add(entry.Key));


        if(property.managedReferenceValue != null)
        {
            selectedType = property.managedReferenceValue.GetType();
            dropdownMenu.value = dropdownMenu.choices[dropdownMenu.choices.IndexOf(selectedType.Name)];
            DrawUI(selectedType, property, objectPropertiesContainer);
        }

        //try to find the type and pass it to selected type when the value is changed
        dropdownMenu.RegisterValueChangedCallback(value => 
            {
                derivingTypes.TryGetValue(value.newValue, out selectedType);
                if (selectedType != null)
                {
                    property.managedReferenceValue = Activator.CreateInstance(selectedType);
                    objectPropertiesContainer.Clear();
                    DrawUI(selectedType, property, objectPropertiesContainer);
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        );
        return root;
    }
    /// <summary>
    /// Get all property drawers in the project
    /// </summary>
    /// <returns>All property drawers in the projects a types</returns>
    IEnumerable<System.Type> AllPropertyDrawers()
    {
        List<System.Type> drawers = new List<Type>();
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
        Type customPropertyDrawerForType = GetCustomPropertyDrawerFor(typeToDrawUIFor);
        if(customPropertyDrawerForType != null)
        {
            object customDrawerInstance = Activator.CreateInstance(customPropertyDrawerForType);
            FieldInfo fieldInfo = property.GetFieldInfoAndStaticType(out Type t);
            customPropertyDrawerForType.GetField("m_FieldInfo", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(customDrawerInstance, fieldInfo);
            objectPropertiesContainer.Add((VisualElement)customPropertyDrawerForType.GetMethod("CreatePropertyGUI").Invoke(customDrawerInstance, new object[] { property }));
        }
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
}
