//
//	Last mod:	02 January 2023 11:06:43
//
using System;
using System.IO;
using System.Reflection;
using MigraDoc.DocumentObjectModel;

namespace WebWriter.Documents
	{
	public static class MigraDocHelpers
		{
		static string MigraDocFilenameFromByteArray(byte[] image)
			{
			return "base64:" + Convert.ToBase64String(image);
			}

		static byte[] LoadImageFromResource(Assembly assy, string name)
			{
			using (Stream stream = assy.GetManifestResourceStream(name))
				{
				if (stream == null)
					throw new ArgumentException("No resource with name " + name);

				int count = (int)stream.Length;
				byte[] data = new byte[count];
				stream.Read(data, 0, count);
				return data;
				}
			}

		static byte[] LoadImageFromResource(string name)
			{
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
				{
				if (stream == null)
					throw new ArgumentException("No resource with name " + name);

				int count = (int)stream.Length;
				byte[] data = new byte[count];
				stream.Read(data, 0, count);
				return data;
				}
			}

		public static MigraDoc.DocumentObjectModel.Shapes.Image AddImageFromResource(this HeaderFooter headerFooter, Assembly assy, string resourceName)
			{
			return headerFooter.AddImage(MigraDocFilenameFromByteArray(LoadImageFromResource(assy, resourceName)));
			}

		public static MigraDoc.DocumentObjectModel.Shapes.Image AddImageFromResource(this HeaderFooter headerFooter, string resourceName)
			{
			return headerFooter.AddImage(MigraDocFilenameFromByteArray(LoadImageFromResource(resourceName)));
			}

		/// <summary>
		/// Get the list of all emdedded resources in the assembly.
		/// </summary>
		/// <returns>An array of fully qualified resource names</returns>
		public static string[] GetEmbeddedResourceNames()
			{
			return Assembly.GetExecutingAssembly().GetManifestResourceNames();
			}
		}
	}
