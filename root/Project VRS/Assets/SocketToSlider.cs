using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Content.Interaction;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketToSlider : MonoBehaviour
{
    public XRSlider slider;
    public XRSocketInteractor socketInteractor;

    //if the sliders valus is the max, do something?
    //if the slider is at the minvalue, remve it as the handle and give it to the socket.

    //public new IXRSelectInteractor interactorObject
    //{
    //    get => (IXRSelectInteractor)base.interactorObject;
    //    set => base.interactorObject = value;
    //}
    //public new IXRSelectInteractable interactableObject
    //{
    //    get => (IXRSelectInteractable)base.interactableObject;
    //    set => base.interactableObject = value;
    //}

    void ControlInteraction(SelectEnterEventArgs args)
    {
        if(slider.value >= 1.0f)
        {
            //handle the object to the socket
            TransitionToSocket(args);

        }
        else if(slider.value > 0.0f)
        {
            //slide should take over
            TransitionToSlider(args);
        }
        else
        {
            //do something?
        }
    }
    public void CheckIfCanDisLodge(Single Value)
    {
        if(slider.value >= 1.0f)
        {
            // TransitionToSocket()
        }
    }

    void TransitionToSocket(SelectEnterEventArgs args)
    {
        GameObject GO = args.interactableObject.transform.gameObject;
        //socketInteractor.enabled = true;
        ApplyBoolToPositionOnRigidbody(GO.GetComponent<Rigidbody>(), false);
        GO.GetComponent<XRBaseInteractable>().enabled = true;
        slider.colliders.Remove(GO.GetComponent<Collider>());
        socketInteractor.hoverSocketSnapping = true;
        GO.transform.parent = null;
    }

    public void TransitionToSlider(SelectEnterEventArgs args)
    {
        GameObject GO = args.interactableObject.transform.gameObject;
        GO.transform.parent = transform;
        GO.GetComponent<XRBaseInteractable>().enabled = false;
        //OBHECT GETS SET TO HANDLE AND WE TURN OFF THE SOCKET
        slider.m_Handle = GO.transform;
        socketInteractor.enabled = false;
        slider.colliders.Add(GO.GetComponent<Collider>());
        ApplyBoolToPositionOnRigidbody(GO.GetComponent<Rigidbody>(), true);
        
    }

    void ApplyBoolToPositionOnRigidbody(Rigidbody rb, bool value)
    {
        if(value)
        {
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            rb.useGravity = false;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;
        }
        
    }



}
