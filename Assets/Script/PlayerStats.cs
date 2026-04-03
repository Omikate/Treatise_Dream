using UnityEngine;
using UnityEngine.UI; 

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthSlider; 

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrain = 20f; 
    public float staminaRegen = 10f;  
    public RectTransform staminaBarRect; 

    [Header("Audio Settings")]
    public AudioSource playerAudioSource;    
    public AudioClip hurtSound;              
    
    [Space]
    public AudioSource heavyBreathingSource; 
    public float staminaThreshold = 25f;     

    public bool isSprinting = false;

    void Start()
    {
        // เริ่มเกมมาให้เลือดและพลังงานเต็ม
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    void Update()
    {
        // --- 1. ระบบลด/ฟื้นฟู Stamina ---
        if (isSprinting && currentStamina > 0)
        {
            currentStamina -= staminaDrain * Time.deltaTime;
        }
        else if (!isSprinting && currentStamina < maxStamina)
        {
            currentStamina += staminaRegen * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        // --- 2. ระบบเสียงหอบอัตโนมัติ ---
        if (heavyBreathingSource != null)
        {
            if (currentStamina < staminaThreshold)
            {
                if (!heavyBreathingSource.isPlaying) heavyBreathingSource.Play();
            }
            else if (currentStamina > (staminaThreshold + 10f)) 
            {
                if (heavyBreathingSource.isPlaying) heavyBreathingSource.Stop();
            }
        }

        // --- 3. อัปเดต UI หน้าจอ ---
        if (healthSlider != null) 
        {
            healthSlider.value = currentHealth / maxHealth;
        }

        if (staminaBarRect != null) 
        {
            float staminaPercent = currentStamina / maxStamina;
            staminaBarRect.localScale = new Vector3(staminaPercent, 1f, 1f);
        }

        // --- 4. เช็คความตาย ---
        if (currentHealth <= 0) 
        {
            Respawn(); // เรียกใช้งานระบบจุดเซฟ
        }
    }

    // ฟังก์ชันนี้จะถูกเรียกจากตัวศัตรูเมื่อมันมาชนเรา
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (hurtSound != null && playerAudioSource != null)
        {
            playerAudioSource.PlayOneShot(hurtSound);
        }
    }

    // ฟังก์ชันฟื้นคืนชีพที่จุดเซฟ
    public void Respawn()
    {
        Debug.Log("คุณตายแล้ว! กำลังวาร์ปไปจุดเซฟ...");
        
        // 1. คืนค่าเลือดและ Stamina ให้เต็ม
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        // 2. ปิด Character Controller ชั่วคราว (หัวใจหลักที่ทำให้วาร์ปติด!)
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false; 
        }

        // 3. วาร์ปกลับไปที่จุดเซฟล่าสุด
        if (CheckpointManager.instance != null)
        {
            transform.position = CheckpointManager.instance.lastCheckPointPos;
        }
        else
        {
            Debug.LogWarning("หา CheckpointManager ไม่เจอ! ทำให้กลับจุดเซฟไม่ได้");
        }

        // 4. เปิด Character Controller กลับมาใช้งานตามปกติ
        if (cc != null)
        {
            cc.enabled = true; 
        }

        // 5. หยุดแรงเหวี่ยงฟิสิกส์ (กันกระเด็น)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}