using UnityEngine;

public class CatMove : MonoBehaviour {
    public float catSpeed = 2f;
    private float wanderTime = 0f;
    private Vector3 wanderDir;

    void Start() {
        wanderDir = Random.insideUnitSphere.normalized;
        wanderDir.y = 0;
    }

    void Update() {
        wanderTime -= Time.deltaTime;
        if (wanderTime <= 0) {
            wanderDir = Random.insideUnitSphere.normalized;
            wanderDir.y = 0;
            wanderTime = Random.Range(2f, 5f);
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, wanderDir, out hit, 1f) && hit.collider.tag != "Terrain") {
            wanderDir = -wanderDir;
        }
        Vector3 newPos = transform.position + wanderDir * catSpeed * Time.deltaTime;
        newPos.x = Mathf.Clamp(newPos.x, 0, 20);
        newPos.z = Mathf.Clamp(newPos.z, 0, 20);
        transform.position = newPos;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f)) {
            transform.position = new Vector3(transform.position.x, hit.point.y + 0.5f, transform.position.z);
        }
    }
}