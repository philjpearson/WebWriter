//
//	Last mod:	11 November 2025 16:26:19
//
namespace WebWriter.Models;

public class NameAndEmail(string name, string emailAddress)
	{
	public string Name { get; set; } = name;
	public string EmailAddress { get; set; } = emailAddress;
	}
