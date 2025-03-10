using UnityEngine;

public class Portal : MonoBehaviour {
    public Vector3 destination = new Vector3(25, 50.5f, 25); // Above main terrain
    public Material redSkybox;

    void Start() {
        gameObject.tag = "Portal"; // Add Tag "Portal" in Editor
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            other.transform.position = destination;
            RenderSettings.skybox = redSkybox;
            Debug.Log("Teleported to: " + destination);
        }
    }
}