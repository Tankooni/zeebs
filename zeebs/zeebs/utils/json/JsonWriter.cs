using System;
using System.IO;
using Newtonsoft.Json;

namespace Utils.Json
{
	public static class JsonWriter
	{
		//public const string SAVE_PREFIX = "save/";

		public static void Save(object objectToSave, string path, bool addExt = false)
		{
			//string path = JsonLoader.PATH_PREFIX;
			//if (!Directory.Exists(path))
			//	Directory.CreateDirectory(path);

			if(addExt)
				path += JsonLoader.RESOURCE_EXT;
			File.WriteAllText(path, JsonConvert.SerializeObject(objectToSave));
		}
		//public static WriteObjectToDisk<T>(string path)
		//{

		//}
	}
}
