//
//	Last mod:	11 January 2017 13:46:15
//
using MySql.Data.MySqlClient;
using System;

namespace WebWriter.Models
	{
	class StaffordMySQLConnection
		{
			private StaffordMySQLConnection()
				{
				}

			private string server = "remote-mysql4.servage.net";
			private string databaseName = "stafford";

			public string DatabaseName
				{
				get { return databaseName; }
				set { databaseName = value; }
				}

			public string Password { get; set; } = "cKjgqVsjUvsW6kNbi0qa";

			private MySqlConnection connection = null;

			public MySqlConnection Connection
				{
				get { return connection; }
				}

			private static StaffordMySQLConnection _instance = null;
			public static StaffordMySQLConnection Instance()
				{
				if (_instance == null)
					_instance = new StaffordMySQLConnection();
				return _instance;
				}

			public bool IsConnect()
				{
				bool result = true;
				if (Connection == null)
					{
					if (String.IsNullOrEmpty(databaseName))
						result = false;
					string connstring = string.Format($"Server={server}; database={databaseName}; UID=stafford; password={Password}");
					connection = new MySqlConnection(connstring);
					connection.Open();
					result = true;
					}

				return result;
				}

			public void Close()
				{
				connection.Close();
				}
			}
		}
