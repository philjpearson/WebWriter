//
//	Last mod:	27 January 2024 20:27:01
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
    static Stack<Cursor?> stack = new Stack<Cursor?>();

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
				}
			return cursor;
			}
		}
	}
