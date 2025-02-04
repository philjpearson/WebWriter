//
//	Last mod:	04 February 2025 13:50:27
//
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

#nullable enable

namespace WebWriter.Utilities
	{
	class CursorOverride : IDisposable
		{
		private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

		private static Stack<Cursor?> stack = new();

		public CursorOverride(string changeToCursorResourceName)
			: this(LoadFromResource(changeToCursorResourceName))
			{
			}

		public CursorOverride(Cursor? changeToCursor)
      {
      stack.Push(changeToCursor);

      if (Mouse.OverrideCursor != changeToCursor)
        Mouse.OverrideCursor = changeToCursor;
      }

    public void Dispose()
      {
      stack.Pop();

      Cursor? cursor = stack.Count > 0 ? stack.Peek() : null;

      if (cursor != Mouse.OverrideCursor)
        Mouse.OverrideCursor = cursor;
      }

		public static Cursor? LoadFromResource(string resourceName)
			{
			Cursor? cursor = null;

			try
				{
				var uri = new Uri($"pack://application:,,,/Resources/{resourceName}", UriKind.RelativeOrAbsolute);
				var stream = Application.GetResourceStream(uri).Stream;
				cursor = new Cursor(stream);
				}
			catch (Exception ex)
				{
				logger.Error("Failed to load cursor from resources: {0}", ex.Message);
				}
			return cursor;
			}
		}
	}
