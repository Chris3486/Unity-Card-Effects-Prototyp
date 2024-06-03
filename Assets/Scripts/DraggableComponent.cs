using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DraggableComponent : MonoBehaviour
{
    public float maxRotationOffset = 20f;
    public float rotationOffsetRate = 0.1f;
    public float rotationSettleRate = 5f;

    private bool beingDragged = false;
    private Vector3 dragStartMousePos, dragStartMyPos;

    private Vector3 rotationOffset;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleDrag();

        // Slowly decay rotation offset so card settles back into position
        if (rotationOffset.y > 0f)
        {
            rotationOffset.y -= Mathf.Min(rotationOffset.y, rotationSettleRate * Time.deltaTime);
        }
        else
        {
            rotationOffset.y += Mathf.Min(Mathf.Abs(rotationOffset.y), rotationSettleRate * Time.deltaTime);
        }

        transform.localEulerAngles = rotationOffset;
    }

    void HandleDrag()
    {
        if (beingDragged)
        {
            if (Input.GetMouseButton(0))
            {
                // Continue dragging - calculate the drag offset and move ourself relative to that
                Vector3 oldPosition = transform.position;

                Vector3 dragMouseOffset = Camera.main.ScreenPointToRay(Input.mousePosition).origin - dragStartMousePos;
                transform.position = dragStartMyPos + dragMouseOffset;

                // Rotate in line with movement
                Vector3 movedAmount = transform.position - oldPosition;
                float newYRotation = Mathf.Clamp(rotationOffset.y - movedAmount.x * Time.deltaTime, -maxRotationOffset, maxRotationOffset);
                rotationOffset.y = newYRotation;

                // Slowly decay rotation offset
                rotationOffset.y *= 0.95f;
                transform.localEulerAngles = rotationOffset;
            }
            else
            {
                // MOuse was released while dragging
                beingDragged = false;
                Debug.Log("Yeah!!");
            }
        }
        else
        {
            // Potentially pick up the card
            if (!Input.GetMouseButtonDown(0)) return;



            // Raycast to see if we`ve been clicked
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (!Physics.Raycast(mouseRay, out rayHit)) return;



            // Check to see if the ray hit us and not some other objects
            if (rayHit.collider.gameObject != gameObject) return;

            beingDragged = true;
            dragStartMousePos = mouseRay.origin;
            dragStartMyPos = transform.position;

            Debug.Log("Uff!!");



        }
    }
}
