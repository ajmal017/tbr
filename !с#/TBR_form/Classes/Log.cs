﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Windows.Forms;

namespace TBR_form.Classes
{
	/**
	 * Logging class which offers messages retrival from MySql database.
	 * Class is used as a communication service provider for C# Visual Studio projects located in the same solution
	 * which can not be linked together due to circular link restriction. 
	 * This class can pe copied to other projects in order to send messages (insert) to the DB which will be read by the
	 * main class, putputed or used as triggers for business logic.
	 */
	class Log
	{
		// DB Credentials
		private const string SERVER = "127.0.0.1";
		private const string DATABASE = "tut_db";
		private const string UID = "slinger";
		private const string PASSWORD = "659111";
		private static MySqlConnection dbConn; // MySql connection 

		// DB record feilds
		public DateTime Date { get; private set; }
		public string Source { get; private set; }
		public string Message { get; private set; }
		public string Color { get; private set; }
		public bool IsNew { get; private set; }

		// Class constructor
		public Log(DateTime d, string s, string m, string c, bool i)
		{
			Date = d;
			Source = s;
			Message = m;
			Color = c;
			IsNew = i;
			Console.WriteLine("Log.cs InitializeDB()"); 
			InitializeDB(); // ERROR goes from here
		}

		public static void InitializeDB()
		{
			MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
			builder.Server = SERVER;
			builder.UserID = UID;
			builder.Password = PASSWORD;
			builder.Database = DATABASE;

			string connString = builder.ToString();
			Console.WriteLine("Log.cs Connection string :" + builder.ToString());
			builder = null;

			dbConn = new MySqlConnection(connString);

			//DBConnectionOpen(); // DB Connection open 
		}

		public static List<Log> GetLogs() // List with custom class type - Log
		{
			List<Log> logs = new List<Log>();

			string query = "SELECT * from logs";

			MySqlCommand cmd = new MySqlCommand(query, dbConn);

			MySqlDataReader reader = cmd.ExecuteReader(); // Create reader and get all recoeds from DB
			while (reader.Read())
			{
				DateTime date = (DateTime)reader["date"]; // Cast it to integer. (int)reader["id"]
				string source = reader["source"].ToString();
				string message = reader["message"].ToString();
				string color = reader["color"].ToString();
				bool isNew = (bool)reader["is_new"]; // Type cast

				Log log = new Log(date, source, message, color, isNew);
				logs.Add(log);
			}

			return logs;
		}

		public static List<Log> GetNewLogs() // List with custom class type - DBLogging
		{
			List<Log> logs = new List<Log>();

			using (var conn = new MySqlConnection("server=127.0.0.1;user id=slinger;password=659111;database=tut_db"))
			{
				if (conn.State == System.Data.ConnectionState.Closed)
				{
					Console.WriteLine("Connection state: " + conn.State + " Opening connection!");
					conn.Open(); // If no connection to DB
				}
				else
				{
					Console.WriteLine("Connection state: " + conn.State + " Connection open, no need to connect");
				}

				string sql = "SELECT * from logs WHERE is_new = '1'";
				MySqlCommand cmd2 = new MySqlCommand(sql, conn);
				cmd2.ExecuteNonQuery();


				MySqlDataReader reader = cmd2.ExecuteReader(); // Create reader and get all recoeds from DB
				while (reader.Read())
				{
					DateTime date = (DateTime)reader["date"]; // Cast it to integer. (int)reader["id"]
					string source = reader["source"].ToString();
					string message = reader["message"].ToString();
					string color = reader["color"].ToString();
					bool isNew = (bool)reader["is_new"]; // Type cast

					Log l = new Log(date, source, message, color, isNew);
					logs.Add(l); // And new value to the list. Then this list will be returned

					// get id of the current record 
					// update is_new flag from 1 to 0
					int id = (int)reader["id"]; // Type cast
				}

				//conn.Close(); // No need to close the connection. It closes at the disposal 
			}

			return logs;
		}

		public static void Insert(DateTime d, String s, String m, string c) // Date, source, message
		{

			string query = string.Format("INSERT INTO logs(date, source, message, color) VALUES ('{0}', '{1}', '{2}', '{3}' )", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), s, m, c);
			
			MySqlCommand cmd = new MySqlCommand(query, dbConn);

			// https://dev.mysql.com/doc/connector-net/en/connector-net-tutorials-sql-command.html 

			using (var conn = new MySqlConnection("server=127.0.0.1;user id=slinger;password=659111;database=tut_db"))
			{
				if (conn.State == System.Data.ConnectionState.Closed)
				{
					Console.WriteLine("Connection state: " + conn.State + "Opening connection!");
					conn.Open(); // If no connection to DB
				}
				else
				{
					Console.WriteLine("Connection state: " + conn.State + " Connection open, no need to connect");
				}

				string sql = "INSERT INTO logs(date, source, message) VALUES ('" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" + s + "', '" + m + "' )";
				MySqlCommand cmd2 = new MySqlCommand(sql, conn);
				cmd2.ExecuteNonQuery();

				//conn.Close(); // No need to close the connection. It closes at the disposal 
			}

		}



	}
}
