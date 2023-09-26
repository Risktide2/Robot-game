using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private Slider healthBar;
    [SerializeField] private float deathDelay = 0.5f;
    
    public UnityEvent die;

    //The amount of health the player has currently
    private int _currentHealth;

    // Start is called before the first frame update
    private void Start()
    {
        //Start player with max health
        _currentHealth = maxHealth;

        //Setup the health bar UI
        healthBar.maxValue = maxHealth;
        healthBar.minValue = 0;
        healthBar.value = maxHealth;
        healthBar.wholeNumbers = false;
    }

    public void TakeDamage(int damage)
    {
        //Reduce the player health by damage
        _currentHealth -= damage;
        //Update the health bar UI
        healthBar.value = _currentHealth;

        if (_currentHealth <= 0) 
            Invoke(nameof(Die), deathDelay);
    }

    private void Die()
    {
        die?.Invoke();
        Destroy(gameObject);
    }
}