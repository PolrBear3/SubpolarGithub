using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct PropertyPerson
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName
    {
        get
        {
            return FirstName + " " + LastName;
        }
    }
}

public class Properties : MonoBehaviour
{
    PropertyPerson person;

    private void Start()
    {
        person = new PropertyPerson();

        person.FirstName = "David";
        person.LastName = "Kim";

        Debug.Log(person.FullName);
    }
}
