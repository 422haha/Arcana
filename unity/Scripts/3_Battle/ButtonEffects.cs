using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(Button))]
public class ButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Scale Settings")]
    [SerializeField] private float hoverScaleMultiplier = 1.1f;
    [SerializeField] private float scaleDuration = 0.1f;

    [Header("Color Settings")]
    [SerializeField] private float pressedColorMultiplier = 0.8f;
    [SerializeField] private float colorChangeDuration = 0.1f;

    [Header("Outline Glow Settings")]
    [SerializeField] private Color outlineColor = Color.white;
    [SerializeField] private float outlineWidth = 3f;
    [SerializeField] private float glowDuration = 0.2f;

    private Vector3 originalScale;
    private Image buttonImage;
    private Color originalColor;
    private Outline outline;

    private void Awake()
    {
        originalScale = transform.localScale;
        buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            originalColor = buttonImage.color;
            SetupOutline();
        }
    }

    private void SetupOutline()
    {
        // Outline 컴포넌트 추가
        outline = GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }

        // Outline 초기 설정
        outline.effectColor = outlineColor;
        outline.effectDistance = new Vector2(outlineWidth, -outlineWidth);
        outline.enabled = false; // 시작할 때는 비활성화
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 버튼 확대
        transform.DOScale(originalScale * hoverScaleMultiplier, scaleDuration).SetEase(Ease.OutQuad);

        // Outline 효과 켜기
        if (outline != null)
        {
            outline.enabled = true;
            // Outline 색상 투명도를 0에서 1로 페이드인
            DOTween.To(() => outline.effectColor.a,
                       x => outline.effectColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, x),
                       1f, glowDuration)
                   .SetEase(Ease.OutQuad);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 원래 크기로 복귀
        transform.DOScale(originalScale, scaleDuration).SetEase(Ease.OutQuad);

        // Outline 효과 끄기
        if (outline != null)
        {
            // Outline 색상 투명도를 1에서 0으로 페이드아웃
            DOTween.To(() => outline.effectColor.a,
                       x => outline.effectColor = new Color(outlineColor.r, outlineColor.g, outlineColor.b, x),
                       0f, glowDuration)
                   .SetEase(Ease.OutQuad)
                   .OnComplete(() => outline.enabled = false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 버튼 색상 어둡게
        if (buttonImage != null)
        {
            buttonImage.DOColor(originalColor * pressedColorMultiplier, colorChangeDuration).SetEase(Ease.OutQuad);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 원래 색상으로 복귀
        if (buttonImage != null)
        {
            buttonImage.DOColor(originalColor, colorChangeDuration).SetEase(Ease.OutQuad);
        }
    }
}