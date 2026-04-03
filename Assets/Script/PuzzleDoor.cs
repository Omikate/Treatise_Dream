using UnityEngine;

public class PuzzleDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public float moveSpeed = 2f;      
    public float moveDistance = 4f;   
    public bool isOpened = false;

    private Vector3 closedPosition;
    private Vector3 openPosition;

    void Start()
    {
        closedPosition = transform.position;
        openPosition = closedPosition - new Vector3(0, moveDistance, 0); 
    }

    void Update()
    {
        // เลือกตำแหน่งเป้าหมายตามสถานะ isOpened
        Vector3 target = isOpened ? openPosition : closedPosition;
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * moveSpeed);
    }

    public void OpenDoor() { isOpened = true; }
    public void CloseDoor() { isOpened = false; } // เพิ่มฟังก์ชันปิดประตู
}