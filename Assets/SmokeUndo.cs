using UnityEngine;

public class SmokeUndo : MonoBehaviour {
    private Torch torch;

    void Start() {
        gameObject.tag = "Smoke"; // Ensure tag
    }

    public void Init(Torch t) {
        torch = t;
    }

    public void Undo() {
        torch.RestoreLastDeleted();
        Destroy(gameObject);
    }
}