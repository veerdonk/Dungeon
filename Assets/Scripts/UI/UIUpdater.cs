using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUpdater : MonoBehaviour
{
    public Text goldText;
    public Slider expSlider;
    public Slider health;
    public Image healthFill;
    public Gradient healthGradient;
    public Text levelUpText;
    public GameObject gameOver;

    public ParticleSystem healthLossPS;

    public Image[] dashResfresh;

    public bool displayLevelUp;

    public static UIUpdater instance;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogWarning("More than one instance of UIUpdater present");
        }
        instance = this;
    }

    private void Start()
    {
        Debug.Log("Subscribing to onplayerstatchange");
        PlayerManager.instance.OnPlayerStatChange += UpdateStatsInUI;
    }

    public void UpdateStatsInUI()
    {
        //Set gold
        goldText.text = Inventory.instance.gold.ToString();
        //Set exp
        expSlider.maxValue = PlayerManager.instance.expForNextLvl;
        expSlider.value = PlayerManager.instance.exp;
        //Set health
        health.value = PlayerManager.instance.health;
        health.maxValue = PlayerManager.instance.maxHealth;
        healthFill.color = healthGradient.Evaluate(health.normalizedValue);
        //Display level up message
        if (displayLevelUp)
        {
            levelUpText.enabled = true;
            //After 1 second start fading out text
            StartCoroutine(Util.ExecuteAfterTime(1, () =>
            {
                StartCoroutine(Util.FadeOutRoutine(levelUpText, 1));
            }));
            displayLevelUp = false;
        }

        if (PlayerController2D.instance.dead)
        {
            gameOver.SetActive(true);
        }
    }

    public void SetDashResetIndicator()
    {

        
        foreach (Image dashImage in dashResfresh)
        {
            if(dashImage.fillAmount == 1)
            {
                Debug.Log("Image to reset selected: " + dashImage.name);
                //Full circle found
                StartCoroutine(StartDashCooldown(dashImage));
                break;
            }
        }
    }


    IEnumerator StartDashCooldown(Image dashImage)
    {
        dashImage.fillAmount = 0;
        float startTime = PlayerController2D.instance.dashCooldown;
        float timeLeft = 0;
        
        while(timeLeft < startTime)
        {
            timeLeft += Time.deltaTime;

            dashImage.fillAmount = timeLeft/startTime;
            yield return null;
        }

    }

}
