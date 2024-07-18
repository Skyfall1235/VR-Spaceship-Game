using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class BC_Item
{
    [SerializeField] Mesh m_modelToDisplay;
    public Mesh ModelToDisplay { get => m_modelToDisplay; private set => m_modelToDisplay = value; }

    [SerializeField] Material m_materialToDisplay;
    public Material MaterialToDisplay { get => m_materialToDisplay; private set => m_materialToDisplay = value; }

    [SerializeField] Sprite m_spriteForInventory;
    public Sprite SpriteForInventory { get => m_spriteForInventory; private set => m_spriteForInventory = value; }

    [SerializeField] uint m_maxStackSize;
    public uint MaxStackSize { get => m_maxStackSize; private set => m_maxStackSize = value; }

    [SerializeField] string m_name;
    public string Name { get => m_name; private set => m_name = value; }

    [SerializeField] string m_description;
    public string Description { get => m_description; private set => m_description = value; }
}