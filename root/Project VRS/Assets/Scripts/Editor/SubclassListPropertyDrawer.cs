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
            DrawUI(property, objectPropertiesContainer);
        }

        //try to find the type and pass it to selected type when the value is changed
        dropdownMenu.RegisterValueChangedCallback(value => 
            {
                derivingTypes.TryGetValue(value.newValue, out selectedType);
                if (selectedType != null)
                {
                    property.managedReferenceValue = Activator.CreateInstance(selectedType);
                    objectPropertiesContainer.Clear();
                    DrawUI(property, objectPropertiesContainer);
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        );
        return root;
    }

    IEnumerable<Type> GetDerivingTypes(Type typeToFindDerivingTypesFrom)
    {
        return (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
         from type in domainAssembly.GetTypes()
         where typeToFindDerivingTypesFrom.IsAssignableFrom(type) && !type.IsAbstract
         select type);
    }
    void DrawUI(SerializedProperty property, VisualElement objectPropertiesContainer)
    {
        foreach(SerializedProperty childProperty in property.GetChildren())
        {
            PropertyField fieldToAdd = new PropertyField();
            fieldToAdd.BindProperty(childProperty);
            objectPropertiesContainer.Add(fieldToAdd);
        }
    }
}
