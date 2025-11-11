//
//	Last mod:	04 February 2025 16:38:57
//
using System;
using System.Runtime.Serialization;
using System.Windows;
using Catel.Data;

namespace WebWriter.Models
	{
	public enum VideoSize
		{
		NotSet, Size852x480, Size640x360, Size160x90
		}

	[Serializable]
	public class VideoModel : ModelBase
		{
		public VideoModel() { }

		/// <summary>
		/// Initializes a new object based on <see cref="SerializationInfo"/>.
		/// </summary>
		/// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
		/// <param name="context"><see cref="StreamingContext"/>.</param>
		protected VideoModel(SerializationInfo info, StreamingContext context)
				//: base(info, context)
			{
			}

		public int Id { get; set; }

		public string Title { get; set; } = string.Empty;

		public string Link { get; set; } = string.Empty;

		public string Tag { get; set; } = string.Empty;

		public DateTime Date { get; set; }

		public bool IsBibleHour { get; set; }

		public VideoSize Size { get; set; }

		public string Speaker { get; set; } = string.Empty;

		public string Ecclesia { get; set; } = string.Empty;

		public string Details { get; set; } = string.Empty;

		public bool Publish { get; set; }

		public bool HasDuplicateTag { get; set; }

		public Size GetSize()
			{
			Size size = new Size();

			switch (this.Size)
				{
			case VideoSize.NotSet:
				break;

			case VideoSize.Size852x480:
				size.Width = 852;
				size.Height = 480;
				break;

			case VideoSize.Size640x360:
				size.Width = 640;
				size.Height = 360;
				break;

			case VideoSize.Size160x90:
				size.Width = 160;
				size.Height = 90;
				break;
				}
			return size;
			}
		}
	}
