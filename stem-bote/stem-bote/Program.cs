using System;
using System.IO;
using System.Linq;
using Discord;
using System.Collections.Generic;
using System.Timers;

namespace stembote
{
	class stembote
	{
		static void Main(string[] args) => new stembote().Start();

		public static DiscordClient _client = new DiscordClient();

		public void Start()
		{
			_client.Log.Message += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

			_client.MessageReceived += async (s, e) =>
			{
				string p = "!"; // define prefix variable

				bool isCmd = e.Message.Text.StartsWith(p); // test if message starts with the prefix
				bool isOwner = e.User.Id == 140564059417346049; // test if user is the owner of the bot
				string msg = e.Message.Text; // grab contents of message
				string rawmsg = e.Message.RawText; // grab raw contents of message

				if(isCmd && (e.Channel.Id == 282500390891683841 || isOwner))
				{
					var msgarray = msg.Replace (p, "").Split (' ');
					string cmd = msgarray.FirstOrDefault ().ToString ();
					var args = msgarray.Skip (1).ToArray ();
					var argtext = msg.Replace(p+cmd+"", "");
					if (argtext.Contains(p+cmd+" "))
						argtext = msg.Replace(p+cmd+" ", "");

					if(cmd == "help")
					{
						string help = $"`{p}help` - lists the bot commands";
						string usercount = $"`{p}usercount` - lists the users on the server"; // note: this command won't work unless joinbot is running
						string userinfo = $"`{p}userinfo` - displays info about yourself or a mentioned user";
						string serverinfo = $"`{p}serverinfo` - displays info about the server";

						string cmds = $"\n{help}\n{usercount}\n{userinfo}\n{serverinfo}";
						string greet = "Hi! I'm the STEM part of the HTC-Bote, a super-exclusive part only for the HTwins STEM server.";
						string avacmds = $"I have a couple commands you can try out, which include: {cmds}";

						string helpmsg = greet + avacmds;
						await e.Channel.SendMessage(helpmsg);
					}
					else if (cmd == "userinfo")
					{
						// find user

						User usr = e.User;

						if (e.Message.MentionedUsers.FirstOrDefault () != null)
							usr = e.Message.MentionedUsers.FirstOrDefault ();
						else if (argtext != "")
							usr = e.Server.FindUsers(argtext).FirstOrDefault ();

						// get user info

						ulong id = usr.Id;
						string username = usr.Name;
						string discrim = $"#{usr.Discriminator}";

						string nickname = usr.Nickname;
						if (string.IsNullOrWhiteSpace(nickname))
							nickname = "[none]";

						string game;
						string status = usr.Status.ToString();

						if (usr.CurrentGame == null)
							game = "[none]";
						else
						{
							game = usr.CurrentGame.GetValueOrDefault().Name;
							string streamUrl = usr.CurrentGame.GetValueOrDefault().Url;
							if (usr.CurrentGame.GetValueOrDefault().Url != null)
								status = $"streaming # {streamUrl}";
						}

						DateTime joined = usr.JoinedAt;
						var joinedDays = DateTime.Now - joined;
						string avatar = usr.AvatarUrl;

						// send message

						await e.Channel.SendMessage($"```ini\n" +
							$"\n[ID]            {id}\n" +
							$"[Username]      {username}\n" +
							$"[Discriminator] {discrim}\n" +
							$"[Nickname]      {nickname}\n" +
							$"[Current game]  {game}\n" +
							// $"[Status]        {status}\n" +     | disabled because everyone shows as offline. possible discord bug
							$"[Joined]        {joined} ({joinedDays.Days} days ago)\n" +
							$"[Avatar] {avatar}\n```");
					}
					else if (cmd == "serverinfo")
					{
						var Roles = e.Server.Roles;
						List<string> rolesList = new List<string>();
						foreach (Role role in Roles)
							rolesList.Add(role.Name);

						var CreationDate = DateTime.Now - e.Server.Owner.JoinedAt;
						string rolesString = string.Join(", ", rolesList.ToArray());
						string region = e.Server.Region.Name;

						await e.Channel.SendMessage($"```ini\n" +
							$"[Name]            {e.Server.Name}\n" +
							$"[ID]              {e.Server.Id}\n" +
							$"[User Count]      {e.Server.UserCount}\n" +
							$"[Channel Count]   {e.Server.ChannelCount}\n" +
							$"[Default Channel] #{e.Server.DefaultChannel}\n" +
							$"[Role Count]      {e.Server.RoleCount}\n" +
							$"[Roles]           {rolesString}\n" +
							$"[Owner]           @{e.Server.Owner}\n" +
							$"[Creation date]   {e.Server.Owner.JoinedAt} ({CreationDate.Days} days ago) # possibly inaccurate\n" +
							$"[Icon] {e.Server.IconUrl}\n" +
							$"```");
					}
				}
			};

			string token = File.ReadAllText("token.txt");
			_client.ExecuteAndWait(async () => {
				await _client.Connect(token, TokenType.Bot);
				Console.WriteLine($"Connected as {_client.CurrentUser.Name}#{_client.CurrentUser.Discriminator}");
			});

		}
	}
}
