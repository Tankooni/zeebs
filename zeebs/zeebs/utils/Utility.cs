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
		public static Dictionary<string, ComEntity> SessionPlayers = new Dictionary<string, ComEntity>();
		public static Dictionary<string, ComEntity> GamePlayers = new Dictionary<string, ComEntity>();

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

		public static Dictionary<string, string> ColorHexes = new Dictionary<string, string> { { "aliceblue", "F0F8FF" }, { "antiquewhite", "FAEBD7" }, { "aqua", "00FFFF" }, { "aquamarine", "7FFFD4" }, { "azure", "F0FFFF" }, { "beige", "F5F5DC" }, { "bisque", "FFE4C4" }, { "black", "000000" }, { "blanchedalmond", "FFEBCD" }, { "blue", "0000FF" }, { "blueviolet", "8A2BE2" }, { "brown", "A52A2A" }, { "burlywood", "DEB887" }, { "cadetblue", "5F9EA0" }, { "chartreuse", "7FFF00" }, { "chocolate", "D2691E" }, { "coral", "FF7F50" }, { "cornflowerblue", "6495ED" }, { "cornsilk", "FFF8DC" }, { "crimson", "DC143C" }, { "cyan", "00FFFF" }, { "darkblue", "00008B" }, { "darkcyan", "008B8B" }, { "darkgoldenrod", "B8860B" }, { "darkgray", "A9A9A9" }, { "darkgrey", "A9A9A9" }, { "darkgreen", "006400" }, { "darkkhaki", "BDB76B" }, { "darkmagenta", "8B008B" }, { "darkolivegreen", "556B2F" }, { "darkorange", "FF8C00" }, { "darkorchid", "9932CC" }, { "darkred", "8B0000" }, { "darksalmon", "E9967A" }, { "darkseagreen", "8FBC8F" }, { "darkslateblue", "483D8B" }, { "darkslategray", "2F4F4F" }, { "darkslategrey", "2F4F4F" }, { "darkturquoise", "00CED1" }, { "darkviolet", "9400D3" }, { "deeppink", "FF1493" }, { "deepskyblue", "00BFFF" }, { "dimgray", "696969" }, { "dimgrey", "696969" }, { "dodgerblue", "1E90FF" }, { "firebrick", "B22222" }, { "floralwhite", "FFFAF0" }, { "forestgreen", "228B22" }, { "fuchsia", "FF00FF" }, { "gainsboro", "DCDCDC" }, { "ghostwhite", "F8F8FF" }, { "gold", "FFD700" }, { "goldenrod", "DAA520" }, { "gray", "808080" }, { "grey", "808080" }, { "green", "00FF00" }, { "greenyellow", "ADFF2F" }, { "honeydew", "F0FFF0" }, { "hotpink", "FF69B4" }, { "indianred", "CD5C5C" }, { "indigo", "4B0082" }, { "ivory", "FFFFF0" }, { "khaki", "F0E68C" }, { "lavender", "E6E6FA" }, { "lavenderblush", "FFF0F5" }, { "lawngreen", "7CFC00" }, { "lemonchiffon", "FFFACD" }, { "lightblue", "ADD8E6" }, { "lightcoral", "F08080" }, { "lightcyan", "E0FFFF" }, { "lightgoldenrodyellow", "FAFAD2" }, { "lightgray", "D3D3D3" }, { "lightgrey", "D3D3D3" }, { "lightgreen", "90EE90" }, { "lightpink", "FFB6C1" }, { "lightsalmon", "FFA07A" }, { "lightseagreen", "20B2AA" }, { "lightskyblue", "87CEFA" }, { "lightslategray", "778899" }, { "lightslategrey", "778899" }, { "lightsteelblue", "B0C4DE" }, { "lightyellow", "FFFFE0" }, { "lime", "00FF00" }, { "limegreen", "32CD32" }, { "linen", "FAF0E6" }, { "magenta", "FF00FF" }, { "maroon", "800000" }, { "mediumaquamarine", "66CDAA" }, { "mediumblue", "0000CD" }, { "mediumorchid", "BA55D3" }, { "mediumpurple", "9370DB" }, { "mediumseagreen", "3CB371" }, { "mediumslateblue", "7B68EE" }, { "mediumspringgreen", "00FA9A" }, { "mediumturquoise", "48D1CC" }, { "mediumvioletred", "C71585" }, { "midnightblue", "191970" }, { "mintcream", "F5FFFA" }, { "mistyrose", "FFE4E1" }, { "moccasin", "FFE4B5" }, { "navajowhite", "FFDEAD" }, { "navy", "000080" }, { "oldlace", "FDF5E6" }, { "olive", "808000" }, { "olivedrab", "6B8E23" }, { "orange", "FFA500" }, { "orangered", "FF4500" }, { "orchid", "DA70D6" }, { "palegoldenrod", "EEE8AA" }, { "palegreen", "98FB98" }, { "paleturquoise", "AFEEEE" }, { "palevioletred", "DB7093" }, { "papayawhip", "FFEFD5" }, { "peachpuff", "FFDAB9" }, { "peru", "CD853F" }, { "pink", "FFC0CB" }, { "plum", "DDA0DD" }, { "powderblue", "B0E0E6" }, { "purple", "800080" }, { "rebeccapurple", "663399" }, { "red", "FF0000" }, { "rosybrown", "BC8F8F" }, { "royalblue", "4169E1" }, { "saddlebrown", "8B4513" }, { "salmon", "FA8072" }, { "sandybrown", "F4A460" }, { "seagreen", "2E8B57" }, { "seashell", "FFF5EE" }, { "sienna", "A0522D" }, { "silver", "C0C0C0" }, { "skyblue", "87CEEB" }, { "slateblue", "6A5ACD" }, { "slategray", "708090" }, { "slategrey", "708090" }, { "snow", "FFFAFA" }, { "springgreen", "00FF7F" }, { "steelblue", "4682B4" }, { "tan", "D2B48C" }, { "teal", "008080" }, { "thistle", "D8BFD8" }, { "tomato", "FF6347" }, { "turquoise", "40E0D0" }, { "violet", "EE82EE" }, { "wheat", "F5DEB3" }, { "white", "FFFFFF" }, { "whitesmoke", "F5F5F5" }, { "yellow", "FFFF00" }, { "yellowgreen", "9ACD32" } };
	}

	public class MainConfig
	{
		public string CurrentMusic { get; set; }
		public int MaxPlayers { get; set; }
		public string AdminUser { get; set; }
		public string OverrideBotUser { get; set; }
		public string OverrideOauth { get; set; }
		public string Channel { get; set; }
		public string DefaultBody { get; set; }
		public bool PreventBotTalking { get; set; }
		public bool IsDebug { get; set; }
		public bool IsOfflineMode { get; set; }
		public string BackgroundColor { get; set; }
		public bool UseBackgroundImage { get; set; }

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
				AdminUser = "",
				OverrideBotUser = "",
				OverrideOauth = "",
				Channel = "#",
				PreventBotTalking = false,
				DefaultBody = "ZeebSmall",
				IsDebug = false,
				IsOfflineMode = false,
				BackgroundColor = "#00FF00",
				UseBackgroundImage = true

			};
			JsonWriter.Save(mainConfig, MainConfigPath, false);
			return mainConfig;
		}

		public static MainConfig LoadMainConfig(string path = null)
		{
			var config = JsonLoader.Load<MainConfig>(path ?? MainConfig.MainConfigPath, false);
			if (String.IsNullOrWhiteSpace(config.OverrideOauth))
			{
				config.OverrideBotUser = Dont.Do1;
				config.OverrideOauth = Dont.Do2;
			}
			return config;
		}
	}
}
