using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Tankooni.IRC;
using zeebs.entities;
using Utils.Json;
using Indigo.Masks;
using Tankooni.Pathing;

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

		public static void LoadAndProcessClickMap(string path, PathNode[,] pathNodes, Grid pathGrid, int tileSize)
		{
			var map = new SFML.Graphics.Image(path);
			var clickMap = new bool[map.Size.X, map.Size.Y];

			float totalPixelsInTile = tileSize * tileSize;
			int totalTrue = 0;
			int xMax = 0;
			int yMax = 0;
			for (int x = 0; x < pathNodes.GetLength(0); x++)
			{
				for (int y = 0; y < pathNodes.GetLength(1); y++)
				{
					totalTrue = 0;
					xMax = tileSize * x + tileSize;

					for (int xMap = tileSize * x; xMap < xMax; xMap++)
					{
						yMax = tileSize * y + tileSize;
						for (int yMap = tileSize * y; yMap < yMax; yMap++)
							if (xMap < map.Size.X && yMap < map.Size.Y && map.GetPixel((uint)xMap, (uint)yMap).B > 40)
								totalTrue++;
					}
					pathGrid.SetTile(x, y, pathNodes[x, y].Enabled = (totalTrue / totalPixelsInTile) >= .5f);
				}
			}
		}
	}

	public class MainConfig
	{
		public string CurrentMusic { get; set; }
		public int MaxPlayers { get; set; }
		public string BotUser { get; set; }
		public string Oauth { get; set; }
		public string Channel { get; set; }
		public string DefaultBody { get; internal set; }
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
				PreventBotTalking = false,
				DefaultBody = "Navi"

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
