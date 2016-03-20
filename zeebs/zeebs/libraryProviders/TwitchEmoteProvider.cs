using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Indigo;
using Indigo.Content;
using ImageMagick;
using System.Drawing;
using System.Runtime.Serialization;

namespace Indigo.Content
{
	public class TwitchEmoteProvider : LibraryProvider
	{
		private WebClient client;
		private SubscriberEmoteAPI subscriberEmotes;
		private HashSet<string> loadedEmotes;
		public HashSet<string> LoadedSpecialEmotes { get; private set; }
		private string currentChannel;

		public TwitchEmoteProvider(string channel = null)
		{
			ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
			client = new WebClient();
			loadedEmotes = new HashSet<string>();
			LoadedSpecialEmotes = new HashSet<string>();
			currentChannel = channel;
		}

		public override void Precache(Library.ILibraryInternal library)
		{
			if (!Library.FolderExists("content/twitchcache"))
				Directory.CreateDirectory("content/twitchcache");
			LoadRobots(library);
			LoadGlobalSet(library);
			LoadSubscriberSet(library);
			LoadBttvSet(library);
			if(currentChannel != null)
				LoadChannelBttvSet(library, currentChannel);
		}

		public override void Load(string filename, Library.ILibraryInternal library)
		{
			var emote = FromFilename(filename);
			AddTwitchEmote(emote.code, emote.image_id, library);
		}

		SubscriberEmoteAPI.SubscriberEmote FromFilename(string filename)
		{
			var match = Regex.Match(filename, @"twitch//([\w\W]+)");
			if (!match.Success)
				return null;

			var emote = subscriberEmotes.FindEmote(match.Groups[1].Value);
			if (emote == null)
				return null;

			return emote;
		}

		public override bool MatchPath(string filename)
		{
			return filename.StartsWith("twitch//") && FromFilename(filename) != null;
		}
		
		private void LoadRobots(Library.ILibraryInternal library)
		{
			string json = null;
			if (Library.FolderExists("content/twitchcache"))
			{
				var qualifiedFilename = string.Format("content/twitchcache/{0}.json", "robots");
				if (Library.FileExists(qualifiedFilename))
					json = File.ReadAllText(Library.GetFilename(qualifiedFilename));
			}
			
			if (json == null)
				throw new Exception("Can't find robots!");
			
			var robots = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
			if (robots == null)
				throw new Exception("Failed to create robot dictionary");
			
			foreach (var pair in robots)
			{
				try {
					AddEmote(pair.Key, pair.Value, library);
				} catch (Exception) {
					FP.Log("Failed to load robot", pair.Key);
				}
			}
		}

		private void LoadGlobalSet(Library.ILibraryInternal library)
		{
			string json = null;
			var qualifiedFilename = string.Format("content/twitchcache/{0}.json", "global");

			try
			{
				json = client.DownloadString(string.Format("https://twitchemotes.com/api_cache/v2/{0}.json", "global"));
				if (!String.IsNullOrEmpty(json))
					File.WriteAllText(qualifiedFilename, json);
				else
					json = null;
			}
			catch
			{
				json = null;
			}

			if (json == null)
			{
				if (Library.FileExists(qualifiedFilename))
				{
					json = File.ReadAllText(Library.GetFilename(qualifiedFilename));
				}
				else
					return;
			}

			var api = Newtonsoft.Json.JsonConvert.DeserializeObject<GlobalEmoteAPI>(json);
			foreach (var pair in api.emotes)
			{
				try
				{
					AddTwitchEmote(pair.Key, pair.Value.image_id, library);
				}
				catch (Exception e)
				{
					FP.Log(pair.Key, e.Message);
				}
			}
		}

		private void LoadSubscriberSet(Library.ILibraryInternal library)
		{
			string json = null;
			var qualifiedFilename = string.Format("content/twitchcache/{0}.json", "subscriber");

			try
			{
				json = client.DownloadString(string.Format("https://twitchemotes.com/api_cache/v2/{0}.json", "subscriber"));
				if (!String.IsNullOrEmpty(json))
					File.WriteAllText(qualifiedFilename, json);
				else
					json = null;
			}
			catch
			{
				json = null;
			}

			if (json == null)
			{
				if (Library.FileExists(qualifiedFilename))
				{
					json = File.ReadAllText(Library.GetFilename(qualifiedFilename));
				}
				else
					return;
			}

			subscriberEmotes = Newtonsoft.Json.JsonConvert.DeserializeObject<SubscriberEmoteAPI>(json);
		}

