using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//This class mostly just helps other classes have a link to the player gameobject without using
//GameObject.Find() which is VERY slow and should never be used
public class Player : MonoBehaviour
{
    //Enemies can use this to know where the players are
    public static readonly List<Player> Instances = new();

    //Gets the player closest to a given point
    public static Player GetClosest(Vector3 point)
    {
        float min = float.MaxValue;
        Player theChosen = null;

        foreach(var player in Instances)
        {
            float dist = Vector3.Distance(player.transform.position, point);
            if (dist < min)
            {
                min = dist;
                theChosen = player;
            }
        }

        return theChosen;
    }
    
    //Initialize the list if it hasnt been already and add this player to it
    private void Awake()
    {
        Instances.Add(this);
    }
    
    //Remove this player from the list when the game object is destroyed to prevent null reference
    private void OnDestroy()
    {
        Instances.Remove(this);
    }
}