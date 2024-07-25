using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class BC_Item
{
    [Flags]
    public enum ItemFlags
    {
        Nothing = 0,
        Ammo = 1 << 0,
        Module = 1 << 1,
        Weapon = 1 << 2,
        Material = 1 << 3,
        Everything = (1 << 4) - 1,
    }

    [SerializeField] Mesh m_modelToDisplay;
    public Mesh ModelToDisplay { get => m_modelToDisplay; private set => m_modelToDisplay = value; }

    [SerializeField] Material m_materialToDisplay;
    public Material MaterialToDisplay { get => m_materialToDisplay; private set => m_materialToDisplay = value; }

    [SerializeField] Sprite m_spriteForInventory;
    public Sprite SpriteForInventory { get => m_spriteForInventory; private set => m_spriteForInventory = value; }

    [SerializeField] ItemFlags m_itemType;
    public ItemFlags ItemType { get => m_itemType; }

    [SerializeField] uint m_maxStackSize;
    public uint MaxStackSize { get => m_maxStackSize; private set => m_maxStackSize = value; }

    [SerializeField] float m_mass;
    public float Mass { get => m_mass; }

    [SerializeField] string m_name;
    public string Name { get => m_name; private set => m_name = value; }

    [SerializeField] string m_description;
    public string Description { get => m_description; private set => m_description = value; }
}