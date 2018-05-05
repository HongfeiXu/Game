/*
 
1. 设置Tank的初始生命值。
2. 提供坦克受到伤害的处理函数，更新HealthSlider，以及判断是否死亡。
3. 若坦克死亡，则播放TankExplosion效果，并且DeActive Tank GameObject。
 
 */

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TankHealth : MonoBehaviour {

    public float m_StartHealth = 100f;
    public Slider m_Slider;     // HealthSlider
    public Image m_FillImage;   // HealthSlider 中的填充图片
    public Color m_FullHealthColor = Color.green;
    public Color m_ZeroHealthColor = Color.red;
    public GameObject m_TankExplosionPrefab;

    private ParticleSystem m_ExplosionParticle;
    private AudioSource m_ExplosionAudioSource;
    private float m_CurrentHealth;
    private bool m_Dead;

    private void Awake()
    {
        // instantiate Prefabs\TankExplosion，并且获得其 ParticleSystem Component 与 AudioSource Component
        m_ExplosionParticle = Instantiate(m_TankExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudioSource = m_ExplosionParticle.GetComponent<AudioSource>();

        // DeActive TankExplosion GameObject
        m_ExplosionParticle.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        m_CurrentHealth = m_StartHealth;
        m_Dead = false;

        SetHealthUI();
    }

    public void TakeDamage(float amount)
    {
        m_CurrentHealth -= amount;
        m_CurrentHealth = Mathf.Max(m_CurrentHealth, 0f);

        // 受到伤害时，计算Health以及更新HealthUI
        SetHealthUI();

        if(m_CurrentHealth == 0 && !m_Dead)
        {
            OnDeath();
        }
    }

    // 更新 HealthUI 的颜色
    private void SetHealthUI()
    {
        m_Slider.value = m_CurrentHealth;
        // 保持 Color 的 alpha 分量不变
        float alpha = m_FillImage.color.a;
        Color tempColor = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartHealth);
        m_FillImage.color = new Color(tempColor.r, tempColor.g, tempColor.b, alpha);
    }

    // 当Tank死亡时，播放音效以及粒子特效
    private void OnDeath()
    {
        m_Dead = true;
        m_ExplosionParticle.transform.position = transform.position;
        m_ExplosionParticle.gameObject.SetActive(true);

        m_ExplosionParticle.Play();
        m_ExplosionAudioSource.Play();
        gameObject.SetActive(false);
    }
}
