using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Indigo;
using Indigo.Audio;
using System.IO;
using System.Threading;
using zeebs;

public static class SoundManager
{
	public static Sound CurrentSong;
	public static string CloboboboSongName = "";
	public static float MusicVolume { get; set; }
	private static Dictionary<string, Sound> musics = new Dictionary<string, Sound>();
	private static Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();
	public static void Init(float musicVolume)
	{
		MusicVolume = FP.Clamp(musicVolume, 0, 1);
		foreach (string file in Utility.RetrieveFilePathForFilesInDirectory(@"./content/music", @"*.ogg|*.wav"))
		{
			var sound = new Sound(Library.GetSoundStream(file));
			sound.OnComplete += LoopMusic;
			musics.Add(Path.GetFileNameWithoutExtension(file), sound);
		}

		foreach (string file in Utility.RetrieveFilePathForFilesInDirectory(@"./content/sounds", @"*.ogg|*.wav"))
			sounds.Add(Path.GetFileNameWithoutExtension(file), new Sound(Library.GetSoundBuffer(file)));
	}

	private static void LoopMusic()
	{
		if (CurrentSong != null)
			CurrentSong.Stop();
		Sound newSong = musics[CloboboboSongName];
		newSong.Volume = MusicVolume;
		CurrentSong = newSong;
		CurrentSong.Play();
	}

	public static void PlayMusic(string music)
	{
		Utility.MainConfig.CurrentMusic = CloboboboSongName = music;
		if (CurrentSong != null)
			CurrentSong.Stop();
		Sound newSong = musics[music];
		newSong.Volume = MusicVolume;
		CurrentSong = newSong;
		CurrentSong.Play();
	}

	public static void PlaySound(string soundName)
	{
		sounds[soundName].Play();

	}

	/// <summary>
	/// Plays a sound with some volume varience
	/// </summary>
	/// <param name="soundName">Name of sound to play</param>
	/// <param name="minimumVolume">0 to 1</param>
	/// <param name="maxVolume">0 to 1</param>
	public static void PlaySoundVariations(string soundName, float minimumVolume, float maxVolume)
	{
		sounds[soundName].Volume = (FP.Random.Float((int)((maxVolume - minimumVolume) * 100.0f)) / 100.0f) + minimumVolume;
		//sounds[soundName].Volume = (FP.Rand(100 - (int)minimumVolume*100) + (int)minimumVolume)/100.0f;
		sounds[soundName].Play();
	}
}
