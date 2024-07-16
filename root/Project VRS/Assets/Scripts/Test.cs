using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SubclassList(typeof(BC_Item))] [SerializeReference] public List<BC_Item> test;
}
