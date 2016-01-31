using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Tankooni.IRC;
using zeebs.entities;
using Utils.Json;

namespace Tankooni
{
	class Utility
	{
		public const string CONTENT_DIR = "content";
		public const string SAVE_DIR = "save";
		public const string TWITCH_SAVE_DIR = "twitchUserData";
		public static MainConfig MainConfig;
		public static TwitchInterface Twitchy;
		public static Dictionary<string, ComEntity> ConnectedPlayers = new Dictionary<string, ComEntity>();
		/// <summary>
		/// Returns all files in a folder using one or more search filters.
		/// Treats filters as or's
		/// </summary>
		/// <param name="path">Path to folder to search</param>
		/// <param name="filters">A list of filters speparated by |</param>
		/// <returns></returns>
		public static string[] RetrieveFilePathForFilesInDirectory(string path, string filters)
		{
			List<string> files = new List<string>();
			foreach (string filter in filters.Split('|'))
				files.AddRange(Directory.GetFiles(path, filter));
			return files.Select(x => x.Remove(0, 2).Replace(@"\", "/")).ToArray();
		}

		public static List<Type> GetTypeFromAllAssemblies<T>()
		{
			var typeList = new List<Type>();
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; ++i)
			{
				var types = assemblies[i].GetTypes();
				for (int j = 0; j < types.Length; ++j)
				{
					var t = types[j];
					if (typeof(T).IsAssignableFrom(t))
						typeList.Add(t);
				}
			}

			return typeList;
		}
	}

	public class MainConfig
	{
		public string CurrentMusic { get; set; }
		public int MaxPlayers { get; set; }
		public string BotUser { get; set; }
		public string Oauth { get; set; }
		public string Channel { get; set; }

		public bool PreventBotTalking { get; set; }

		public static string MainConfigPath
		{
			get
			{
				return Utility.SAVE_DIR + "/" + "MainConfig" + JsonLoader.RESOURCE_EXT;
			}
		}

		public static MainConfig WriteDefaultConfig()
		{
			MainConfig mainConfig = new MainConfig
			{
				CurrentMusic = "",
				MaxPlayers = 500,
				Oauth = "",
				Channel ="#",
				PreventBotTalking = false

			};
			JsonWriter.Save(mainConfig, MainConfigPath, false);
			return mainConfig;
		}

		public static MainConfig LoadMainConfig(string path = null)
		{
			return JsonLoader.Load<MainConfig>(path ?? MainConfig.MainConfigPath, false);
		}
	}
}
