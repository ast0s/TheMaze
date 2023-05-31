using UnityEngine;
using UnityEngine.Audio;

public class SettingsAdjuster : MonoBehaviour
{
    [SerializeField]
    private AudioMixer audioMixer;
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master Volume", volume);
        Debug.Log($"Master volume: {volume}");
    }
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("Music Volume", volume);
        Debug.Log($"Music volume: {volume}");
    }
    public void SetEffectsVolume(float volume)
    {
        audioMixer.SetFloat("Effects Volume", volume);
        Debug.Log($"effects volume: {volume}");
    }
}
