using UnityEngine;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
    private float baseSpeed = 30f;
    public float moveSpeed = 30f;
    public float mouseSensitivity = 500f;
    public float jumpForce = 10f;
    public Transform cameraTransform;
    private float xRotation = 0f;
    public List<GameObject> inventory = new List<GameObject>(20);
    public GameObject chestPrefab;
    public GameObject torch;
    private int toolIndex = 0;
    private bool isGrounded;
    public UnityEngine.UI.Image[] inventorySlots;

    void Start() {
        LockCursor();
        torch.SetActive(false);
        GetComponent<Rigidbody>().mass = 1f;
        GetComponent<Rigidbody>().drag = 0f;
        moveSpeed = baseSpeed;
        gameObject.tag = "Player";
    }

    void Update() {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float speed = Input.GetKey(KeyCode.LeftShift) ? moveSpeed * 1.5f : moveSpeed;
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move = move.normalized * speed * Time.deltaTime;
        GetComponent<Rigidbody>().MovePosition(transform.position + move);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 3f)) {
            isGrounded = true;
        } else {
            isGrounded = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) {
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) {
            toolIndex = (toolIndex + 1) % 2;
            SwitchTool();
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f) {
            toolIndex = (toolIndex - 1 + 2) % 2;
            SwitchTool();
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            UnityEditor.EditorApplication.isPlaying = false;
        }
        PickupAndDrop();
        CraftArchive();
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("HighDensity")) moveSpeed = baseSpeed * 0.7f;
    }

    void OnTriggerExit(Collider other) {
        if (other.CompareTag("HighDensity")) moveSpeed = baseSpeed;
    }

    void PickupAndDrop() {
        if (Input.GetKeyDown(KeyCode.E)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 5f)) {
                GameObject item = hit.collider.gameObject;
                if (item.tag != "Terrain") {
                    if (inventory.Count < 20) {
                        inventory.Add(item);
                        item.SetActive(false);
                        if (inventorySlots.Length == 0) Debug.LogError("No slots assigned!");
                        var slot = inventorySlots[inventory.Count - 1];
                        slot.sprite = Resources.Load<Sprite>("ItemIcon");
                        if (slot.sprite == null) Debug.LogError("ItemIcon not loaded from Resources!");
                        else Debug.Log("Slot " + (inventory.Count - 1) + " filled with " + item.name);
                        if (item.name.Contains("Statue")) slot.color = Color.red;
                        else if (item.name.Contains("Book")) slot.color = Color.blue;
                        else slot.color = Color.white;
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Q)) {
            if (inventory.Count > 0) {
                Vector3 pos = transform.position + transform.forward * 2;
                GameObject chest = Instantiate(chestPrefab, pos, Quaternion.identity);
                chest.transform.localScale = new Vector3(2f, 1f, 1f);
                for (int i = 0; i < inventorySlots.Length; i++) {
                    inventorySlots[i].sprite = null;
                    inventorySlots[i].color = Color.white;
                }
                inventory.Clear();
            }
        }
    }

    void CraftArchive() {
        if (Input.GetKeyDown(KeyCode.C) && inventory.Count >= 2) {
            Vector3 pos = transform.position + transform.forward * 2;
            GameObject archive = Instantiate(chestPrefab, pos, Quaternion.identity);
            archive.transform.localScale = new Vector3(2f, 1f, 1f);
            archive.tag = "Archive";
            for (int i = 0; i < inventorySlots.Length; i++) {
                inventorySlots[i].sprite = null;
                inventorySlots[i].color = Color.white;
            }
            inventory.Clear();
        }
    }

    void SwitchTool() {
        if (toolIndex == 0) torch.SetActive(false);
        else if (toolIndex == 1) torch.SetActive(true);
    }

    void LockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}