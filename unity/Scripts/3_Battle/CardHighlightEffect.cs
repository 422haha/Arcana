using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class CardHighlightEffect : MonoBehaviour
{
    [System.Serializable]
    public class EffectSettings
    {
        public Color innerColor = new Color(0.4f, 0.8f, 1f, 0.8f);
        public Color outerColor = new Color(0.3f, 0.7f, 1f, 0.0f);
        [Range(0.1f, 2f)] public float animationSpeed = 1f;
        public float waveWidth = 20f;
        public int waveCount = 3;
    }

    public EffectSettings settings = new EffectSettings();

    private Image[] waveEffectImages;
    private RectTransform cardRect;
    private Vector2 lastCardSize;

    private void Awake()
    {
        cardRect = GetComponent<RectTransform>();
        lastCardSize = cardRect.sizeDelta;
        SetupEffects();
    }

    private void SetupEffects()
    {
        waveEffectImages = new Image[settings.waveCount];

        for (int i = 0; i < settings.waveCount; i++)
        {
            Image waveImage = CreateEffectObject($"WaveEffect_{i}");
            waveImage.sprite = CreateEnhancedWaveSprite();
            waveImage.color = Color.white;
            FitRectTransform(waveImage.rectTransform, Vector2.zero);
            waveEffectImages[i] = waveImage;
        }

        PlayWaveAnimations();
    }

    private Image CreateEffectObject(string name)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(transform, false);
        Image img = obj.AddComponent<Image>();
        img.material = new Material(Shader.Find("UI/Default"));
        img.maskable = true;
        return img;
    }

    private void FitRectTransform(RectTransform rect, Vector2 padding)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = -padding * 2f;
        rect.anchoredPosition = Vector2.zero;
    }

    private Sprite CreateEnhancedWaveSprite()
    {
        float cardWidth = cardRect.rect.width;
        float cardHeight = cardRect.rect.height;

        int width = Mathf.RoundToInt(cardWidth);
        int height = Mathf.RoundToInt(cardHeight);

        width = Mathf.Max(width, 1);
        height = Mathf.Max(height, 1);

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Bilinear;
        texture.wrapMode = TextureWrapMode.Clamp;

        // 이펙트 너비 스케일 조정
        float effectScale = Mathf.Min(width, height) / 200f;
        float waveWidth = settings.waveWidth * effectScale;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // 테두리까지의 최소 거리 계산
                float distX = Mathf.Min(x, width - x);
                float distY = Mathf.Min(y, height - y);
                float distance = Mathf.Min(distX, distY);

                // 코너 부분 처리
                if (distX < width * 0.1f && distY < height * 0.1f)
                {
                    distance = Mathf.Sqrt(distX * distX + distY * distY);
                }

                // 그라데이션 계산
                float gradient = 1f - Mathf.Clamp01(distance / waveWidth);
                gradient = Mathf.Pow(gradient, 1.5f); // 부드러운 페이드아웃

                // 색상 및 알파 계산
                Color pixelColor = Color.Lerp(settings.outerColor, settings.innerColor, gradient);
                pixelColor.a *= gradient;

                texture.SetPixel(x, y, pixelColor);
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
    }

    private void PlayWaveAnimations()
    {
        float baseDuration = 2f / settings.animationSpeed;
        float delayBetweenWaves = baseDuration / settings.waveCount;

        for (int i = 0; i < settings.waveCount; i++)
        {
            Image waveImage = waveEffectImages[i];
            waveImage.color = new Color(1f, 1f, 1f, 0f);

            Sequence waveSequence = DOTween.Sequence();

            waveSequence.AppendInterval(i * delayBetweenWaves);
            waveSequence.Append(waveImage.DOFade(1f, baseDuration * 0.3f).SetEase(Ease.OutQuad));
            waveSequence.AppendInterval(baseDuration * 0.4f);
            waveSequence.Append(waveImage.DOFade(0f, baseDuration * 0.3f).SetEase(Ease.InQuad));

            // 스케일 애니메이션 조정
            waveSequence.Join(
                waveImage.transform.DOScale(new Vector3(1.03f, 1.03f, 1f), baseDuration)
                .SetEase(Ease.OutQuad)
            );

            waveSequence.SetLoops(-1, LoopType.Restart);
        }
    }

    private void Update()
    {
        if (cardRect.sizeDelta != lastCardSize)
        {
            lastCardSize = cardRect.sizeDelta;
            UpdateWaveSprites();
        }
    }

    private void UpdateWaveSprites()
    {
        foreach (Image waveImage in waveEffectImages)
        {
            if (waveImage.sprite != null)
            {
                Destroy(waveImage.sprite.texture);
            }
            waveImage.sprite = CreateEnhancedWaveSprite();
        }
    }

    private void OnDestroy()
    {
        DOTween.Kill(gameObject);
    }
}