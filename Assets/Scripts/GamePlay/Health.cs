using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using DG.Tweening;

public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    public float currentHealth { get; private set; }

    private Image healthBar;
    private GameObject healthBarContainer;

    private Coroutine hideHealthBarCoroutine;

    void Start()
    {
        healthBarContainer = this.transform.Find("Health").gameObject;
        healthBar = healthBarContainer.transform.Find("BG").Find("HealthBar").GetComponent<Image>();

        currentHealth = maxHealth;
        UpdateHealthUI();
        healthBarContainer.SetActive(false);
    }
    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.T))
    //     {
    //         TakeDamage(10);
    //     }
    // }
    public bool TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();
        ShowHealthBar();
        ShowDamageText(damage);

        if (hideHealthBarCoroutine != null)
        {
            StopCoroutine(hideHealthBarCoroutine);
        }

        hideHealthBarCoroutine = StartCoroutine(HideHealthBarAfterDelay(3f));
        return currentHealth > 0;
    }

    private void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth;
        }
    }

    private void ShowHealthBar()
    {
        if (healthBarContainer != null)
        {
            healthBarContainer.SetActive(true);
        }
    }

    private IEnumerator HideHealthBarAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (currentHealth < maxHealth)
        {
            healthBarContainer.SetActive(false);
        }
    }
    private void ShowDamageText(float damage)
    {
        GameObject damageTextObj = PoolManager.Instance.Get(ResourceManager._DamageTextPrefab, healthBar.transform.position);
        //damageTextObj.transform.position = Vector3.one * 0.5f;

        TextMeshProUGUI damageText = damageTextObj.GetComponentInChildren<TextMeshProUGUI>();
        if (damageText != null)
        {
            damageText.text = "-" + damage.ToString();
        }

        damageTextObj.transform.DOMoveY(damageTextObj.transform.position.y + 0.5f, 0.5f);
        damageTextObj.transform.DOScale(1f, 0.5f).OnComplete(() =>
        {
            damageTextObj.transform.DOScale(0f, 0.3f).OnComplete(() =>
            {
                damageTextObj.SetActive(false);
            });
        });
    }
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        if (hideHealthBarCoroutine != null)
        {
            StopCoroutine(hideHealthBarCoroutine);
        }

        healthBarContainer.SetActive(false);
    }

}
