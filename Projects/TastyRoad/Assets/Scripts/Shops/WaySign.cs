using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaySign : ActionBubble_Interactable
{
    [Header("")]
    [SerializeField] private SpriteRenderer _signIcon;

    [Header("")]
    [SerializeField] private GameObject _subLocationPrefab;

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

        SubLocations_Controller controller = Main_Controller.instance.subLocation;

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
        Main_Controller main = Main_Controller.instance;
        Player_Controller player = main.Player();

        player.Toggle_Controllers(false);

        // curtain scene transition
        main.transitionCanvas.Set_LoadIcon(_signIcon.sprite);
        main.transitionCanvas.CurrentScene_Transition();

        // wait until curtain closes
        while (TransitionCanvas_Controller.transitionPlaying) yield return null;

        // set return point for sub location exit
        _subLocation.Set_ReturnPoint(player.transform);

        // move player to sub location spawn point
        player.transform.position = _subLocation.spawnPoint.position;

        // move camera to sub location
        main.cameraController.UpdatePosition(_subLocation.transform.position);

        player.Toggle_Controllers(true);
    }
}