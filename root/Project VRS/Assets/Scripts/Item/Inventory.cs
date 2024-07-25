using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Accessors
    public InventorySlot this[int i]
    {
        get { return m_inventorySlots[i]; }
    }
    #nullable enable
    public List<InventorySlot>? GetSlotsWithItem(BC_Item item)
    {
        List<InventorySlot> result = m_inventorySlots.Select(current => current)
            .Where(slot => slot.item == item).ToList();
        if(result.Count > 0)
        {
            return result;
        }
        else
        {
            return null;
        }
    } 
    public List<uint>? GetIndiciesOfSlotsWithItem(BC_Item item)
    {
        List<uint> result = m_inventorySlots.Select((current, i) => new { current, i })
            .Where(current => current.current.item == item)
            .Select(current => (uint)current.i).ToList();
        if (result.Count > 0)
        {
            return result;
        }
        else
        {
            return null;
        }
    }
    public List<InventorySlot>? GetSlotsWithItemWithName(string name)
    {
        List<InventorySlot> result = m_inventorySlots.Select(current => current)
            .Where(slot => slot.item.Name == name).ToList();
        if (result.Count > 0)
        {
            return result;
        }
        else
        {
            return null;
        }
    }    
    public List<uint>? GetIndiciesOfSlotsWithItemWithName(string name)
    {
        List<uint> result = m_inventorySlots.Select((current, i) => new { current, i })
            .Where(current => current.current.item.Name == name)
            .Select(current => (uint)current.i).ToList();
        if (result.Count > 0)
        {
            return result;
        }
        else
        {
            return null;
        }
    }
    #nullable disable
    #endregion
    [SerializeField] uint m_inventorySize;
    [SerializeField] List<InventorySlot> m_inventorySlots = new List<InventorySlot>();
    #region Monobehavior Methods
    private void Awake()
    {
        OnValidate();
    }
    private void OnValidate()
    {
        m_inventorySlots.Resize((int)m_inventorySize);
    }
    #endregion
    #region Data Editing Methods
    /// <summary>
    /// Adds a selected item to the inventory
    /// </summary>
    /// <param name="itemToAdd">The item to add to the inventory</param>
    /// <param name="amountToAdd">The amount of the item to add</param>
    /// <returns>The amount of items that failed to be added to the inventory</returns>
    public uint TryAddToInventory(BC_Item itemToAdd, uint amountToAdd = 1)
    {
        //look for slots to fill first
        for(int i = 0; i < m_inventorySlots.Count && amountToAdd > 0; i++)
        {
            if(itemToAdd == m_inventorySlots[i].item && m_inventorySlots.Count < itemToAdd.MaxStackSize)
            {
                if (m_inventorySlots[i].itemCount + amountToAdd <= itemToAdd.MaxStackSize)
                {
                    m_inventorySlots[i].itemCount += amountToAdd;
                    amountToAdd = 0;
                }
                else
                {
                    m_inventorySlots[i].itemCount = itemToAdd.MaxStackSize;
                    amountToAdd -= itemToAdd.MaxStackSize - m_inventorySlots[i].itemCount;
                }
            }
        }
        //look for empty slots to fill
        for (int i = 0; i < m_inventorySlots.Count && amountToAdd > 0; i++)
        {
            if(amountToAdd > 0 && m_inventorySlots[i].item == null)
            {
                m_inventorySlots[i].item = itemToAdd;
                if(amountToAdd > itemToAdd.MaxStackSize)
                {
                    m_inventorySlots[i].itemCount = itemToAdd.MaxStackSize;
                    amountToAdd -= itemToAdd.MaxStackSize;
                }
                else
                {
                    m_inventorySlots[i].itemCount = amountToAdd;
                    amountToAdd = 0;
                }
            }
        }
        return amountToAdd;
    }
    public uint TryRemoveFromInventory(BC_Item item, uint amountToRemove = 1)
    {
        for(int i = 0; i < m_inventorySlots.Count && amountToRemove > 0; i++)
        {
            if (m_inventorySlots[i].item == item)
            {
                if(amountToRemove > m_inventorySlots[i].itemCount)
                {
                    amountToRemove -= m_inventorySlots[i].itemCount;
                    m_inventorySlots[i].itemCount = 0;
                }
                else
                {
                    m_inventorySlots[i].itemCount = 0;
                    amountToRemove = 0;
                }
            }
        }
        return amountToRemove;
    }
    #endregion
}
[Serializable]
public class InventorySlot 
{
    [SerializeField] public BC_Item item = null;
    [SerializeField] public uint itemCount;
}