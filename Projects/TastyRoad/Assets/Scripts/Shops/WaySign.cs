using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaySign : ActionBubble_Interactable
{
    [Header("")]
    [SerializeField] private SpriteRenderer _signIcon;

    [Header("")]
    [SerializeField] private GameObject _subLocationPrefab;

    [Header("Exit Settings")]
    [SerializeField] private bool _isExit;
    [SerializeField] private SubLocation _subLocation;


    // MonoBehaviour
    private new void Start()
    {
        base.Start();

        Set_SubLocation();
        OnAction1Input += Moveto_SubLocation;
    }

    private void OnDestroy()
    {
        OnAction1Input -= Moveto_SubLocation;
    }


    // Set Functions
    private void Set_SubLocation()
    {
        // return if current sign is exit
        if (_subLocation != null) return;

        // instantiate sub location
        GameObject locationObj = Instantiate(_subLocationPrefab);
        SubLocation subLocation = locationObj.GetComponent<SubLocation>();

        SubLocations_Controller controller = mainController.subLocation;

        // track
        _subLocation = subLocation;
        controller.Track(subLocation);

        // reposition on game scene
        controller.RePosition();
    }


    // Interact Functions
    private void Moveto_SubLocation()
    {
        // exit sub location
        if (_isExit)
        {
            _subLocation.Exit();
            return;
        }

        // enter sub location
        StartCoroutine(Moveto_SubLocation_Coroutine());
    }
    private IEnumerator Moveto_SubLocation_Coroutine()
    {
        // curtain scene transition
        mainController.transitionCanvas.Set_LoadIcon(_signIcon.sprite);
        mainController.transitionCanvas.CurrentScene_Transition();

        // wait until curtain closes
        while (TransitionCanvas_Controller.transitionPlaying) yield return null;

        // set return point for sub location exit
        _subLocation.Set_ReturnPoint(mainController.Player().transform);

        // move player to sub location spawn point
        mainController.Player().transform.position = _subLocation.spawnPoint.position;

        // move camera to sub location
        mainController.cameraController.UpdatePosition(_subLocation.transform.position);
    }
}