using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baggage_DropPoint : MonoBehaviour
{
    private List<Baggage> _detectedBaggages = new();
    public List<Baggage> detectedBaggeges => _detectedBaggages;

    // OnTrigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Baggage bag)) return;
        _detectedBaggages.Add(bag);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out Baggage bag)) return;
        _detectedBaggages.Remove(bag);
    }

    //
    public void RePosition_DroppedBaggages()
    {
        for (int i = 0; i < _detectedBaggages.Count; i++)
        {
            if (_detectedBaggages[i].moveable) continue;

            _detectedBaggages[i].transform.localPosition = new Vector2(_detectedBaggages[i].transform.localPosition.x, 1f);
            _detectedBaggages[i].Movement_Toggle(true);
        }
    }
}
