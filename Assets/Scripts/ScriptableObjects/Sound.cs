using UnityEngine.Audio;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sound", menuName = "Sounds/Generic Sound")]
public class Sound : ScriptableObject
{
    public new string name;

    public AudioClip clip;

	[Range(0f, 1f)]
	public float volume = .75f;
	[Range(0f, 1f)]
	public float volumeVariance = 0f;

	[Range(.1f, 3f)]
	public float pitch = 1f;
	[Range(0f, 1f)]
	public float pitchVariance = 0f;

	public bool loop = false;

	public AudioMixerGroup mixerGroup;

	[HideInInspector]
    public AudioSource source;

	public void SetSettings()
	{
		source.clip = clip;
		source.loop = loop;
		source.volume = volume;
		source.pitch = pitch;
		source.outputAudioMixerGroup = mixerGroup;
	}

	public void SetAllProps(string name, AudioClip clip, float volume, float volumeVariance, float pitch, float pitchVariance, bool loop, AudioMixerGroup mixerGroup)
	{
		this.name = name;
		this.clip = clip;
		this.volume = volume;
		this.volumeVariance = volumeVariance;
		this.pitch = pitch;
		this.pitchVariance = pitchVariance;
		this.loop = loop;
		this.mixerGroup = mixerGroup;
	}
}
