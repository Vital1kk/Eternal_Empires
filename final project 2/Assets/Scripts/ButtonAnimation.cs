using UnityEngine;
using UnityEngine.EventSystems; 

public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")]
    [SerializeField] private float hoverScale = 1.1f;    
    [SerializeField] private float pressScale = 0.9f;    
    [SerializeField] private float speed = 15f;

    [Header("References")]
    [SerializeField] private Animator anim;

    private Vector3 originalScale;
    private Vector3 targetScale;

    private void Awake()
    {
        // Запам'ятовуємо розмір один раз при завантаженні
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        // ЩОРАЗУ при відкритті панелі скидаємо все в нуль
        transform.localScale = originalScale;
        targetScale = originalScale;

        // Також примусово кажемо аніматору повернутися в нормальний стан
        if (anim != null)
        {
            anim.SetTrigger("Normal");
            // Додатково можна скинути тригер натискання, щоб він не "вистрілив" сам
            anim.ResetTrigger("Pressed");
        }
    }

    private void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        if (anim == null) anim = GetComponent<Animator>();
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = originalScale * pressScale;

        if (anim != null)
        {
            anim.SetTrigger("Pressed");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;

        if (anim != null)
        {
            anim.SetTrigger("Normal");
        }
    }
}