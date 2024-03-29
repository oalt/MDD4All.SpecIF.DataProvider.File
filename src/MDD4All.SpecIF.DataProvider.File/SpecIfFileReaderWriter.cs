﻿/*
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
		public static DataModels.SpecIF ReadDataFromSpecIfFile(string path)
		{
			DataModels.SpecIF result = null;

			StreamReader file = null;

			try
			{

				file = new StreamReader(path);

				JsonSerializer serializer = new JsonSerializer();

				result = (DataModels.SpecIF)serializer.Deserialize(file, typeof(DataModels.SpecIF));
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

		public static void SaveSpecIfToFile(DataModels.SpecIF data, string path)
		{
			StreamWriter sw = new StreamWriter(path);
			JsonWriter writer = new JsonTextWriter(sw)
			{
				Formatting = Formatting.Indented
			};

			JsonSerializer serializer = new JsonSerializer()
			{
				NullValueHandling = NullValueHandling.Ignore
			};

			serializer.Serialize(writer, data);

			writer.Flush();
			writer.Close();

		}
	}
}
