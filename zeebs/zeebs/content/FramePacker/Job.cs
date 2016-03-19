using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FramePacker
{
	public class Job
	{
		public string Name;
		public Dictionary<string, string[]> Anims = new Dictionary<string, string[]>();
		
		private static readonly Regex CommentLine = new Regex(@"^#.*$", RegexOptions.Compiled);
		private static readonly Regex InstructionLine = new Regex(@"^combine\s+(.+?)\s+from\s+(.+?)(?:\s+into\s+(.+?))?$", RegexOptions.Compiled);
		
		public static Job ReadLine(string configLine)
		{
			if (CommentLine.IsMatch(configLine))
				return null;
			
			var match = InstructionLine.Match(configLine);
			if (!match.Success)
				throw new Exception(string.Format("Invalid config line: {0}", configLine));
			
			var job = new Job();
			var pattern = match.Groups[1].Value;
			var input = match.Groups[2].Value;
			var output = match.Groups[3].Value;
			
			job.Name = output;
			if (string.IsNullOrWhiteSpace(output))
				job.Name = new DirectoryInfo(input).Name;
			
			var qualified = Path.Combine(Directory.GetCurrentDirectory(), input);
			var immediateFiles = Directory.GetFiles(qualified, pattern);
			if (immediateFiles.Length != 0)
			{
				job.Anims.Add("", immediateFiles);
			}
			else
			{
				foreach (var animDir in Directory.GetDirectories(qualified))
				{
					var anim = new DirectoryInfo(animDir).Name;
					var files = Directory.GetFiles(animDir, pattern);
					job.Anims.Add(anim, files);
				}
			}
			
			return job;
		}
	}
}
