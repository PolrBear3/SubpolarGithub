using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vehicle_Controller : MonoBehaviour, IInteractable
{
    [HideInInspector] public Main_Controller mainController;
    [HideInInspector] public Detection_Controller detection;

    [Header("Insert Vehicle Panel Prefab")]
    [SerializeField] private VehiclePanel_Controller _panel;

    [Header(" ")]
    [SerializeField] private Transform _transparencyPoint;

    // UnityEngine
    private void Awake()
    {
        mainController = FindObjectOfType<Main_Controller>();
        mainController.Track_CurrentStation(gameObject);

        if (gameObject.TryGetComponent(out Detection_Controller detection)) { this.detection = detection; }
    }

    private void Start()
    {
        VehiclePanel_Toggle(false);
    }

    private void Update()
    {
        Transparency_Update();
    }

    // IInteractable
    public void Interact()
    {
        Player_InputSystem_Toggle(false);
        VehiclePanel_Toggle(true);
    }

    // OnTrigger
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (detection.Has_Player() == false)
        {
            LeanTween.alpha(gameObject, 1f, 0.01f);
        }
    }

    // Vehicle Prefab Control
    private void Transparency_Update()
    {
        if (detection.Has_Player() == false) return;

        if (detection.player.transform.position.y > _transparencyPoint.position.y)
        {
            LeanTween.alpha(gameObject, 0.3f, 0.01f);
        }
        else
        {
            LeanTween.alpha(gameObject, 1f, 0.01f);
        }
    }

    // Panel Control
    public void VehiclePanel_Toggle(bool toggleOn)
    {
        if (toggleOn == false)
        {
            _panel.gameObject.SetActive(false);

            return;
        }

        _panel.gameObject.SetActive(true);
    }

    public void Player_InputSystem_Toggle(bool toggleOn)
    {
        if (detection.player.gameObject.TryGetComponent(out PlayerInput playerInput) == false) return;

        if (toggleOn == false)
        {
            playerInput.enabled = false;
            return;
        }

        playerInput.enabled = true;
    }
}
