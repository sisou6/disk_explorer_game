using UnityEngine;

public class BatSwarm : MonoBehaviour {
    public float swarmSpeed = 3f;
    private Vector3 target;

    void Start() {
        target = transform.position + new Vector3(0, 2, 0); // Hover above
    }

    void Update() {
        transform.position = Vector3.Lerp(transform.position, target + Random.insideUnitSphere * 0.5f, swarmSpeed * Time.deltaTime);
    }
}