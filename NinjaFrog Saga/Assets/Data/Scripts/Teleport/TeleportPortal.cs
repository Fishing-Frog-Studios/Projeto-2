using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportPortal : MonoBehaviour, IInteractable
{
    [SerializeField] private TeleportData teleportData;
    [SerializeField] private CameraFollow cameraFollow;

    public void Interact(GameObject interactor)
    {
        if (teleportData == null)
        {
            Debug.LogWarning("TeleportData não configurado!");
            return;
        }

        if (teleportData.playVideo && teleportData.videoClip != null)
        {
            StartCoroutine(PlayCutsceneAndTeleport(interactor));
        }
        else
        {
            ExecuteTeleport(interactor);
        }
    }

    private IEnumerator PlayCutsceneAndTeleport(GameObject interactor)
    {
        if (TeleportCutsceneManager.Instance != null)
        {
            TeleportCutsceneManager.Instance.PlayVideo(teleportData.videoClip);
            yield return new WaitForSeconds((float)teleportData.videoClip.length);
        }
        else
        {
            yield return new WaitForSeconds(teleportData.cutsceneDuration);
        }

        ExecuteTeleport(interactor);
    }

    private void ExecuteTeleport(GameObject interactor)
    {
        if (teleportData.useSameScene)
        {
            TeleportWithinScene(interactor);
        }
        else
        {
            TeleportToOtherScene();
        }
    }

    private void TeleportWithinScene(GameObject interactor)
    {
        GameObject targetPoint = GameObject.Find(teleportData.targetPointName);
        if (targetPoint == null)
        {
            Debug.LogWarning($"Destino {teleportData.targetPointName} não encontrado na cena!");
            return;
        }

        interactor.transform.position = targetPoint.transform.position;

        if (cameraFollow != null)
        {
            cameraFollow.SnapToTarget();
        }

        Debug.Log("Teleporte dentro da mesma cena realizado!");
    }

    private void TeleportToOtherScene()
    {
        PlayerPrefs.SetString("NextSpawnPoint", teleportData.targetPointName);
        SceneManager.LoadScene(teleportData.targetSceneName);
    }
}
