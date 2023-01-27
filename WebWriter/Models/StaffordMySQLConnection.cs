//
//	Last mod:	05 January 2023 13:02:41
//
using System;
using System.Windows;
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
		private string userName = "u880159079_phil";

		public string DatabaseName { get; set; } = "u880159079_stafford";

		public string Password { get; set; } = "z6ZjgSn4tfkM_nD";

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
				try
					{
					if (!String.IsNullOrEmpty(DatabaseName) && (!useTunnel || BudeTunnel.Open()))
						{
						string port = (useTunnel ? BudeTunnel.TunnelPort : 3307).ToString();

						string connstring = string.Format($"Host={serverTunnel}; Port={port}; database={DatabaseName}; User={userName}; password={Password}");
						Connection = new MySqlConnection(connstring);
						Connection.Open();
						result = true;
						}
					}
				catch (Exception ex)
					{
					MessageBox.Show(ex.Message, "StaffordMySQLConnection");
					}
				}
			return result;
			}

		public void Close()
			{
			Connection?.Close();
			Connection = null;
			BudeTunnel.Close();
			}
		}
	}
