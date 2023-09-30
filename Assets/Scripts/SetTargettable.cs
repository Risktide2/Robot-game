using Entities;
using UnityEngine;

public class SetTargettable : MonoBehaviour
{
    [SerializeField] private float senseDistance;

    public Entity Target
    {
        set
        {
            if (!value)
            {
                _target = null;
                return;
            }
            
            //Only set if within senseDistance
            if(Vector3.Distance(transform.position, value.transform.position) < senseDistance)
                _target = value;
        }
    }

    protected Entity _target;
}