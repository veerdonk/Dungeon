using UnityEngine.Audio;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sound Collection", menuName = "Sounds/Sound Collection")]
public class SoundCollection : ScriptableObject
{
	public string namePrefix;
    public AudioClip[] clips;

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
	public SoundType type;
}
