using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;
using System.IO;

class Program
{
	static void Main(string[] args) => new Program().Start();

	public static string prefix = "!"; 
	private static DiscordClient _client = new DiscordClient();

	public void Start()
	{
		_client.Log.Message += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

		_client.MessageReceived += async (s, e) =>
		{
			if (e.Message.Text.ToLower() == "!help" && e.Channel.Id != 207785659006451723)
			{
				string warning = "**All commands besides `!help` only work in DM's!**\n";
				string teams = "Joho, Meester, Midge, Midnight, and Yessoan";
				string msg1 = $"To join a team, type `!` followed by one of the living TWOWers names (the living TWOWers are {teams}) - ex `!Meester`\n";
				string msg2 = "To remove yourself from your team, simply type `!Remove`";

				await e.Channel.SendMessage(warning + msg1 + msg2);
			}
			else if (e.Channel.IsPrivate)
			{
				Server htc = _client.GetServer(184755239952318464);
				User user = htc.GetUser(e.User.Id);
				List<Role> teams = new List<Role>();
				var roles = htc.Roles;

				foreach (Role role in roles)
				    if (role.Name.StartsWith("#Team"))
				        teams.Add(role);

				if (e.Message.Text.ToLower() == "!remove")
				{
					var removed = false;
					foreach (Role role in teams)
					{
						if(user.HasRole(role)) {
							removed = true;
						    await user.RemoveRoles(role);
						    System.Threading.Thread.Sleep(500);
						}
					}
					if (removed)
					    await e.Channel.SendMessage("You have successfully been removed from your team.");
					else
					    await e.Channel.SendMessage("You're not on a team!");
				}
				else if (e.Message.Text.ToLower() == "!joho")
				{
					foreach (Role role in teams)
					{
						if(user.HasRole(role)) {
						    await user.RemoveRoles(role);
						    System.Threading.Thread.Sleep(500);
						}
					}
					Role team = htc.FindRoles("#TeamJoho", false).FirstOrDefault();
					await user.AddRoles(team);
					await e.Channel.SendMessage("You have joined #TeamJoho.");
				}
				else if (e.Message.Text.ToLower() == "!meester")
				{
					foreach (Role role in teams)
					{
						if(user.HasRole(role)) {
						    await user.RemoveRoles(role);
						    System.Threading.Thread.Sleep(500);
						}
					}
					Role team = htc.FindRoles("#TeamMeester", false).FirstOrDefault();
					await user.AddRoles(team);
					await e.Channel.SendMessage("You have joined #TeamMeester.");
				}
				else if (e.Message.Text.ToLower() == "!midge")
				{
					foreach (Role role in teams)
					{
						if(user.HasRole(role)) {
						    await user.RemoveRoles(role);
						    System.Threading.Thread.Sleep(500);
						}
					}
					Role team = htc.FindRoles("#TeamMidge", false).FirstOrDefault();
					await user.AddRoles(team);
					await e.Channel.SendMessage("You have joined #TeamMidge.");
				}
				else if (e.Message.Text.ToLower() == "!midnight")
				{
					foreach (Role role in teams)
					{
						if(user.HasRole(role)) {
						    await user.RemoveRoles(role);
						    System.Threading.Thread.Sleep(500);
						}
					}
					Role team = htc.FindRoles("#TeamMidnight", false).FirstOrDefault();
					await user.AddRoles(team);
					await e.Channel.SendMessage("You have joined #TeamMidnight.");
				}
				else if (e.Message.Text.ToLower() == "!yessoan")
				{
					foreach (Role role in teams)
					{
						if(user.HasRole(role)) {
						    await user.RemoveRoles(role);
						    System.Threading.Thread.Sleep(500);
						}
					}
					Role team = htc.FindRoles("#TeamYessoan", false).FirstOrDefault();
					await user.AddRoles(team);
					await e.Channel.SendMessage("You have joined #TeamYessoan.");
				}
			}

		};

		string token = File.ReadAllText("token.txt");
		_client.ExecuteAndWait(async () => {
			await _client.Connect(token, TokenType.Bot);
		});

	}
}
