//
//	Last mod:	04 February 2025 12:08:00
//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Catel.Data;
using Catel.Runtime.Serialization;

namespace WebWriter.Models
	{
	[Serializable]
	public class GalleryModel : SavableModelBase<GalleryModel>
		{
		ChangeNotificationWrapper? wrapper;

		public GalleryModel()
			{
			Videos = new ObservableCollection<VideoModel>();
			wrapper = new ChangeNotificationWrapper(Videos);
			wrapper.CollectionChanged += Wrapper_CollectionChanged;
			wrapper.CollectionItemPropertyChanged += Wrapper_CollectionItemPropertyChanged;
			}

		public enum ChangeType { None, Edit, Add, Delete };
		Dictionary<VideoModel, ChangeType> changeDictionary = new Dictionary<VideoModel, ChangeType>();

		//public static GalleryModel Load(string filePath)
		//	{
		//	GalleryModel gallery = null;

		//	using (FileStream stream = new FileStream(filePath, FileMode.Open))
		//		{
		//		gallery = Load(stream, SerializationFactory.GetXmlSerializer());
		//		}
		//	return gallery;
		//	}

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

		public ReadOnlyDictionary<VideoModel, ChangeType> GetChanges()
			{
			return new ReadOnlyDictionary<VideoModel, ChangeType>(changeDictionary);
			}

		public void ResetChanges()
			{
			changeDictionary.Clear();
			}

		public void Sort()
			{
			Videos = new ObservableCollection<VideoModel>(Videos.OrderByDescending(v => v.Date));
			}

		private void CheckDuplicateTags()
			{
			for (int anchor = 0; anchor < Videos.Count - 1; anchor++)
				{
				Videos[anchor].HasDuplicateTag = false;
				for (int comp = anchor + 1; comp < Videos.Count; comp++)
					{
					if (Videos[anchor].Tag.Equals(Videos[comp].Tag, StringComparison.InvariantCultureIgnoreCase))
						{
						Videos[anchor].HasDuplicateTag = true;
						Videos[comp].HasDuplicateTag = true;
						}
					}
				}
			}

		private void Wrapper_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
			{
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && e.NewItems is not null)
				{
				foreach (VideoModel item in e.NewItems)
					{
					changeDictionary.Add(item, ChangeType.Add);
					}
				}
			else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && e.OldItems is not null)
				{
				foreach (VideoModel item in e.OldItems)
					{
					changeDictionary.Remove(item);
					changeDictionary.Add(item, ChangeType.Delete);
					}
				}
			CheckDuplicateTags();
			}

		private void Wrapper_CollectionItemPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
			{
			if (sender is VideoModel video
				&& new string[] { nameof(VideoModel.Date),
													nameof(VideoModel.Title),
													nameof(VideoModel.Tag),
													nameof(VideoModel.Link),
													nameof(VideoModel.Speaker),
													nameof(VideoModel.Ecclesia),
													nameof(VideoModel.Details) }.Contains(e.PropertyName))
				{
				if (!changeDictionary.ContainsKey(video))
					changeDictionary.Add(video, ChangeType.Edit);
				if (e.PropertyName == nameof(VideoModel.Tag))
					CheckDuplicateTags();
				}
			}
		}
	}
