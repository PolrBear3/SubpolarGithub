using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Interaction : MonoBehaviour, IInteractable
{
    private NPC_Controller _controller;

    // UnityEngine
    private void Awake()
    {
        if (gameObject.TryGetComponent(out NPC_Controller controller)) { _controller = controller; }
    }

    // IInteractable
    public void Interact()
    {
        Leave_Location();
    }

    // After Food Receive, Leave Area
    private IEnumerator Leave_Location_Coroutine()
    {
        // get components
        Detection_Controller detection = _controller.detectionController;
        NPC_Movement movement = _controller.movement;

        // stop free roam
        movement.Stop_FreeRoam();

        // collider toggle off
        detection.BoxCollider_Toggle(false);

        // wait until time pass
        yield return new WaitForSeconds(3f);

        // set target position to outer camera
        float randDirection = Random.Range(-1, 1);
        float posX;

        if (randDirection >= 0)
        {
            randDirection = 1f;
            posX = 2f;
        }
        else
        {
            randDirection = -1f;
            posX = -2f;
        }

        Vector2 targetPos = _controller.mainController.OuterCamera_Position(randDirection, 0f, posX, -3f);
        movement.Assign_TargetPosition(targetPos);

        // if npc reaches target position
        while (movement.At_TargetPosition() == false)
        {
            yield return null;
        }

        // spawn another new customer
        _controller.mainController.currentLocation.Spawn_NPCs(1);

        // untrack this npc
        _controller.mainController.UnTrack_CurrentCharacter(gameObject);

        // remove this npc
        Destroy(gameObject);
    }
    private void Leave_Location()
    {
        StartCoroutine(Leave_Location_Coroutine());
    }
}
