//
//	Last mod:	14 March 2015 16:44:40
//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Catel.Data;

namespace WebWriter.Models
	{
	[Serializable]
	public class GalleryModel : SavableModelBase<GalleryModel>
		{
		public GalleryModel()
			{
			}

		public GalleryModel(bool initialise)
			{
			if (initialise)
				{
				Videos = new ObservableCollection<VideoModel>();
				}
			}

		/// <summary>
		/// Initializes a new object based on <see cref="SerializationInfo"/>.
		/// </summary>
		/// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
		/// <param name="context"><see cref="StreamingContext"/>.</param>
		protected GalleryModel(SerializationInfo info, StreamingContext context)
				: base(info, context)
			{
			}

		/// <summary>
		/// Gets or sets the property value.
		/// </summary>
		public ObservableCollection<VideoModel> Videos
			{
			get { return GetValue<ObservableCollection<VideoModel>>(VideosProperty); }
			set { SetValue(VideosProperty, value); }
			}

		/// <summary>
		/// Register the Videos property so it is known in the class.
		/// </summary>
		public static readonly PropertyData VideosProperty = RegisterProperty("Videos", typeof(ObservableCollection<VideoModel>), null);

		public void Sort()
			{
			Videos = new ObservableCollection<VideoModel>(Videos.OrderByDescending(v => v.Date));
			}
		}
	}
