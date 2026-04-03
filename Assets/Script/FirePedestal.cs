using UnityEngine;

public class FirePedestal : MonoBehaviour
{
    [Header("Pedestal Settings")]
    public bool isAlwaysLit = false; // ถ้าติ๊กอันนี้ ไฟจะติดตลอดกาล (ใช้สำหรับแท่นต้นทาง)
    public bool isLit = false;
    public float litDuration = 10f;  // ระยะเวลาที่ไฟจะติด (ตั้งค่าแยกกันได้ในแต่ละแท่น)

    [Header("Visuals & Event")]
    public GameObject flameVisual; 
    public PuzzleDoor puzzleDoor; 

    private float timer;

    void Start()
    {
        if (isAlwaysLit)
        {
            isLit = true;
        }
        
        if (flameVisual != null) flameVisual.SetActive(isLit);
    }

    void Update()
    {
        // ระบบนับถอยหลัง (เฉพาะแท่นที่ไม่ได้ตั้งค่าเป็น Always Lit)
        if (isLit && !isAlwaysLit)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Extinguish(); // ไฟดับ
            }
        }
    }

    // ฟังก์ชันสำหรับ "จุดไฟ"
    public void LightUp()
    {
        isLit = true;
        timer = litDuration; // เริ่มนับถอยหลังตามเวลาที่ตั้งไว้ของแท่นนั้นๆ
        
        if (flameVisual != null) flameVisual.SetActive(true);
        
        // สั่งให้ประตูเปิด
        if (puzzleDoor != null)
        {
            puzzleDoor.OpenDoor(); 
        }
        
        Debug.Log(gameObject.name + " ถูกจุดไฟแล้ว! จะดับใน " + litDuration + " วินาที");
    }

    // ฟังก์ชันสำหรับ "ไฟดับ"
    public void Extinguish()
    {
        isLit = false;
        if (flameVisual != null) flameVisual.SetActive(false);
        
        // สั่งให้ประตูปิด (ถ้าต้องการให้พัซเซิลรีเซ็ตเมื่อไฟดับ)
        if (puzzleDoor != null)
        {
            puzzleDoor.CloseDoor(); 
        }

        Debug.Log(gameObject.name + " ไฟดับลงแล้ว");
    }
}