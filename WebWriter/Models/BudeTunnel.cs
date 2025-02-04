//
//	Last mod:	04 February 2025 11:21:36
//
using System;
using System.Net;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace WebWriter.Models
	{
	public static class BudeTunnel
		{
		private static SshClient? client;

		public static uint TunnelPort { get; private set; }

		public static bool Open()
			{
			bool result = false;

			var connectionInfo = new PasswordConnectionInfo("153.92.7.25", "u880159079", "5Alvat10n!ssh")
				{
				Timeout = TimeSpan.FromSeconds(30)
				};

			client = new SshClient("153.92.7.25", 65002, "u880159079", "5Alvat10n!ssh"); // new SshClient(connectionInfo);
			try
				{
				Console.WriteLine("Trying SSH connection...");
				client.Connect();
				if (client.IsConnected)
					{
					var portFwld = new ForwardedPortLocal("127.0.0.1", /*3307,*/ "127.0.0.1", 3307);
					client.AddForwardedPort(portFwld);
					portFwld.Start();
					if (portFwld.IsStarted)
						{
						TunnelPort = portFwld.BoundPort;
						result = true;
						}
					else
						{
						Console.WriteLine("Port forwarding has failed.");
						}
					}
				else
					{
					Console.WriteLine("SSH connection has failed: {0}", client.ConnectionInfo.ToString());
					}
				}
			catch (SshException e)
				{
				Console.WriteLine("SSH client connection error: {0}", e.Message);
				}
			catch (System.Net.Sockets.SocketException e)
				{
				Console.WriteLine("Socket connection error: {0}", e.Message);
				}

			if (!result)
				{
				Close();
				}
			return result;
			}

		public static void Close()
			{
			client?.Dispose();
			client = null;
			}
		}
	}
