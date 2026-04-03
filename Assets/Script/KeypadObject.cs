using UnityEngine;

public class KeypadObject : MonoBehaviour
{
    public KeypadUI keypadManager; // ลาก KeypadUI มาใส่ช่องนี้

    // ฟังก์ชันนี้จะถูกเรียกจาก PlayerInteract
    public void OpenKeypad(FirstPersonController player)
    {
        if (keypadManager != null)
        {
            keypadManager.playerController = player; // ส่งตัวผู้เล่นให้ UI รู้จัก
            keypadManager.keypadCanvas.SetActive(true); // โชว์หน้าจอกดรหัส
            keypadManager.ClearDisplay(); // ล้างหน้าจอ
            
            player.DisableController(); // สั่งหยุดเดินและปลดเมาส์ออกมาคลิก
        }
    }
}