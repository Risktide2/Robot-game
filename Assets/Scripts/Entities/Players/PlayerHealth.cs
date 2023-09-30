using UnityEngine;
using UnityEngine.UI;

namespace Entities.Players
{
    /// <summary>
    /// Same as health but with health bar
    /// </summary>
    public class PlayerHealth : Health
    {
        [SerializeField] private Slider healthBar;

        protected override void Start()
        {
            //Setup the health bar UI
            healthBar.maxValue = maxHealth;
            healthBar.minValue = 0;
            healthBar.value = maxHealth;
            healthBar.wholeNumbers = false;
        }

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);
            
            Debug.Log("Player ouch!");

            //Update the health bar UI
            healthBar.value = _currentHealth;
        }
    }
}