using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enteractiontest : MonoBehaviour, Interactable
{
    public void Interact()
    {
        Debug.Log(Random.Range(0, 100));
    }
}

