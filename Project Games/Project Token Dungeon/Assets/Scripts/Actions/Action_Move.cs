using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Move : MonoBehaviour, IAction
{
    public Action_Controller controller;

    public void Action()
    {
        Debug.Log("check");
    }
}
