using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace zeebs
{
	class Utility
	{
		public static MainConfig MainConfig;

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
	}

	public class MainConfig
	{
		public string CurrentMusic { get; set; }
	}
}
