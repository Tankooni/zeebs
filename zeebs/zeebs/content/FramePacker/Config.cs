using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FramePacker
{
	public class Config
	{	
		public List<Job> Jobs = new List<Job>();
		private static readonly Regex ConfigLine = new Regex(@"^$", RegexOptions.Compiled);
		public const string ConfigPath = "spritepacker.txt";
		
		public static Config ReadConfig()
		{
			if (File.Exists(ConfigPath))
			{
				var result = new Config();
				var lines = File.ReadAllLines(ConfigPath);
				
				var lineNumber = 0;
				foreach (var line in lines)
				{
					lineNumber++;
					if (string.IsNullOrWhiteSpace(line))
						continue;
					
					var job = Job.ReadLine(line);
					if (job != null)
						result.Jobs.Add(job);
				}
				
				return result;
			}
			
			return null;
		}
	}
}
