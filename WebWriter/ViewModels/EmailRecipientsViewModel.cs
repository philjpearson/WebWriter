//
//	Last mod:	11 November 2025 16:46:54
//
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Catel.Collections;
using Catel.MVVM;
using WebWriter.Models;

namespace WebWriter.ViewModels;

public class EmailRecipientsViewModel : ViewModelBase
	{
	List<(string Name, string EmailAddress)> defaultAddresses = [
		("Bland, Pauline", "p.a.bland@btinternet.com"),
		("Ball, Judith", "judithball123@hotmail.com"),
		("Johanesen, Mary", "mary.joh@hotmail.com"),
		("Macmillan, Jennie", "jenniemac58@gmail.com"),
		("Elisabeth Pearson", "lispearson@gmail.com"),
		("Pearson, Phil", "phil@realworldsoftware.co.uk"),
		("Robinson, Jon", "jonrob@gmail.com"),
		("Andrew White", "andywhite2591@gmail.com"),
		("White, Jacinth", "jacinthwhite1@gmail.com"),
		("Woodcock, Ali", "thepetalqueen@yahoo.co.uk"),
		("Woodcock, Dan", "woodcockjdaniel@outlook.com"),
		("Woodcock, Gina", "gina.woodcock@hotmail.co.uk"),
		("Woodcock, Mark", "leonsgrandad@gmail.com"),
		("Nichols, Andrew", "andrewnichols2003@yahoo.co.uk")
		];

	public EmailRecipientsViewModel()
		{
		RecipientAddresses = [];
		var recipients = Properties.Settings.Default.EmailRecipients;
		if (recipients?.Count > 0)
			{
			foreach (var recip in recipients)
				{
				if (recip is not null)
					{
					var bits = recip.Split('|');
					if (bits.Length == 2 && bits[0] is not null && bits[1] is not null)
						{
						RecipientAddresses.Add(new NameAndEmail(bits[0], bits[1]));
						}
					}
				}
			}
		else
			{
			foreach (var item in defaultAddresses)
				{
				RecipientAddresses.Add(new NameAndEmail(item.Name, item.EmailAddress));
				}
			RecipientAddresses.Sort((a, b) => string.Compare(a.Name, b.Name));
			}
		}

	public override string Title { get { return "Email Recipients"; } }

	[Model]
	public ObservableCollection<NameAndEmail> RecipientAddresses { get; set; }

	protected override Task<bool> SaveAsync()
		{
		var addresses = new System.Collections.Specialized.StringCollection();
		foreach (var addr in RecipientAddresses)
			{
			addresses.Add($"{addr.Name}|{addr.EmailAddress}");
			}
		Properties.Settings.Default.EmailRecipients = addresses;
		Properties.Settings.Default.Save();
		return base.SaveAsync();
		}
	}
