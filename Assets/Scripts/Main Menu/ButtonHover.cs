using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Color hoverColor = new Color(0.6f, 0.85f, 1f);
    [SerializeField] private float hoverScale = 1.15f;
    [SerializeField] private float speed = 12f;

    private Color normalColor;
    private Vector3 normalScale;
    private Color targetColor;
    private Vector3 targetScale;

    private void Awake()
    {
        if (label == null) label = GetComponentInChildren<TMP_Text>();

        normalColor = label.color;
        normalScale = transform.localScale;
        targetColor = normalColor;
        targetScale = normalScale;
    }

    private void Update()
    {
        // unscaledDeltaTime keeps it working even if Time.timeScale is 0
        float t = Time.unscaledDeltaTime * speed;
        label.color = Color.Lerp(label.color, targetColor, t);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, t);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetColor = hoverColor;
        targetScale = normalScale * hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetColor = normalColor;
        targetScale = normalScale;
    }

    private void OnDisable()
    {
        // Reset if the menu is hidden while hovered (e.g. opening Settings)
        if (label == null) return;
        label.color = normalColor;
        transform.localScale = normalScale;
        targetColor = normalColor;
        targetScale = normalScale;
    }
}