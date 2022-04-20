using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct MyFavoriteMovies
{
    public string genreType;
    public int rating;
    public string[] movieNames;
}

public class Struct_System : MonoBehaviour
{
    public string genreType;
    public int rating;
    public string[] movieNames;

    MyFavoriteMovies movies;
    
    private void Start()
    {
        Set();
    }

    void Set()
    {
        movies = new MyFavoriteMovies();

        movies.genreType = genreType;
        movies.rating = rating;
        movies.movieNames = movieNames;
    }

    public void Print()
    {
        Debug.Log(movies.genreType);
        Debug.Log(movies.rating);

        int i = 0;
        while (i < movies.movieNames.Length)
        {
            Debug.Log(movies.movieNames[i]);
            i += 1;
        }
    }
}
