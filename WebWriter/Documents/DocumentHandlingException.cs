//
//	Last mod:	02 January 2023 10:45:50
//
using System;

namespace WebWriter.Documents
	{
	public class DocumentHandlingException : Exception
		{
		public DocumentHandlingException(string message, Exception innerException = null)
			: base(message, innerException)
			{
			}
		}
	}
