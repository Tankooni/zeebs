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

namespace Indigo.Content
{
	public class TwitchEmoteProvider : LibraryProvider
	{
		private WebClient client;
		private SubscriberEmoteAPI subscriberEmotes;
		private HashSet<string> loadedEmotes;

		public TwitchEmoteProvider()
		{
			client = new WebClient();
			loadedEmotes = new HashSet<string>();
		}

		public override void Precache(Library.ILibraryInternal library)
		{
			LoadGlobalSet(library);
			LoadSubscriberSet(library);
		}

		public override void Load(string filename, Library.ILibraryInternal library)
		{
			var emote = FromFilename(filename);
			AddEmote(emote.code, emote.image_id, library);
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

		private void LoadGlobalSet(Library.ILibraryInternal library)
		{
			string json = null;
			if (Library.FolderExists("content/twitchcache"))
			{
				var qualifiedFilename = string.Format("content/twitchcache/{0}.json", "global");
				if (Library.FileExists(qualifiedFilename))
					json = File.ReadAllText(Library.GetFilename(qualifiedFilename));
			}

			if (json == null)
			{
				json = client.DownloadString(string.Format("http://twitchemotes.com/api_cache/v2/{0}.json", "global"));
			}

			var api = Newtonsoft.Json.JsonConvert.DeserializeObject<GlobalEmoteAPI>(json);
			foreach (var pair in api.emotes)
			{
				try
				{
					AddEmote(pair.Key, pair.Value.image_id, library);
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
			var client = new System.Net.WebClient();
			if (Library.FolderExists("content/twitchcache"))
			{
				var qualifiedFilename = string.Format("content/twitchcache/{0}.json", "subscriber");
				if (Library.FileExists(qualifiedFilename))
					json = File.ReadAllText(Library.GetFilename(qualifiedFilename));
			}

			if (json == null)
			{
				json = client.DownloadString(string.Format("http://twitchemotes.com/api_cache/v2/{0}.json", "subscriber"));
			}

			subscriberEmotes = Newtonsoft.Json.JsonConvert.DeserializeObject<SubscriberEmoteAPI>(json);
		}

		private void AddEmote(string emoteName, int image_id, Library.ILibraryInternal library)
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
			
			if(data == null)
			{
				data = client.DownloadData(string.Format("http://static-cdn.jtvnw.net/emoticons//v1/{0}/1.0", image_id));
				var folder = Library.GetFolderName("content/twitchcache");
				MemoryStream stream = new MemoryStream();
				using (MagickImage mImage = new MagickImage(data))
				{
					mImage.Format = MagickFormat.Png32;
					mImage.Write(stream);
					stream.Position = 0;
				}
				
				File.WriteAllBytes(string.Format("{0}/{1}", folder, emoteName), data = stream.ToArray());
			}

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
				public int image_id { get; set; }
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
				public int image_id { get; set; }
			}
		}
		#endregion
	}

}
