using System.Linq;
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Mainly to differentiate between EnemyDrones and other entities,
    /// Also find the nearest player
    /// </summary>
    public class EnemyDrone : Entity
    {
        [Header("Internal References")] 
        [SerializeField] public SetTargettable[] setTargets;
        
        private void Update()
        {
            if (Instances == null || Instances.Count == 0)
                return;
            
            //Filter for players
            Entity[] players = Instances.Where(entity => entity is Player).ToArray();
            //Get closest player
            Player target = GetClosest(players, transform.position) as Player;
            
            //The the set-targets to target it 
            foreach (var setTarget in setTargets)
                setTarget.Target = target;
        }
    }
}