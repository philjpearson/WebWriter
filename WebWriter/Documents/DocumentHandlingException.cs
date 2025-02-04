//
//	Last mod:	04 February 2025 11:21:37
//
using System;

namespace WebWriter.Documents
	{
	public class DocumentHandlingException : Exception
		{
		public DocumentHandlingException(string message, Exception? innerException = null)
			: base(message, innerException)
			{
			}
		}
	}
