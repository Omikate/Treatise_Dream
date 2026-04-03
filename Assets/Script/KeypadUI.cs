using UnityEngine;
using UnityEngine.UI; // ต้องมีบรรทัดนี้เพื่อใช้ระบบ UI

public class KeypadUI : MonoBehaviour
{
    [Header("Keypad Settings")]
    public string correctCode = "1234"; // รหัสที่ถูกต้อง (ตั้งเองได้เลย)
    private string currentInput = "";   // รหัสที่ผู้เล่นกำลังพิมพ์

    [Header("UI & References")]
    public Text displayText;           // หน้าจอแสดงตัวเลขบน Keypad
    public GameObject keypadCanvas;    // ตัวหน้าจอ UI ทั้งหมด (ไว้สั่งเปิด/ปิด)
    public PuzzleDoor doorToOpen;      // ประตูที่จะให้เปิด

    [HideInInspector] 
    public FirstPersonController playerController; // รับค่าผู้เล่นมาเพื่อล็อค/ปลดล็อคเมาส์

    void Start()
    {
        keypadCanvas.SetActive(false); // ซ่อนหน้าจอกดรหัสไว้ตอนเริ่มเกม
    }

    // ฟังก์ชันนี้จะถูกเรียกเมื่อผู้เล่นกดปุ่มตัวเลขบนหน้าจอ
    public void ButtonPressed(string number)
    {
        if (currentInput.Length < 4) // ให้กดได้สูงสุด 4 ตัว
        {
            currentInput += number;
            UpdateDisplay();
        }

        // ถ้ารหัสครบ 4 ตัวแล้ว ให้เช็คทันที
        if (currentInput.Length == 4)
        {
            CheckCode();
        }
    }

    void CheckCode()
    {
        if (currentInput == correctCode)
        {
            displayText.text = "PASS";
            displayText.color = Color.green;
            
            if (doorToOpen != null) doorToOpen.OpenDoor(); // สั่งเปิดประตู!
            
            Invoke("CloseKeypad", 1f); // รอ 1 วินาทีแล้วปิดหน้าจอ
        }
        else
        {
            displayText.text = "ERROR";
            displayText.color = Color.red;
            currentInput = ""; // ล้างรหัสทิ้ง
            
            Invoke("ClearDisplay", 1f); // รอ 1 วินาทีแล้วให้พิมพ์ใหม่ได้
        }
    }

    public void ClearDisplay()
    {
        currentInput = "";
        displayText.text = "____";
        displayText.color = Color.white;
    }

    void UpdateDisplay()
    {
        displayText.text = currentInput;
    }

    // ฟังก์ชันสำหรับปุ่ม "ออก" (หรือเมื่อกดรหัสถูก)
    public void CloseKeypad()
    {
        keypadCanvas.SetActive(false);
        ClearDisplay();

        // คืนการควบคุมให้ผู้เล่น (ล็อคเมาส์กลับไปตรงกลางจอ)
        if (playerController != null)
        {
            playerController.EnableController();
        }
    }
}