		private void LoadBttvSet(Library.ILibraryInternal library)
		{
			string json = null;

			var qualifiedFilename = string.Format("content/twitchcache/{0}.json", "bttv");

			try
			{
				json = client.DownloadString(string.Format("https://api.betterttv.net/2/emotes"));
				if (!String.IsNullOrEmpty(json))
					File.WriteAllText(qualifiedFilename, json);
				else
					json = null;
			}
			catch
			{
				json = null;
			}

			if (json == null)
			{
				if (Library.FileExists(qualifiedFilename))
				{
					json = File.ReadAllText(Library.GetFilename(qualifiedFilename));
				}
				else
					return;
			}
			

			var api = Newtonsoft.Json.JsonConvert.DeserializeObject<BaseBttvAPI>(json);
			foreach (var emote in api.emotes)
			{
				try
				{
					AddBttvEmote(emote.code, emote.id, library);
				}
				catch (Exception e)
				{
					FP.Log(emote.code, e.Message);
				}
			}
		}

		private void LoadChannelBttvSet(Library.ILibraryInternal library, string channel)
		{
			string json = null;

			var qualifiedFilename = string.Format("content/twitchcache/{0}_{1}.json", "bttv", channel);

			try
			{
				json = client.DownloadString(string.Format("https://api.betterttv.net/2/channels/{0}", channel));
				if (!String.IsNullOrEmpty(json))
					File.WriteAllText(qualifiedFilename, json);
				else
					json = null;
			}
			catch
			{
				json = null;
			}

			if (json == null)
			{
				if (Library.FileExists(qualifiedFilename))
				{
					json = File.ReadAllText(Library.GetFilename(qualifiedFilename));
				}
				else
					return;
			}


			var api = Newtonsoft.Json.JsonConvert.DeserializeObject<BaseBttvAPI>(json);
			foreach (var emote in api.emotes)
			{
				try
				{
					AddBttvEmote(emote.code, emote.id, library);
				}
				catch (Exception e)
				{
					FP.Log(emote.code, e.Message);
				}
			}
		}

		private void AddBttvEmote(string emoteName, string image_id, Library.ILibraryInternal library)
		{
			AddEmote(emoteName, library, string.Format("https://cdn.betterttv.net/emote/{0}/1x", image_id));
			LoadedSpecialEmotes.Add(emoteName);
		}

		public void AddTwitchEmote(string emoteName, string image_id, Library.ILibraryInternal library)
		{
			AddEmote(emoteName, library, string.Format("https://static-cdn.jtvnw.net/emoticons/v1/{0}/1.0", image_id));
		}

		private void AddEmote(string emoteName, Library.ILibraryInternal library, string url)
		{
			if (loadedEmotes.Contains(emoteName))
				return;

			byte[] data = null;

			if (Library.FolderExists("content/twitchcache"))
			{
				var qualifiedFilename = string.Format("content/twitchcache/{0}", emoteName);
				if (Library.FileExists(qualifiedFilename))
					data = File.ReadAllBytes(Library.GetFilename(qualifiedFilename));
			}
			MemoryStream imageStream = new MemoryStream();

			if (data == null)
			{
				if (new HashSet<string> { "PepePls", "ItsBoshyTime" }.Contains(emoteName))
				{

				}

				data = client.DownloadData(url);
				var folder = Library.GetFolderName("content/twitchcache");

				MagickReadSettings settings = new MagickReadSettings();
				settings.ColorSpace = ColorSpace.sRGB;
				settings.SetDefine(MagickFormat.Png, "format", "png8");

				using (MagickImage mImage = new MagickImage(data, settings))
				{
					//Console.WriteLine(mImage.Format);
					//ImageMagick.MagickImageInfo

					//MagickImageInfo info = new MagickImageInfo(data);
					//mImage.BitDepth(32);
					//Console.WriteLine("| " + info.Format + "| " + emoteName);
					switch (mImage.Format)
					{
						case MagickFormat.Gif:
								
							break;
						case MagickFormat.Png:
						case MagickFormat.Png8:
						case MagickFormat.Png00:
						case MagickFormat.Png24:
						case MagickFormat.Png32:
						case MagickFormat.Png48:
						case MagickFormat.Png64:
							//using (MemoryStream bMapStream = new MemoryStream(data))
							//{
							//	using (Bitmap bMap = new Bitmap(bMapStream))
							//	{
							//		switch (bMap.PixelFormat)
							//		{
							//			case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
							//				mImage.Format = MagickFormat.Png8;
							//				break;
							//			default:
							//				mImage.Format = MagickFormat.Png32;
							//				break;
							//		}
							//	}
							//}
							break;
					}

							

					//This is the only exception out of all default emotes. See issue 12.
					//if (emoteName == "FuzzyOtterOO")
					//	mImage.Format = MagickFormat.Png8;

					mImage.Write(imageStream);
					imageStream.Position = 0;
                }
				
				//File.WriteAllBytes(string.Format("{0}/{1}", folder, emoteName), data = stream.ToArray());
			}

			library.AddTexture(string.Format("twitch//{0}", emoteName), imageStream);
			loadedEmotes.Add(emoteName);
		}
		
