using UnityEngine;

public class Torch : MonoBehaviour {
    public GameObject smokePrefab;
    private GameObject lastDeleted;

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 5f)) {
                GameObject target = hit.collider.gameObject;
                if (target.tag == "Smoke") {
                    target.GetComponent<SmokeUndo>().Undo();
                } else if (target.tag != "Terrain") {
                    lastDeleted = target;
                    Vector3 pos = target.transform.position;
                    target.SetActive(false);
                    GameObject smoke = Instantiate(smokePrefab, pos, Quaternion.identity);
                    smoke.AddComponent<SmokeUndo>().Init(this);
                    Destroy(smoke, 5f);
                }
            }
        }
    }

    public void RestoreLastDeleted() {
        if (lastDeleted != null) {
            lastDeleted.SetActive(true);
            lastDeleted = null;
        }
    }
}