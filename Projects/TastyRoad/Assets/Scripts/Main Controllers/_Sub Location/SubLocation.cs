using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubLocation : MonoBehaviour
{
    [Header("")]
    [SerializeField] private Transform _spawnPoint;
    public Transform spawnPoint => _spawnPoint;

    [SerializeField] private SpriteRenderer _roamArea;
    public SpriteRenderer roamArea => _roamArea;

    [Header("")]
    [SerializeField] private bool _outerBlack;
    public bool outerBlack => _outerBlack;

    private Vector2 _returnPoint;


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
        Main_Controller main = Main_Controller.instance;
        Player_Controller player = main.Player();

        player.Toggle_Controllers(false);

        // set load icon
        int worldNum = main.worldMap.currentData.worldNum;
        main.transitionCanvas.Set_LoadIcon(main.dataController.World_Data(worldNum).worldIcon);

        // curtain scene transition
        main.transitionCanvas.CurrentScene_Transition();

        // sound
        Audio_Controller.instance.Play_OneShot(gameObject, 0);
        
        // wait until curtain closes
        while (TransitionCanvas_Controller.instance.coroutine != null) yield return null;

        // move player to current location return spawn point
        main.Player().transform.position = _returnPoint;

        // move camera to current location
        main.cameraController.UpdatePosition(main.currentLocation.transform.position);

        player.Toggle_Controllers(true);
    }
}