		private void AddEmote(string emoteName, string path, Library.ILibraryInternal library)
		{
			if (loadedEmotes.Contains(emoteName))
				return;

			byte[] data = null;

			if (Library.FolderExists("content/twitchcache"))
			{
				var qualifiedFilename = string.Format("content/twitchcache/{0}", path);
				if (Library.FileExists(qualifiedFilename))
					data = File.ReadAllBytes(Library.GetFilename(qualifiedFilename));
			}
			
//			if(data == null)
//			{
//				data = client.DownloadData(string.Format("http://static-cdn.jtvnw.net/emoticons//v1/{0}/1.0", image_id));
//				var folder = Library.GetFolderName("content/twitchcache");
//				MemoryStream stream = new MemoryStream();
//				using (MagickImage mImage = new MagickImage(data))
//				{
//					mImage.Format = MagickFormat.Png32;
//					mImage.Write(stream);
//					stream.Position = 0;
//				}
//				
//				File.WriteAllBytes(string.Format("{0}/{1}", folder, emoteName), data = stream.ToArray());
//			}

			library.AddTexture(string.Format("twitch//{0}", emoteName), new MemoryStream(data, false));
			loadedEmotes.Add(emoteName);
		}

		#region	json structures
		public class EmoteAPI
		{
			public Meta meta { get; set; }
			public Template template { get; set; }

			public class Meta
			{
				public string generated_at { get; set; }
			}

			public class Template
			{
				public string small { get; set; }
				public string medium { get; set; }
				public string large { get; set; }
			}
		}

		public class GlobalEmoteAPI : EmoteAPI
		{
			public Dictionary<string, GlobalEmote> emotes { get; set; }

			public class GlobalEmote
			{
				public string description { get; set; }
				public string image_id { get; set; }
			}
		}

		public class BaseBttvAPI
		{
			public string status { get; set; }
			public string urlTemplate { get; set; }
			public List<BaseBttvEmote> emotes { get; set; }

			public class BaseBttvEmote
			{
				public string id { get; set; }
				public string code { get; set; }
				public string channel { get; set; }
				public string imageType { get; set; }
			}
		}

		public class SubscriberEmoteAPI : EmoteAPI
		{
			public Dictionary<string, Channel> channels { get; set; }

			public IEnumerable<SubscriberEmote> GetAllEmotes()
			{
				foreach (var channel in channels.Values)
				{
					foreach (var emote in channel.emotes)
					{
						yield return emote;
					}
				}
			}

			public SubscriberEmote FindEmote(string emoteName)
			{
				return GetAllEmotes().FirstOrDefault(e => e.code == emoteName);
			}

			public class Channel
			{
				public string title { get; set; }
				public string link { get; set; }
				public object desc { get; set; }
				public string id { get; set; }
				public string badge { get; set; }
				public int set { get; set; }
				public List<SubscriberEmote> emotes { get; set; }
			}

			public class SubscriberEmote
			{
				public string code { get; set; }
				public string image_id { get; set; }
			}
		}
		#endregion
	}

}
