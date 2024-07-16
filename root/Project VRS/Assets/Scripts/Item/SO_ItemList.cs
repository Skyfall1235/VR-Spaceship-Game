using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Project VRS/New Item List")]
public class SO_ItemList : ScriptableObject
{
    [SubclassList(typeof(BC_Item))] [SerializeReference] public List<BC_Item> ItemList = new List<BC_Item>();
}