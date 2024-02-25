using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLauncher_Bullet : MonoBehaviour
{
    [SerializeField] private float _distance;
    [SerializeField] private float _height;
    [SerializeField] private float _endTime;

    private float _direction;
    private float _timeElapsed;

    public delegate void Update_Event();
    public event Update_Event Movement;



    // UnityEngine
    private void Update()
    {
        Movement?.Invoke();
    }



    //
    public void Set_Direction(float direction)
    {
        if (direction >= 0) _direction = 1f;
        else _direction = -1f;
    }

    //
    public void Basic_Movement()
    {

    }

    public void Parabola_Movement()
    {
        _endTime -= Time.deltaTime;

        _timeElapsed += Time.deltaTime;
        _timeElapsed %= 5f;

        Vector2 endPoint = new (_distance * _direction, 0);

        transform.localPosition = MathParabola.Parabola(Vector2.zero, endPoint, _height, _timeElapsed);

        if (_endTime > 0) return;
        Destroy(gameObject);
    }
}