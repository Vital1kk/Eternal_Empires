using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim; // <-- Додали поле для Аніматора
    [SerializeField] private SpriteRenderer spriteRenderer; // <-- Додали для повороту вліво/вправо

    [Header("Attributes")]
    [SerializeField] private float moveSpeed = 2f;

    private Transform target;
    private int pathIndex = 0;
    private bool isDead = false; // <-- Прапорець, щоб мертвий ворог нічого не робив

    public delegate void EnemyReachedLastPoint();
    public static event EnemyReachedLastPoint OnEnemyReachedLastPoint;

    private void Start()
    {
        target = PointManager.main.path[pathIndex];

        // Унікальна швидкість для цього конкретного ворога (базова швидкість +/- 15%)
        moveSpeed = Random.Range(moveSpeed * 0.85f, moveSpeed * 1.15f);

        // Авто-пошук компонентів, якщо забув перетягнути в інспекторі
        if (anim == null) anim = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isDead) return; // Якщо ворог мертвий, зупиняємо виконання Update

        if (Vector2.Distance(target.position, transform.position) <= 0.1f)
        {
            pathIndex++;

            if (pathIndex == PointManager.main.path.Length)
            {
                OnEnemyReachedLastPoint?.Invoke();
                enemySpawner.onEnemyDestroy.Invoke();
                Destroy(gameObject);
                return;
            }
            else
            {
                target = PointManager.main.path[pathIndex];
            }
        }

        // Поворот спрайту залежно від напрямку руху
        FlipSprite();
    }

    private void FixedUpdate()
    {
        if (isDead) return; // Якщо мертвий, не рухаємо його через фізику

        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

    private void FlipSprite()
    {
        // Якщо ціль справа — дивимось направо, якщо зліва — наліво
        if (target.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false; // або true (залежить від того, куди спочатку дивиться твій спрайт)
        }
        else if (target.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
    }

    // --- МЕТОД ДЛЯ СМЕРТІ ВОРОГА ---
    // Викликай його зі скрипту вежі / кулі / гравця, коли у ворога закінчується HP
    public void Die()
    {
        if (isDead) return; // Захист від повторного виклику

        isDead = true;

        // Зупиняємо фізичне тіло, щоб ворог не котився по інерції
        rb.velocity = Vector2.zero;
        rb.simulated = false; // Вимикаємо колізії, щоб інші вороги не спотикалися об труп

        // Запускаємо тригер смерті в Аніматорі
        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        // Викликаємо івент знищення (як у тебе при досягненні кінця вейпоінту)
        enemySpawner.onEnemyDestroy.Invoke();

        // Видаляємо об'єкт через 1 секунду (дай час анімації смерті програтися)
        // Зміни 1.0f на тривалість твоєї анімації, якщо вона довша/коротша
        Destroy(gameObject, 1.0f);
    }
}