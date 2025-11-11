//
//	Last mod:	07 February 2025 16:47:38
//
using System;
using MySqlConnector;

#nullable enable

namespace WebWriter.Models
	{
	class StaffordMySQLConnection : IDisposable
		{
		private string server = "srv583.hstgr.io"; // "srv495.hstgr.io"
		private string userName = "u880159079_phil";
		private const string password = "z6ZjgSn4tfkM_nD";

		public string DatabaseName { get; set; } = "u880159079_stafford";

		public Exception? LastException { get; private set; }

		public StaffordMySQLConnection()
			{
			string connstring = string.Format($"Server={server}; Database={DatabaseName}; Uid={userName}; Pwd={password}");
			Connection = new MySqlConnection(connstring);
			}

		public MySqlConnection Connection { get; private set; }

		public static implicit operator MySqlConnection(StaffordMySQLConnection con) => con.Connection;

		public bool Open()
			{
			try
				{
				Connection?.Open();
				LastException = null;
				return true;
				}
			catch (Exception ex)
				{
				LastException = ex;
				return false;
				}
			}

		public void Close()
			{
			Connection?.Close();
			}

		public void Dispose()
			{
			Close();
			Connection?.Dispose();
			}
		}
	}
