using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FramePacker
{
	class Program
	{
		private static readonly Regex extractNumber = new Regex(@"\w*?(\d+).[\w]+", RegexOptions.Compiled);
		public const string DialogFilter = "Image files|*.png;*.jpg;*.jpeg;*.bmp";
		
		[STAThread]
		public static void Main(string[] args)
		{
			if (args.Length > 0)
				Directory.SetCurrentDirectory(args[0]);
			
			var config = Config.ReadConfig();
			var jobs = config != null ? config.Jobs : CreateJob();
			
			if (jobs == null)
				return;
			
			foreach (var job in jobs)
			{
				if (job.Anims.Count == 0)
					continue;
				
				var bitmaps = job.Anims
					.SelectMany(p => p.Value)
					.Select((file, i) => new { Name = file, Index = i })
					.OrderBy(p => GetNumberProbably(p.Name, p.Index))
					.Select(p => new Bitmap(p.Name))
					.ToList();
				
				int frameWidth = bitmaps.Max(bmp => bmp.Width);
				int frameHeight = bitmaps.Max(bmp => bmp.Height);
				int frameCount = bitmaps.Count;
				
				var size = Size.GetDimensions(frameWidth, frameHeight, frameCount);
				var sprite = new Bitmap(frameWidth * size.Cols, frameHeight * size.Rows);
				
				using (var g = Graphics.FromImage(sprite))
				{
					g.FillRectangle(new SolidBrush(Color.Transparent), 0, 0, sprite.Width, sprite.Height);
					
					int x = 0, y = 0;
					foreach (var bmp in bitmaps)
					{
						var brush = new TextureBrush(bmp);
						g.FillRectangle(brush, x * frameWidth, y * frameHeight, bmp.Width, bmp.Height);
	
						if (++x >= size.Cols)
						{
							x = 0;
							y++;
						}
					}
				}
				
				SaveFiles(sprite, job, config == null);
			}
		}
		
		static List<Job> CreateJob()
		{
			var openDialog = new OpenFileDialog();
			openDialog.Title = "Select animation files";
			openDialog.ShowHelp = false;
			openDialog.Multiselect = true;
			openDialog.RestoreDirectory = false;
			openDialog.InitialDirectory = Directory.GetCurrentDirectory();
			openDialog.Filter = DialogFilter;
			
			var result = openDialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				if (openDialog.FileNames.Length == 0)
				{
					var action = MessageBox.Show("Select at least one file", "No files selected", MessageBoxButtons.OKCancel);
					if (action == DialogResult.Cancel)
						return null;
					else if (action == DialogResult.OK)
						return CreateJob();
				}
				
				var job = new Job();
				job.Name = LCS(openDialog.FileNames.Select(s => Path.GetFileName(s)))
					.TrimEnd("0123456789".ToCharArray());
				
				job.Anims[""] = openDialog.FileNames;
				return new List<Job> { job };
			}
		
			return null;
		}
		
		static string LCS(IEnumerable<string> strings)
		{
			if (!strings.Any())
				return null;
			
			var commonSubstrings = new HashSet<string>(GetSubstrings(strings.First()));
			foreach (var str in strings.Skip(1))
			{
				commonSubstrings.IntersectWith(GetSubstrings(str));
				if (commonSubstrings.Count == 0)
					return null;
			}
			
			return commonSubstrings.OrderByDescending(s => s.Length)
				.FirstOrDefault();
		}
		
		static IEnumerable<string> GetSubstrings(string str)
		{
			for (int c = 0; c < str.Length; c++)
			{
				for (int cc = 0; c + cc <= str.Length; cc++)
				{
					yield return str.Substring(c, cc);
				}
			}
		}
		
		static void SaveFiles(Bitmap sprite, Job job, bool showDialog)
		{
			var filename = job.Name ?? "filename.png";
			
			if (showDialog)
			{
				var saveDialog = new SaveFileDialog();
				saveDialog.FileName = filename;
				saveDialog.Filter = "PNG images|*.png";
				var result = saveDialog.ShowDialog();
				
				if (result == DialogResult.Cancel)
					return;
				
				if (result == DialogResult.OK)
					filename = saveDialog.FileName;
			}
			
			if (!filename.EndsWith(".png"))
				filename += ".png";
			
			if (job.Anims.Count > 1)
			{
				var total = 0;
				var builder = new StringBuilder();
				foreach (var anim in job.Anims)
				{
					builder.AppendLine(anim.Key);
					var frames = anim.Value
						.Select((name, i) => i + total)
						.ToArray();
					
					total += anim.Value.Length;
					builder.AppendLine(string.Join(",", frames));
					builder.AppendLine();
				}
				
				File.WriteAllText(Path.GetFileNameWithoutExtension(filename) + ".txt", builder.ToString());
			}
			
			sprite.Save(filename, ImageFormat.Png);
		}
		
		private static int GetNumberProbably(string filename, int index)
		{
			filename = Path.GetFileName(filename);
			var match = extractNumber.Match(filename);
			if (match.Success)
			{
				int result = 0;
				var group = match.Groups[1].Value;
				if (int.TryParse(group, out result))
					return result;
			}
			
			return index;
		}
	}
}