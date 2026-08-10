using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource walksound;
    [SerializeField] private AudioSource deadsound;
    [SerializeField] private AudioSource monsterAudioSource; // Джерело звуку для хрипів/звуків монстра
    [SerializeField] private AudioClip[] monsterSounds; // Масив аудіокліпів (закидай сюди будь-яку кількість)

    [Header("Monster Sound Timing")]
    [SerializeField] private float minSoundInterval = 3f; // Мінімальна пауза між хрипами
    [SerializeField] private float maxSoundInterval = 7f; // Максимальна пауза між хрипами

    [Header("Attributes")]
    [SerializeField] private float moveSpeed = 2f;

    private Transform target;
    private int pathIndex = 0;
    private bool isDead = false;

    public delegate void EnemyReachedLastPoint();
    public static event EnemyReachedLastPoint OnEnemyReachedLastPoint;

    private void Start()
    {
        target = PointManager.main.path[pathIndex];
        moveSpeed = Random.Range(moveSpeed * 0.85f, moveSpeed * 1.15f);

        if (anim == null) anim = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        // 1. Запуск циклічного звуку ходьби
        if (walksound != null)
        {
            walksound.loop = true;
            walksound.Play();
        }

        // 2. Запуск таймера для випадкових хрипів/звуків монстра
        if (monsterSounds != null && monsterSounds.Length > 0 && monsterAudioSource != null)
        {
            StartCoroutine(PlayRandomMonsterSounds());
        }
    }

    private void Update()
    {
        if (isDead) return;

        if (Vector2.Distance(target.position, transform.position) <= 0.1f)
        {
            pathIndex++;

            if (pathIndex == PointManager.main.path.Length)
            {
                OnEnemyReachedLastPoint?.Invoke();
                enemySpawner.onEnemyDestroy.Invoke();

                // Зручно вимкнути звук перед знищенням
                StopAllCoroutines();
                Destroy(gameObject);
                return;
            }
            else
            {
                target = PointManager.main.path[pathIndex];
            }
        }

        FlipSprite();
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        Vector2 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
    }

    private void FlipSprite()
    {
        if (target.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else if (target.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
    }

    // Корутина для програвання випадкових звуків під час ходьби
    private IEnumerator PlayRandomMonsterSounds()
    {
        while (!isDead)
        {
            // Чекаємо випадковий проміжок часу перед наступним хрипом
            float waitTime = Random.Range(minSoundInterval, maxSoundInterval);
            yield return new WaitForSeconds(waitTime);

            if (isDead) break;

            // Обираємо випадковий звук з масиву
            int randomIndex = Random.Range(0, monsterSounds.Length);
            AudioClip clipToPlay = monsterSounds[randomIndex];

            if (clipToPlay != null)
            {
                monsterAudioSource.PlayOneShot(clipToPlay);
            }
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        // Зупиняємо корутину хрипів, щоб після смерті монстр більше не видавав живих звуків
        StopAllCoroutines();

        rb.velocity = Vector2.zero;
        rb.simulated = false;

        // Зупиняємо звук ходьби
        if (walksound != null && walksound.isPlaying)
        {
            walksound.Stop();
        }

        // Відтворюємо звук смерті
        if (deadsound != null && deadsound.clip != null)
        {
            // Створюємо тимчасове джерело звуку у світовій позиції, 
            // щоб звук смерті дограв до кінця, навіть якщо Destroy(gameObject) спрацює раніше
            AudioSource.PlayClipAtPoint(deadsound.clip, transform.position, deadsound.volume);
        }

        if (anim != null)
        {
            anim.SetTrigger("Die");
        }

        enemySpawner.onEnemyDestroy.Invoke();

        Destroy(gameObject, 1.0f);
    }
}