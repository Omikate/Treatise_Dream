using UnityEngine;
using UnityEngine.UI; // ต้องมีบรรทัดนี้เพื่อจัดการ UI Text

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 3f;

    [Header("Lantern Settings")]
    public GameObject handLanternVisual;
    public GameObject handLanternFlame;
    public float fireDuration = 10f;
    
    [Header("UI Settings")]
    public Text timerText; // ลาก UI Text (Legacy) มาใส่ช่องนี้

    [HideInInspector] public bool hasLantern = false;
    [HideInInspector] public bool isLanternLit = false;
    private float currentFireTimer;

    void Start()
    {
        if (handLanternVisual != null) handLanternVisual.SetActive(false);
        if (handLanternFlame != null) handLanternFlame.SetActive(false);
        
        // เริ่มเกมมาให้ซ่อนตัวหนังสือนับเวลาไปก่อน
        if (timerText != null) timerText.gameObject.SetActive(false); 
    }

    void Update()
    {
        // ระบบนับเวลาและอัปเดต UI
        if (isLanternLit)
        {
            currentFireTimer -= Time.deltaTime;
            
            // อัปเดตข้อความบนหน้าจอ (แปลงทศนิยมเป็นจำนวนเต็มปัดขึ้นด้วย Mathf.Ceil)
            if (timerText != null)
            {
                timerText.text = "ไฟจะดับใน: " + Mathf.Ceil(currentFireTimer).ToString() + " วินาที";
            }

            // เมื่อหมดเวลา
            if (currentFireTimer <= 0)
            {
                isLanternLit = false;
                if (handLanternFlame != null) handLanternFlame.SetActive(false);
                if (timerText != null) timerText.gameObject.SetActive(false); // ซ่อน UI
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, interactRange))
            {
                // เก็บตะเกียง
                LanternItem groundLantern = hit.collider.GetComponent<LanternItem>();
                if (groundLantern != null && !hasLantern)
                {
                    hasLantern = true;
                    handLanternVisual.SetActive(true);
                    Destroy(groundLantern.gameObject);
                    return;
                }

                // จุดไฟ/รับไฟ จากแท่น
                FirePedestal pedestal = hit.collider.GetComponent<FirePedestal>();
                if (pedestal != null && hasLantern)
                {
                    if (pedestal.isLit)
                    {
                        // รับไฟจากแท่นต้นทาง
                        isLanternLit = true;
                        currentFireTimer = fireDuration; // รีเซ็ตเวลา
                        handLanternFlame.SetActive(true);
                        
                        if (timerText != null) timerText.gameObject.SetActive(true); // โชว์ UI
                    }
                    else if (!pedestal.isLit && isLanternLit)
                    {
                        // ส่งไฟให้แท่นปลายทาง
                        pedestal.LightUp();
                        
                        // (ถ้าอยากให้ตะเกียงดับหลังไขพัซเซิลเสร็จ ให้ลบ // ออกจาก 3 บรรทัดด้านล่างครับ)
                        // isLanternLit = false;
                        // handLanternFlame.SetActive(false);
                        // if (timerText != null) timerText.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}