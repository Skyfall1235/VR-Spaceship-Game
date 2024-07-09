using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUDReciever : MonoBehaviour
{
    
    
}
public class HUDPanel
{
    private GameObject m_panelParent;
    public GameObject PanelParent => m_panelParent;
}
public class WeaponHUDPanel : HUDPanel
{
    //the current selected gun, if none is available, show the default - "NO ARM"

    //the throttle and braking sliders

    //bounding box for vecto2 for input
    //the icons for the input

}

public class ScanningHUD : HUDPanel
{

}
