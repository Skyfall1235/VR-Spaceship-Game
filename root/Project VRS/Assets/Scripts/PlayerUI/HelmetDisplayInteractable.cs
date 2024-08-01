using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// this class handles the interactable section, as well as the event driven behavior to and from it. it interfaces with the <see cref="PlayerHUDReciever"/> to ensure huds are displayed accurately
/// </summary>
public class HelmetDisplayInteractable : BC_InteractionRegister
{
    //list of all panels that can be displayed, depending on what station we are at
    //have 1 active view at a time

    [SerializeField]
    private Transform CenteredEyeLocation; //this will be needed for tracking when a player looks at an object
    private float viewDistance = 100f;
    public float ViewDistance { get => viewDistance; }

    private List<IHudView> m_hudViews = new List<IHudView>();


    private void OnValidate()
    {
        foreach(var hudView in m_hudViews)
        {
            if ((HelmetView)hudView is HelmetView)
            {
                
            }
        }
    }
}

public interface IHudView
{
    public string ViewName { get; }
    public GameObject Container { get; }

    public abstract void BeginView();

    public void OnUpdate();

    public abstract void EndView();
}

//we do this design structure because the text design is not going to be handled here.
//this is supposed to handle displaying, not what is displayed
[Serializable]
public class HelmetView : IHudView
{
    [SerializeField]
    string m_viewName;
    [SerializeField]
    GameObject m_container;

    public HelmetDisplayInteractable m_interactable;


    public string ViewName { get => m_viewName; }
    public GameObject Container { get => m_container; }

    public virtual void BeginView() { Container.SetActive(true); }

    public virtual void EndView() { }

    public virtual void OnUpdate() { Container.SetActive(false); }
}

public class BasicView : HelmetView
{ 
    public GameObject[] GetChildrenOfContainer()
    {
        GameObject[] children;
        children = GetChildrenOfType<GameObject>();
        return children;
    }
    public void SetTextOnElement(TextMeshProUGUI element, string plainText)
    {
        element.text = plainText;
    }

    protected T[] GetChildrenOfType<T>()
    {
        return Container.GetComponentsInChildren<T>();
    }
}

public class ScanningView : HelmetView
{
    //tracking for any object and implementation for returning it

    internal GameObject FireRaycastAtLookDirection(Transform startingLocation)
    {
        //shoot ray with a set distance looking for certain



        RaycastHit hit;
        Ray ray = new Ray(); 

        int layer_mask = LayerMask.GetMask("Floor");
        if (Physics.Raycast(ray, out hit, m_interactable.ViewDistance, layer_mask))
        {

        }
        return null;
    }

    //store currently locked item

    //ability to engage locked item

    //ability to disengage locked item



}

public class CombatView : ScanningView
{


    //the current selected gun, if none is available, show the default - "NO ARM"

    //the throttle and braking sliders

    //bounding box for vecto2 for input
    //the icons for the input
}

public class MaterialScanningView : HelmetView
{
    //basic combat view has the whole tracking thing built into it
}
