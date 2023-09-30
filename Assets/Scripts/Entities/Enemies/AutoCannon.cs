using System.Collections;
using UnityEngine;

namespace Entities
{
    public class AutoCannon : SetTargettable
    {
        [Header("Attacking")]
        [SerializeField] private float coolDown;
        [SerializeField] private Transform shootPoint;
        [SerializeField] private GameObject projectile;
        
        private bool _alreadyAttacked;

        private void Update()
        {
            if (!_target) return;
            
            //I can see you...
            transform.LookAt(_target.transform);
        
            if (!_alreadyAttacked)
            {
                //Create bullet and point it in the right direction
                Instantiate(projectile, shootPoint.position, shootPoint.rotation);
                //Start reset timer
                StartCoroutine(ResetAttack());
            }
        }
        private IEnumerator ResetAttack()
        {                
            _alreadyAttacked = true;
            yield return new WaitForSeconds(coolDown);
            _alreadyAttacked = false;
        }

    }
}