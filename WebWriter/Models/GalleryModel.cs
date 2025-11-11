//
//	Last mod:	05 February 2025 15:11:31
//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Catel.Data;

namespace WebWriter.Models
	{
	[Serializable]
	public class GalleryModel : SavableModelBase<GalleryModel>
		{
		private readonly ChangeNotificationWrapper? wrapper;

		public GalleryModel()
			{
			Videos = [];
			wrapper = new ChangeNotificationWrapper(Videos);
			wrapper.CollectionChanged += Wrapper_CollectionChanged;
			wrapper.CollectionItemPropertyChanged += Wrapper_CollectionItemPropertyChanged;
			}

		public enum ChangeType { None, Edit, Add, Delete };

		private readonly Dictionary<VideoModel, ChangeType> changeDictionary = [];

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
				//: base(info, context)
			{
			}

		public ObservableCollection<VideoModel> Videos { get; set; }

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
				changeDictionary.TryAdd(video, ChangeType.Edit);
				if (e.PropertyName == nameof(VideoModel.Tag))
					CheckDuplicateTags();
				}
			}
		}
	}
