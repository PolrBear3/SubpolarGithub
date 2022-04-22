using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Person
{
    private string _firstName;
    public string firstName { get { return _firstName; } set { _firstName = value; } }
    private string _lastName;
    public string lastName { get { return _lastName; } set { _lastName = value; } }

    public string fullName { get { return _firstName + " " + _lastName; }}
}

public class Properties : MonoBehaviour
{
    Person person = new Person();

    private void Start()
    {
        person.firstName = "David";
        person.lastName = "Kim";
        Debug.Log(person.fullName);
    }
}
