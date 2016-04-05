//
//	Last mod:	14 March 2015 11:54:51
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
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
				: base(info, context)
			{
			}

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public string Title
			{
			get { return GetValue<string>(TitleProperty); }
			set { SetValue(TitleProperty, value); }
			}

		/// <summary>
		/// Register the Title property so it is known in the class.
		/// </summary>
		public static readonly PropertyData TitleProperty = RegisterProperty("Title", typeof(string), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public string Link
			{
			get { return GetValue<string>(LinkProperty); }
			set { SetValue(LinkProperty, value); }
			}

		/// <summary>
		/// Register the Link property so it is known in the class.
		/// </summary>
		public static readonly PropertyData LinkProperty = RegisterProperty("Link", typeof(string), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public string Tag
			{
			get { return GetValue<string>(TagProperty); }
			set { SetValue(TagProperty, value); }
			}

		/// <summary>
		/// Register the Tag property so it is known in the class.
		/// </summary>
		public static readonly PropertyData TagProperty = RegisterProperty("Tag", typeof(string), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public DateTime Date
			{
			get { return GetValue<DateTime>(DateProperty); }
			set { SetValue(DateProperty, value); }
			}

		/// <summary>
		/// Register the Date property so it is known in the class.
		/// </summary>
		public static readonly PropertyData DateProperty = RegisterProperty("Date", typeof(DateTime), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public bool IsBibleHour
			{
			get { return GetValue<bool>(IsBibleHourProperty); }
			set { SetValue(IsBibleHourProperty, value); }
			}

		/// <summary>
		/// Register the IsBibleHour property so it is known in the class.
		/// </summary>
		public static readonly PropertyData IsBibleHourProperty = RegisterProperty("IsBibleHour", typeof(bool), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public VideoSize Size
			{
			get { return GetValue<VideoSize>(SizeProperty); }
			set { SetValue(SizeProperty, value); }
			}

		/// <summary>
		/// Register the Size property so it is known in the class.
		/// </summary>
		public static readonly PropertyData SizeProperty = RegisterProperty("Size", typeof(VideoSize), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public string Speaker
			{
			get { return GetValue<string>(SpeakerProperty); }
			set { SetValue(SpeakerProperty, value); }
			}

		/// <summary>
		/// Register the Speaker property so it is known in the class.
		/// </summary>
		public static readonly PropertyData SpeakerProperty = RegisterProperty("Speaker", typeof(string), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public string Ecclesia
			{
			get { return GetValue<string>(EcclesiaProperty); }
			set { SetValue(EcclesiaProperty, value); }
			}

		/// <summary>
		/// Register the Ecclesia property so it is known in the class.
		/// </summary>
		public static readonly PropertyData EcclesiaProperty = RegisterProperty("Ecclesia", typeof(string), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public string Details
			{
			get { return GetValue<string>(DetailsProperty); }
			set { SetValue(DetailsProperty, value); }
			}

		/// <summary>
		/// Register the Details property so it is known in the class.
		/// </summary>
		public static readonly PropertyData DetailsProperty = RegisterProperty("Details", typeof(string), null);

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public bool Publish
			{
			get { return GetValue<bool>(PublishProperty); }
			set { SetValue(PublishProperty, value); }
			}

		/// <summary>
		/// Register the Publish property so it is known in the class.
		/// </summary>
		public static readonly PropertyData PublishProperty = RegisterProperty("Publish", typeof(bool), true);

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
