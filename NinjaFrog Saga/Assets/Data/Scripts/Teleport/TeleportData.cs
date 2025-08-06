using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "TeleportData", menuName = "Scriptable Objects/TeleportData")]
public class TeleportData : ScriptableObject
{
    [Header("Configura��o do Teleporte")]
    public string targetSceneName;
    public string targetPointName;
    public bool useSameScene;

    [Header("Cutscene / V�deo")]
    public bool playVideo;
    public VideoClip videoClip;
    public bool playInWorld; // true = cutscene dentro do jogo, false = v�deo full-screen
    public float cutsceneDuration = 2f; // usado quando n�o h� v�deo
}
