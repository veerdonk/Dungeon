using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;

    public List<Sound> sounds;
	public SoundCollection[] collections;

	private Dictionary<SoundType, List<string>> soundsByType = new Dictionary<SoundType, List<string>>();

	void Awake()
    {
		if (instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}

		foreach (SoundCollection soundCollection in collections)
		{
			int index = 0;
			foreach (AudioClip clip in soundCollection.clips)
			{
				Sound newSound = (Sound)ScriptableObject.CreateInstance("Sound");
				newSound.SetAllProps(soundCollection.namePrefix + index, soundCollection.clips[index], soundCollection.volume, soundCollection.volumeVariance, soundCollection.pitch, soundCollection.pitchVariance, soundCollection.loop, soundCollection.mixerGroup);
				sounds.Add(newSound);
				if (soundsByType.ContainsKey(soundCollection.type))
				{
					soundsByType[soundCollection.type].Add(newSound.name);
				}
				else
				{
					soundsByType[soundCollection.type] = new List<string> { newSound.name };
				}
				
				index++;
			}
		}

		foreach (Sound sound in sounds)
        {
			sound.source = gameObject.AddComponent<AudioSource>();
			sound.SetSettings();
		}
    }

	private void Start()
	{
		//Start playiong some bgm
		sounds.Find(sound => sound.name == "medieval_music_2").source.Play();
	}


	public void PlayOnce(string soundName)
	{
		Sound s = sounds.Find(sound => sound.name == soundName);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + soundName + " not found!");
			return;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		//s.source.PlayOneShot(s.source.clip, s.source.volume);
		s.source.Play();
	}

	public void PlayRandomFromNameList(string[] soundsArr)
	{
		PlayOnce(soundsArr[UnityEngine.Random.Range(0, soundsArr.Length)]);
	}

	public void PlayRandomOfType(SoundType type)
	{
		PlayOnce(soundsByType[type][UnityEngine.Random.Range(0, soundsByType[type].Count)]);
	}

}

public enum SoundType
{
	FOOTSTEP_STONE,
	METAL_HIT,
	ORC_HURT,
	SKELETON_HURT,
	PICKUP,
	ITEM_DROP,
	DASH,
	FIREBALL
}
