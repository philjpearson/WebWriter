//
//	Last mod:	04 February 2025 11:32:53
//

namespace WebWriter.Models
	{
	internal class RecordingToAdd
		{
		public uint TypeId { get; set; }

		public string FilePath { get; set; } = string.Empty;

		public string Text { get; set; } = string.Empty;

		public string Speaker { get; set; } = string.Empty;

		public string Ecclesia { get; set; } = string.Empty;
		}
	}
