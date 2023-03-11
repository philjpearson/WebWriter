//
//	Last mod:	27 January 2023 16:23:08
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
		private string server = "sql495.main-hosting.eu";
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

		public static implicit operator MySqlConnection(StaffordMySQLConnection con) => con.Connection;

		public bool IsConnect(bool useTunnel = false)
			{
			bool result = true;
			if (Connection == null)
				{
				result = false;
				try
					{
					if (!string.IsNullOrEmpty(DatabaseName) && (!useTunnel || BudeTunnel.Open()))
						{
						string port = (useTunnel ? BudeTunnel.TunnelPort : 3306).ToString();
						string srv = useTunnel ? serverTunnel : server;
						string connstring = string.Format($"Server={srv}; Port={port}; Database={DatabaseName}; Uid={userName}; Pwd={Password}");
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
