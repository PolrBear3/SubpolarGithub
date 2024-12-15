using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubLocation : MonoBehaviour
{
    private Main_Controller _mainController;

    [Header("")]
    [SerializeField] private Transform _spawnPoint;
    public Transform spawnPoint => _spawnPoint;

    [SerializeField] private SpriteRenderer _roamArea;
    public SpriteRenderer roamArea => _roamArea;

    private Vector2 _returnPoint;


    // MonoBehaviour
    private void Awake()
    {
        _mainController = GameObject.FindGameObjectWithTag("MainController").GetComponent<Main_Controller>();
    }


    // Set Functions
    public void Set_ReturnPoint(Transform returnPoint)
    {
        _returnPoint = returnPoint.position;
    }


    // Interact Functions
    public void Exit()
    {
        StartCoroutine(Exit_Coroutine());
    }
    private IEnumerator Exit_Coroutine()
    {
        // set load icon
        _mainController.transitionCanvas.Set_LoadIcon(_mainController.currentLocation.data.locationScrObj.locationIcon);

        // curtain scene transition
        _mainController.transitionCanvas.CurrentScene_Transition();

        // wait until curtain closes
        while (TransitionCanvas_Controller.transitionPlaying) yield return null;

        // move player to current location return spawn point
        _mainController.Player().transform.position = _returnPoint;

        // move camera to current location
        _mainController.cameraController.UpdatePosition(_mainController.currentLocation.transform.position);
    }
}
