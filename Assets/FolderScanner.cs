using UnityEngine;
using System.IO;

public class FolderScanner : MonoBehaviour {
    public string folderPath;
    public GameObject shackPrefab, housePrefab, manorPrefab;
    public Transform player;
    private float radius = 10f;

    void Start() {
        if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath)) {
            Debug.LogError("Invalid folder path: " + folderPath);
            return;
        }
        if (player == null || shackPrefab == null || housePrefab == null || manorPrefab == null) {
            Debug.LogError("Missing references in FolderScanner!");
            return;
        }

        DirectoryInfo dir = new DirectoryInfo(folderPath);
        var subDirs = dir.GetDirectories();
        if (subDirs.Length == 0) {
            Debug.Log("No subdirectories found in " + folderPath);
            return;
        }

        int buildingCount = subDirs.Length;
        float angleStep = 360f / buildingCount;
        int i = 0;

        foreach (var subDir in subDirs) {
            float sizeMB = GetFolderSize(subDir) / 1024f / 1024f;
            int fileCount = subDir.GetFiles("*", SearchOption.AllDirectories).Length;
            float score = (sizeMB / 100f) + (fileCount / 25f);

            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 pos = player.position + new Vector3(Mathf.Cos(angle) * radius, 10, Mathf.Sin(angle) * radius);
            GameObject building;
            float heightOffset = 0;
            if (score < 5) {
                building = Instantiate(shackPrefab, pos, Quaternion.identity);
                heightOffset = 2f; // Shack 4x4x4, half height
            } else if (score < 15) {
                building = Instantiate(housePrefab, pos, Quaternion.identity);
                heightOffset = 2.5f; // House 5x5x5
            } else {
                building = Instantiate(manorPrefab, pos, Quaternion.identity);
                heightOffset = 4f; // Manor 8x8x8
            }

            // Snap to terrain
            RaycastHit hit;
            if (Physics.Raycast(building.transform.position, Vector3.down, out hit, 20f)) {
                building.transform.position = new Vector3(pos.x, hit.point.y + heightOffset, pos.z);
            }
            building.transform.LookAt(player);
            Debug.Log("Spawned " + building.name + " at " + building.transform.position + " for " + subDir.Name);
            radius += score * 2f;
            i++;
        }
    }

    float GetFolderSize(DirectoryInfo dir) {
        float size = 0;
        foreach (FileInfo file in dir.GetFiles("*", SearchOption.AllDirectories)) size += file.Length;
        return size;
    }
}