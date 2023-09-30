using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Entities
{
    /// <summary>
    /// Basic health script, takes damage and dies
    /// </summary>
    public class Health : MonoBehaviour
    {
        [SerializeField] protected int maxHealth = 100;
        [SerializeField] protected float deathDelay = 0.5f;
    
        public UnityEvent die;

        //The amount of health the player has currently
        protected float _currentHealth;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            //Start player with max health
            _currentHealth = maxHealth;
        }

        public virtual void TakeDamage(float damage)
        {
            //Reduce the player health by damage
            _currentHealth -= damage;
        
            Debug.Log("Ouch!");

            if (_currentHealth <= 0)
                StartCoroutine(Die());
        }

        private IEnumerator Die()
        {
            die?.Invoke();
            yield return new WaitForSeconds(deathDelay);
            Destroy(gameObject);
        }
    }
}