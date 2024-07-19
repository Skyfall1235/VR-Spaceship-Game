using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

[CustomPropertyDrawer(typeof(BC_Item))]
public class BC_ItemPropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        VisualElement root = new VisualElement();
        foreach (SerializedProperty childProperty in property.GetChildren())
        {
            PropertyField fieldToAdd = new PropertyField();
            fieldToAdd.BindProperty(childProperty);
            root.Add(fieldToAdd);
        }
        return root;
    }
}
