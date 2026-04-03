using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 3f; // ระยะที่จะกด E เก็บของได้

    [Header("Lantern Settings")]
    public GameObject handLanternVisual; // โมเดลตะเกียงที่อยู่ในมือ
    public GameObject handLanternFlame;  // แสงไฟ/Particle ของตะเกียงในมือ
    public float fireDuration = 10f;     // เวลาที่ไฟจะติดอยู่ (วินาที)
    
    [HideInInspector] public bool hasLantern = false;
    [HideInInspector] public bool isLanternLit = false;
    private float currentFireTimer;

    void Start()
    {
        // เริ่มเกมมา ซ่อนตะเกียงในมือไว้ก่อน
        if (handLanternVisual != null) handLanternVisual.SetActive(false);
        if (handLanternFlame != null) handLanternFlame.SetActive(false);
    }

    void Update()
    {
        // ระบบนับเวลาถอยหลังให้ไฟดับ
        if (isLanternLit)
        {
            currentFireTimer -= Time.deltaTime;
            if (currentFireTimer <= 0)
            {
                isLanternLit = false;
                if (handLanternFlame != null) handLanternFlame.SetActive(false);
                Debug.Log("ไฟตะเกียงดับลงแล้ว!");
            }
        }

        // กด E เพื่อโต้ตอบกับสิ่งของ
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            // ยิง Raycast จากกล้องไปข้างหน้า
            if (Physics.Raycast(transform.position, transform.forward, out hit, interactRange))
            {
                // 1. ถ้าเจอ "ตะเกียงที่วางอยู่บนพื้น"
                LanternItem groundLantern = hit.collider.GetComponent<LanternItem>();
                if (groundLantern != null && !hasLantern)
                {
                    hasLantern = true;
                    handLanternVisual.SetActive(true); // โชว์ตะเกียงในมือ
                    Destroy(groundLantern.gameObject); // ทำลายตะเกียงที่พื้นทิ้ง
                    return;
                }

                // 2. ถ้าเจอ "แท่นจุดไฟ"
                FirePedestal pedestal = hit.collider.GetComponent<FirePedestal>();
                if (pedestal != null && hasLantern)
                {
                    // กรณีที่ 2.1: แท่นมีไฟ -> เราเอาตะเกียงเราไปจุดไฟ
                    if (pedestal.isLit)
                    {
                        isLanternLit = true;
                        currentFireTimer = fireDuration; // รีเซ็ตเวลาใหม่เต็มหลอด
                        handLanternFlame.SetActive(true);
                        Debug.Log("จุดไฟใส่ตะเกียงแล้ว! เหลือเวลา " + fireDuration + " วินาที");
                    }
                    // กรณีที่ 2.2: แท่นไม่มีไฟ และ ตะเกียงเรามีไฟ -> เอาไฟไปจุดแท่นเป้าหมาย!
                    else if (!pedestal.isLit && isLanternLit)
                    {
                        pedestal.LightUp(); // สั่งให้แท่นติดไฟ
                    }
                }
            }
        }
    }
}