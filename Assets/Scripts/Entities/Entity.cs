using System.Collections.Generic;
using UnityEngine;

namespace  Entities
{
    /// <summary>
    /// This class mostly just helps other classes have a link to entity's gameobjects without using
    /// GameObject.Find() which is VERY slow and should never be used
    /// </summary>
    public class Entity : MonoBehaviour
    {
        //Enemies can use this to know where the players are
        public static readonly List<Entity> Instances = new();

        //Gets the player closest to a given point
        public static Entity GetClosest(Entity[] entities, Vector3 point)
        {
            float min = float.MaxValue;
            Entity theChosen = null;

            foreach(var player in entities)
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
    
        //Add this player to the list
        protected virtual void Awake()
        {
            Instances.Add(this);
        }
    
        //Remove this player from the list when the game object is destroyed to prevent null reference
        private void OnDestroy()
        {
            Instances.Remove(this);
        }
    }
}