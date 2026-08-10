using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonJuice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Settings")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float pressScale = 0.9f;
    [SerializeField] private float speed = 15f;

    [Header("References")]
    [SerializeField] private Animator anim;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isHovering = false; // „и знаходитьс€ мишка над кнопкою зараз
    private bool isPressed = false;  // „и затиснута кнопка зараз

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        ResetButton();
    }

    private void OnDisable()
    {
        ResetButton();
    }

    private void ResetButton()
    {
        isHovering = false;
        isPressed = false;
        transform.localScale = originalScale;
        targetScale = originalScale;
        if (anim != null)
        {
            anim.SetTrigger("Normal");
            anim.ResetTrigger("Pressed");
        }
    }

    private void Update()
    {
        // ѕлавно м≥н€Їмо розм≥р до ц≥льового
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        // якщо ми повернулис€ на кнопку, не в≥дпускаючи мишку Ч вона маЇ знову стати маленькою (pressScale)
        // якщо просто навели Ч стаЇ великою (hoverScale)
        targetScale = isPressed ? originalScale * pressScale : originalScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        //  оли мишка п≥шла геть Ч завжди повертаЇмо до ориг≥нального розм≥ру, 
        // нав≥ть €кщо кнопка ще затиснута (це прибере тв≥й баг з "роздут≥стю")
        targetScale = originalScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        targetScale = originalScale * pressScale;

        if (anim != null)
        {
            anim.SetTrigger("Pressed");
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;

        //  оли в≥дпускаЇмо мишку:
        // якщо ми все ще над кнопкою Ч ставимо розм≥р наведенн€ (hoverScale)
        // якщо ми вже поза кнопкою Ч повертаЇмо ориг≥нальний розм≥р
        if (isHovering)
        {
            targetScale = originalScale * hoverScale;
        }
        else
        {
            targetScale = originalScale;
        }

        if (anim != null)
        {
            anim.SetTrigger("Normal");
        }
    }
}