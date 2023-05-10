using MitoBDO.Model;
using System.Data.SQLite;
using System.Reactive.Joins;
using System.Security.Claims;

namespace MitoBDO.Services
{
	public class DatabaseService
	{
		private const string MitoDBPath = @"Data Source=Data/MitoDB.db";
		private SQLiteConnection MitoDB = new SQLiteConnection(MitoDBPath);

		public DatabaseService()
		{
			InitalizeDatabase();
		}

		private void InitalizeDatabase()
		{
			MitoDB.Open();
			using (SQLiteCommand command = MitoDB.CreateCommand())
			{
				command.CommandText = "CREATE TABLE IF NOT EXISTS MarketAlarm(code INT, enLevel INT, userID VARCHAR(20))";
				command.ExecuteNonQuery();
			}
		}

		private bool CheckAlarmExist(MarketAlarm alarm)
		{
			using (SQLiteCommand command = MitoDB.CreateCommand())
			{
				var ret = new List<MarketAlarm>();
				command.CommandText = "SELECT * FROM MarketAlarm WHERE code like @cd AND enLevel like @lv AND userID like @id";
				command.Parameters.AddWithValue("@cd", $"%{alarm.itemCode}%");
				command.Parameters.AddWithValue("@lv", $"%{alarm.enLevel}%");
				command.Parameters.AddWithValue("@id", $"%{alarm.userID}%");
				var name = command.ExecuteScalar();

				return name is not null;
			}
		}

		public bool InsertAlarmTable(MarketAlarm alarm)
		{
			if (!CheckAlarmExist(alarm))
			{
				using (SQLiteCommand command = MitoDB.CreateCommand())
				{
					command.CommandText
						= $"INSERT INTO MarketAlarm VALUES ({alarm.itemCode}, {alarm.enLevel}, {alarm.userID}); ";
					command.ExecuteNonQuery();
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool DeleteAlarmTable(MarketAlarm alarm)
		{
			if (CheckAlarmExist(alarm))
			{
				using (SQLiteCommand command = MitoDB.CreateCommand())
				{
					var ret = new List<MarketAlarm>();
					command.CommandText = "DELETE FROM MarketAlarm WHERE code like @cd AND enLevel like @lv AND userID like @id";
					command.Parameters.AddWithValue("@cd", $"%{alarm.itemCode}%");
					command.Parameters.AddWithValue("@lv", $"%{alarm.enLevel}%");
					command.Parameters.AddWithValue("@id", $"%{alarm.userID}%");
					using (SQLiteDataReader rdr = command.ExecuteReader())
					{
						while (rdr.Read())
						{
							try
							{
								var code = ulong.Parse(rdr["code"].ToString());
								var level = ulong.Parse(rdr["enLevel"].ToString());

								ret.Add(new MarketAlarm(code, level, rdr["userID"].ToString()));
							}
							catch (Exception e)
							{
								continue;
							}
						}
					}
					return true;
				}
			}
			else
			{
				return false;
			}
		}

		public List<MarketAlarm> ReadMyAlarmTables(string userID)
		{
			var ret = new List<MarketAlarm>();
			using (SQLiteCommand command = MitoDB.CreateCommand())
			{
				command.CommandText = "SELECT * FROM MarketAlarm WHERE userID like @id";
				command.Parameters.AddWithValue("@id", $"%{userID}%");
				using (SQLiteDataReader rdr = command.ExecuteReader())
				{
					while (rdr.Read())
					{
						try
						{
							var code = ulong.Parse(rdr["code"].ToString());
							var level = ulong.Parse(rdr["enLevel"].ToString());

							ret.Add(new MarketAlarm(code, level, rdr["userID"].ToString()));
						}
						catch (Exception e)
						{
							continue;
						}
					}
				}
			}
			return ret;
		}

		public List<MarketAlarm> ReadAllAlarmTables()
		{
			var ret = new List<MarketAlarm>();
			using (SQLiteCommand command = MitoDB.CreateCommand())
			{
				command.CommandText = "SELECT * FROM MarketAlarm";
				using (SQLiteDataReader rdr = command.ExecuteReader())
				{
					while (rdr.Read())
					{
						try
						{
							var code = ulong.Parse(rdr["code"].ToString());
							var level = ulong.Parse(rdr["enLevel"].ToString());

							ret.Add(new MarketAlarm(code, level, rdr["userID"].ToString()));
						}
						catch(Exception e) 
						{
							continue;
						}
					}
				}
			}
			return ret;
		}
	}
}
