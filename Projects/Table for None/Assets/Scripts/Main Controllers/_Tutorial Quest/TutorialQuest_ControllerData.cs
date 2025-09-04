using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialQuest_ControllerData
{
    [ES3Serializable] private List<TutorialQuest> _currentQuests = new();
    public List<TutorialQuest> currentQuests => _currentQuests;
}
