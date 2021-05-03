//
//	Last mod:	03 May 2021 13:09:45
//
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace WebWriter.Utilities
	{
	class CursorOverride : IDisposable
		{
    static Stack<Cursor> stack = new Stack<Cursor>();

    public CursorOverride(Cursor changeToCursor)
      {
      stack.Push(changeToCursor);

      if (Mouse.OverrideCursor != changeToCursor)
        Mouse.OverrideCursor = changeToCursor;
      }

    public void Dispose()
      {
      stack.Pop();

      Cursor cursor = stack.Count > 0 ? stack.Peek() : null;

      if (cursor != Mouse.OverrideCursor)
        Mouse.OverrideCursor = cursor;
      }
    }
	}
