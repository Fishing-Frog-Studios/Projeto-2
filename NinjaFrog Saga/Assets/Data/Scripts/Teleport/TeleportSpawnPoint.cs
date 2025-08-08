using UnityEngine;

public class TeleportSpawnPoint : MonoBehaviour
{
    private void Start()
    {
        string spawnPoint = PlayerPrefs.GetString("NextSpawnPoint", "");
        if (!string.IsNullOrEmpty(spawnPoint))
        {
            GameObject targetPoint = GameObject.Find(spawnPoint);
            if (targetPoint != null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    player.transform.position = targetPoint.transform.position;
                    CameraFollow cameraFollow = FindObjectOfType<CameraFollow>();
                    if (cameraFollow != null)
                        cameraFollow.SnapToTarget();
                }
            }

            PlayerPrefs.DeleteKey("NextSpawnPoint");
        }
    }
}
