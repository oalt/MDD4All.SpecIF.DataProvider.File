/*
 * Copyright (c) MDD4All.de, Dr. Oliver Alt
 */
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;


namespace MDD4All.SpecIF.DataProvider.File
{
	public class SpecIfFileReaderWriter
	{
		public static T ReadDataFromSpecIfFile<T>(string path)
		{
			T result = default(T);

			StreamReader file = null;

			try
			{

				file = new StreamReader(path);

				JsonSerializer serializer = new JsonSerializer();

				result = (T)serializer.Deserialize(file, typeof(T));
			}
			catch(Exception exception)
			{
				
                Debug.WriteLine(path);
				Debug.WriteLine(exception);
			}
			finally
            {
				if(file != null)
                {
					file.Close();
                }
            }

			return result;
		}

		public static void SaveSpecIfToFile<T>(T data, string path)
		{
			StreamWriter sw = new StreamWriter(path);
			JsonWriter writer = new JsonTextWriter(sw)
			{
				Formatting = Formatting.Indented
			};

			JsonSerializer serializer = new JsonSerializer()
			{
				NullValueHandling = NullValueHandling.Ignore,
				//TypeNameHandling = TypeNameHandling.All,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,

			};

			serializer.Serialize(writer, data);

			writer.Flush();
			writer.Close();

		}
	}
}
