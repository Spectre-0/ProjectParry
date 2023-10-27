using UnityEngine;
using UnityEngine.UI;

public class RedTintEffect : MonoBehaviour
{
    public float fadeDuration = 2.0f; // Duration for how long the tint should last
    private Image redTintImage;
    private float timer = 0f;
    private bool shouldFade = false;
    public static RedTintEffect Instance;  // Singleton instance

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        redTintImage = GetComponent<Image>();
        redTintImage.color = new Color(1, 0, 0, 0);
    }

    private void Update()
    {
        if (shouldFade)
        {
            timer += Time.deltaTime;
            float alphaValue = Mathf.SmoothStep(0.5f, 0, timer);
            redTintImage.color = new Color(1, 0, 0, alphaValue);

            if (timer >= 1)
            {
                timer = 0f;
                shouldFade = false;
            }
        }
    }

    public void PlayerHit()
    {
        redTintImage.color = new Color(1, 0, 0, 0.5f);  // Instantly set the red tint's alpha to 0.5 (or any value you prefer for the intensity of the tint)
        timer = 0f;  // Reset the timer
        shouldFade = true;  // Start the fade out
    }
}
