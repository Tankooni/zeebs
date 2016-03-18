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
	public class TwitchAvatarProvider : LibraryProvider
	{
		private readonly string prefix = "twitchAvatar//";
		private WebClient client;
		private HashSet<string> loadedEmotes;

		public TwitchAvatarProvider()
		{
			ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
			client = new WebClient();
			loadedEmotes = new HashSet<string>();
		}

		public override void Precache(Library.ILibraryInternal library)
		{
		}

		public override void Load(string filename, Library.ILibraryInternal library)
		{
			var match = Regex.Match(filename, prefix + @"([\w\W]+)");
			AddAvatar(match.Groups[1].Value, library);
		}

		public override bool MatchPath(string filename)
		{
			return filename.StartsWith(prefix) && Regex.Match(filename, prefix + @"([\w\W]+)").Success;
		}

		private void AddAvatar(string userName, Library.ILibraryInternal library)
		{
			if (loadedEmotes.Contains(userName))
				return;

			byte[] data = null;

			var userJson = client.DownloadString(string.Format("https://api.twitch.tv/kraken/channels/{0}", userName));
			var userData = Newtonsoft.Json.JsonConvert.DeserializeObject<UserData>(userJson);
			if (userData.logo == null)
				return;

			data = client.DownloadData(userData.logo.Replace("300x300", "50x50"));

			MemoryStream stream = new MemoryStream();

			using (MemoryStream bMapStream = new MemoryStream(data))
			{
				using (Bitmap bMap = new Bitmap(bMapStream))
				{
					using (MagickImage mImage = new MagickImage(data))
					{
						switch (bMap.PixelFormat)
						{
							case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
								mImage.Format = MagickFormat.Png8;
								break;
							default:
								mImage.Format = MagickFormat.Png32;
								break;
						}

						mImage.Write(stream);
						stream.Position = 0;
					}
				}
			}

			library.AddTexture(string.Format("{0}{1}", prefix, userName), new MemoryStream(data, false));
			loadedEmotes.Add(userName);
		}

		#region	json structures
		private class UserData
		{
			public string logo;
		}
		#endregion
	}

}
