﻿//
//	Last mod:	04 February 2025 12:07:58
//
namespace WebWriter.Views
	{
	using Catel.Windows;
	using ViewModels;

	public partial class CampaignGalleryView
		{
		public CampaignGalleryView()
				: this(null)
			{ }

		public CampaignGalleryView(CampaignGalleryViewModel? viewModel)
				: base(viewModel)
			{
			InitializeComponent();
			SizeToContent = System.Windows.SizeToContent.Manual;
			}
		}
	}
