using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomGrab : MonoBehaviour
{
    CustomGrab otherHand = null;
    public List<Transform> nearObjects = new List<Transform>();
    public Transform grabbedObject = null;
    public InputActionReference action;
    bool grabbing = false;
    Vector3 lastPosition;
    Quaternion lastRotation;
    public bool doubleRotation = false;

    private void Start()
    {
        action.action.Enable();
        foreach (CustomGrab c in transform.parent.GetComponentsInChildren<CustomGrab>())
        {
            if (c != this)
                otherHand = c;
        }
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    void Update()
    {
        grabbing = action.action.IsPressed();
        Vector3 deltaPosition = transform.position - lastPosition;
        Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(lastRotation);

        if (grabbing)
        {
            if (!grabbedObject)
                grabbedObject = nearObjects.Count > 0 ? nearObjects[0] : otherHand.grabbedObject;

            if (grabbedObject)
            {
                grabbedObject.position += deltaPosition;
                if (doubleRotation)
                    grabbedObject.rotation *= transform.rotation;
                else
                    grabbedObject.rotation = deltaRotation * grabbedObject.rotation;
            }
        }
        else if (grabbedObject)
            grabbedObject = null;

        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform t = other.transform;
        if (t && t.tag.ToLower() == "grabbable")
            nearObjects.Add(t);
    }

    private void OnTriggerExit(Collider other)
    {
        Transform t = other.transform;
        if (t && t.tag.ToLower() == "grabbable")
            nearObjects.Remove(t);
    }
}
