using UnityEngine;
using UnityEngine.UI; // ถ้าคุณเปลี่ยน UI นับเวลาเป็น TextMeshPro ให้แก้ตรงนี้เป็น using TMPro;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 3f; 

    [Header("Lantern Settings")]
    public GameObject handLanternVisual; 
    public GameObject handLanternFlame;  
    
    // เอาตัวแปร fireDuration ออกไปเลยก็ได้ครับ เพราะเราจะไปดึงเวลาจากแท่นไฟแทนแล้ว
    // แต่เก็บไว้เป็นเวลาตั้งต้นเผื่อกรณีฉุกเฉินได้ครับ
    public float defaultFireDuration = 10f;     
    
    [Header("UI Settings")]
    public Text timerText; // ถ้าใช้ TextMeshPro ให้แก้เป็น public TMP_Text timerText;

    [HideInInspector] public bool hasLantern = false;
    [HideInInspector] public bool isLanternLit = false;
    private float currentFireTimer;

    void Start()
    {
        if (handLanternVisual != null) handLanternVisual.SetActive(false);
        if (handLanternFlame != null) handLanternFlame.SetActive(false);
        if (timerText != null) timerText.gameObject.SetActive(false); 
    }

    void Update()
    {
        // --- ระบบนับเวลาตะเกียงถอยหลัง ---
        if (isLanternLit)
        {
            currentFireTimer -= Time.deltaTime;
            
            if (timerText != null)
            {
                timerText.text = "ไฟจะดับใน: " + Mathf.Ceil(currentFireTimer).ToString() + " วินาที";
            }

            if (currentFireTimer <= 0)
            {
                isLanternLit = false;
                if (handLanternFlame != null) handLanternFlame.SetActive(false);
                if (timerText != null) timerText.gameObject.SetActive(false); 
            }
        }

        // --- ระบบกด E เพื่อโต้ตอบกับสิ่งของ ---
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, interactRange))
            {
                // 1. เก็บตะเกียง
                LanternItem groundLantern = hit.collider.GetComponent<LanternItem>();
                if (groundLantern != null && !hasLantern)
                {
                    hasLantern = true;
                    handLanternVisual.SetActive(true); 
                    Destroy(groundLantern.gameObject); 
                    return; 
                }

                // 2. แท่นจุดไฟ
                FirePedestal pedestal = hit.collider.GetComponent<FirePedestal>();
                if (pedestal != null && hasLantern)
                {
                    if (pedestal.isLit)
                    {
                        // --- จุดที่แก้ไขแล้ว ---
                        isLanternLit = true;
                        
                        // ดึงเวลาจากตัวแปร litDuration ของแท่นไฟนั้นๆ มาเป็นเวลาของตะเกียง
                        currentFireTimer = pedestal.litDuration; 
                        
                        handLanternFlame.SetActive(true);
                        if (timerText != null) timerText.gameObject.SetActive(true); 
                    }
                    else if (!pedestal.isLit && isLanternLit)
                    {
                        // เอาไฟไปจุดใส่แท่นเป้าหมาย
                        pedestal.LightUp();
                    }
                    return;
                }

                // 3. เครื่องกดรหัส (Keypad)
                KeypadObject keypad = hit.collider.GetComponent<KeypadObject>();
                if (keypad != null)
                {
                    FirstPersonController fpsController = GetComponentInParent<FirstPersonController>();
                    if (fpsController != null)
                    {
                        keypad.OpenKeypad(fpsController); 
                    }
                    return;
                }
            }
        }
    }
}