//
//	Last mod:	05 February 2025 14:24:28
//
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WebWriter.Models;

public static class StaticProperties
	{
	private static bool editingEnabled = false;

	public static event EventHandler<PropertyChangedEventArgs>? StaticPropertyChanged;

	public static bool EditingEnabled
		{
		get { return editingEnabled; }
		set
			{
			if (editingEnabled != value)
				{
				editingEnabled = value;
				NotifyStaticPropertyChanged();
				}
			}
		}

	private static void NotifyStaticPropertyChanged([CallerMemberName] string propertyName = "")
		{
		StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));
		}
	}
