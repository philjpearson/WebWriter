//
//	Last mod:	27 July 2026 20:58:02
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
	public class BcGalleryModel : SavableModelBase<BcGalleryModel>
		{
		private readonly ChangeNotificationWrapper? wrapper;

		public BcGalleryModel()
			{
			wrapper = new ChangeNotificationWrapper(Videos);
			wrapper.CollectionChanged += Wrapper_CollectionChanged;
			wrapper.CollectionItemPropertyChanged += Wrapper_CollectionItemPropertyChanged;
			}

		public enum ChangeType { None, Edit, Add, Delete };

		private readonly Dictionary<BcVideoModel, ChangeType> changeDictionary = [];

		public bool HasChanges { get { return changeDictionary.Count > 0; } }

		/// <summary>
		/// Initializes a new object based on <see cref="SerializationInfo"/>.
		/// </summary>
		/// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
		/// <param name="context"><see cref="StreamingContext"/>.</param>
		protected BcGalleryModel(SerializationInfo info, StreamingContext context)
			{
			}

		public ObservableCollection<BcVideoModel> Videos { get; set; } = [];

		public ReadOnlyDictionary<BcVideoModel, ChangeType> GetChanges()
			{
			return new ReadOnlyDictionary<BcVideoModel, ChangeType>(changeDictionary);
			}

		public void ResetChanges()
			{
			changeDictionary.Clear();
			}

		public void Sort()
			{
			Videos = new ObservableCollection<BcVideoModel>(Videos.OrderByDescending(v => v.Date));
			}

		private void Wrapper_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
			{
			if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && e.NewItems is not null)
				{
				foreach (BcVideoModel item in e.NewItems)
					{
					changeDictionary.Add(item, ChangeType.Add);
					}
				}
			else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && e.OldItems is not null)
				{
				foreach (BcVideoModel item in e.OldItems)
					{
					changeDictionary.Remove(item);
					changeDictionary.Add(item, ChangeType.Delete);
					}
				}
			}

		private void Wrapper_CollectionItemPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
			{
			if (sender is BcVideoModel video
				&& new string[] { nameof(BcVideoModel.Date),
													nameof(BcVideoModel.Title),
													nameof(BcVideoModel.Link),
													nameof(BcVideoModel.Speaker),
													nameof(BcVideoModel.Ecclesia) }.Contains(e.PropertyName))
				{
				changeDictionary.TryAdd(video, ChangeType.Edit);
				}
			}
		}
	}
