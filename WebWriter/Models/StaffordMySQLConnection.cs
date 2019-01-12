//
//	Last mod:	06 January 2019 18:18:14
//
using System;
// using Devart.Data.MySql;
using MySql.Data.MySqlClient;

namespace WebWriter.Models
	{
	class StaffordMySQLConnection
		{
		private StaffordMySQLConnection()
			{
			}

		private string serverTunnel = "127.0.0.1";
//		private string server = "22.mysql.servage.net"; // "remote-mysql4.servage.net";
		private string userName = "1007246_jfkgga5j"; // "stafford";

		public string DatabaseName { get; set; } = "1007246-stafford"; // "stafford";

		public string Password { get; set; } = "no2337ttU8"; // "cKjgqVsjUvsW6kNbi0qa";

		public MySqlConnection Connection { get; private set; } = null;

		private static StaffordMySQLConnection _instance = null;
		public static StaffordMySQLConnection Instance()
			{
			if (_instance == null)
				_instance = new StaffordMySQLConnection();
			return _instance;
			}

		public bool IsConnect(bool useTunnel = true)
			{
			bool result = true;
			if (Connection == null)
				{
				result = false;
				if (!String.IsNullOrEmpty(DatabaseName) && (!useTunnel || BudeTunnel.Open()))
					{
					//					string connstring = string.Format($"Server={serverTunnel}; Port=3307; database={databaseName}; UID={userName}; password={Password}");
					string port = (useTunnel ? BudeTunnel.TunnelPort : 3307).ToString();

					string connstring = string.Format($"Host={serverTunnel}; Port={port}; database={DatabaseName}; User={userName}; password={Password}");
					Connection = new MySqlConnection(connstring);
					Connection.Open();
					result = true;
					}
				}
			return result;
			}

		public void Close()
			{
			Connection.Close();
			Connection = null;
			BudeTunnel.Close();
			}
		}
	}
