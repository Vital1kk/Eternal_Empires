using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthEnemy : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] private int hitPoints = 2;

    private bool isDestroyed = false; // Тепер ми її використаємо!

    public void TakeDamage(int dmg)
    {
        // Якщо ворог уже мертвий, ігноруємо подальшу шкоду
        if (isDestroyed) return;

        hitPoints -= dmg;

        if (hitPoints <= 0)
        {
            isDestroyed = true;

            // Звертаємося до скрипту руху, який ми налаштували раніше
            EnemyMovement movement = GetComponent<EnemyMovement>();

            if (movement != null)
            {
                // Запускаємо красиву смерть (анімація, зупинка, видалення через секунду)
                movement.Die();
            }
            else
            {
                // Якщо раптом скрипту EnemyMovement немає, просто видаляємо (запасний варіант)
                enemySpawner.onEnemyDestroy.Invoke();
                Destroy(gameObject);
            }
        }
    }
}