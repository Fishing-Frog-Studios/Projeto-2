using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "TeleportData", menuName = "Scriptable Objects/TeleportData")]
public class TeleportData : ScriptableObject
{
    [Header("Configuração do Teleporte")]
    public string targetSceneName;
    public string targetPointName;
    public bool useSameScene;

    [Header("Cutscene / Vídeo")]
    public bool playVideo;
    public VideoClip videoClip;
    public bool playInWorld; // true = cutscene dentro do jogo, false = vídeo full-screen
    public float cutsceneDuration = 2f; // usado quando não há vídeo
}
