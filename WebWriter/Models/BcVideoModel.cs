//
//	Last mod:	28 July 2026 15:52:10
//
using System;
using System.Runtime.Serialization;
using Catel.Data;

namespace WebWriter.Models
	{
	[Serializable]
	public class BcVideoModel : ModelBase
		{
		public BcVideoModel() 
			{
			Link = string.Empty;
			}

		/// <summary>
		/// Initializes a new object based on <see cref="SerializationInfo"/>.
		/// </summary>
		/// <param name="info"><see cref="SerializationInfo"/> that contains the information.</param>
		/// <param name="context"><see cref="StreamingContext"/>.</param>
		protected BcVideoModel(SerializationInfo info, StreamingContext context)
			{
			Link = string.Empty;
			}

		public string Key { get; set; } = string.Empty;

		public string Title { get; set; } = string.Empty;

		public string Link { get; set; }

		public DateTime Date { get; set; }

		public string Speaker { get; set; } = string.Empty;

		public string Ecclesia { get; set; } = string.Empty;

		private void OnLinkChanged()
			{
			if (!string.IsNullOrWhiteSpace(Link))
				{
				Key = Link[(Link.LastIndexOf('/') + 1)..];
				if (Key.Contains('?'))
					Key = Key[(Key.LastIndexOf('=') + 1)..];
				}
			}
		}
	}
