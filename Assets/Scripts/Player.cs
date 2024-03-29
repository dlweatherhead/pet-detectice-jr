﻿using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PickupScript))]
public class Player : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private PickupScript pickupScript;
    private DialogueScript dialogueScript;

    public Animator animator;

    public float interactionRadius = 2.5f;
    
    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        pickupScript = GetComponent<PickupScript>();
        dialogueScript = FindObjectOfType<DialogueScript>();
    }
  
    void Update()
    {
        animator.SetBool("isRunning", navMeshAgent.hasPath);

        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
                GameObject o = hit.transform.gameObject;

                if (o.CompareTag("Ground"))
                {
                    Debug.Log("Moving to location " + o.transform.position);
                    navMeshAgent.destination = hit.point;
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity))
            {
                GameObject o = hit.transform.gameObject;
                var isWithinRadius = IsWithinRadius(o.transform.position);

                if (o.CompareTag("Pet"))
                {
                    if(isWithinRadius)
                    {
                        pickupScript.PickupObject(o);
                    } else
                    {
                        dialogueScript.SetText("You are too far away!");
                    }
                }
                else if (o.CompareTag("MissingPoster"))
                {
                    if (isWithinRadius)
                    {
                        o.GetComponent<MissingPetPoster>().ActivatePoster();
                        o.SetActive(false);
                    }
                    else
                    {
                        dialogueScript.SetText("You are too far away!");
                    }
                }
                else if (o.CompareTag("Owner"))
                {
                    if (isWithinRadius)
                    {
                        var p = pickupScript.GetPickedUpObject().GetComponent<Pet>();
                        var owner = o.GetComponent<Owner>();
                        owner.ReturnPet(p);
                        pickupScript.DropObject(owner.dropPlacement.position);
                    }
                    else
                    {
                        dialogueScript.SetText("You are too far away!");
                    }
                }                
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            pickupScript.DropObject();
        }
    }

    private bool IsWithinRadius(Vector3 other)
    {
        return Vector3.Distance(gameObject.transform.position, other) < interactionRadius;
    }
}
