using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Slider healthSlider; // ลาก Health Bar (Slider) มาใส่ช่องนี้

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrain = 20f; // เสียเท่าไหร่ต่อวินาทีตอนวิ่ง
    public float staminaRegen = 10f;  // ฟื้นฟูเท่าไหร่ต่อวินาที
    public RectTransform staminaBarRect; // ลาก Stamina Bar (Image) มาใส่ช่องนี้เพื่อทำหลอดหดตรงกลาง

    [Header("Audio Settings")]
    public AudioSource playerAudioSource;    // ลำโพงหลัก (เอาไว้เล่นเสียงเจ็บ)
    public AudioClip hurtSound;              // ไฟล์เสียงร้องตอนโดนตี
    
    [Space]
    public AudioSource heavyBreathingSource; // ลำโพงสำหรับเสียงหอบ (ต้องแยกต่างหาก)
    public float staminaThreshold = 25f;     // จุดที่เริ่มหอบ (ต่ำกว่า 25)

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

        // ป้องกันไม่ให้ค่า Stamina ติดลบหรือเกิน 100
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
        // หลอดเลือด (หาร maxHealth เพื่อให้ค่าอยู่ระหว่าง 0-1)
        // **หมายเหตุ: ตรง Slider ของหลอดเลือด ต้องตั้งค่า Max Value เป็น 1 นะครับ**
        if (healthSlider != null) 
        {
            healthSlider.value = currentHealth / maxHealth;
        }

        // หลอด Stamina (บีบสเกลแกน X เพื่อให้หดเข้าหาตรงกลาง)
        if (staminaBarRect != null) 
        {
            float staminaPercent = currentStamina / maxStamina;
            staminaBarRect.localScale = new Vector3(staminaPercent, 1f, 1f);
        }

        // --- 4. เช็คความตาย ---
        if (currentHealth <= 0) 
        {
            // โหลด Scene ปัจจุบันใหม่ (เริ่มเกมใหม่)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // ฟังก์ชันนี้จะถูกเรียกจากตัวศัตรูเมื่อมันมาชนเรา
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        // เล่นเสียงร้องตอนโดนตี
        if (hurtSound != null && playerAudioSource != null)
        {
            playerAudioSource.PlayOneShot(hurtSound);
        }
    }
}