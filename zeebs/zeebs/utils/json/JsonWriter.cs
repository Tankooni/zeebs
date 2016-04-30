using System;
using System.IO;
using Newtonsoft.Json;

namespace Utils.Json
{
	public static class JsonWriter
	{
		public static void Save(object objectToSave, string path, bool addExt = false)
		{
			if(addExt)
				path += JsonLoader.RESOURCE_EXT;
			File.WriteAllText(path, JsonConvert.SerializeObject(objectToSave));
		}
	}
}
