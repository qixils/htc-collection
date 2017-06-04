using System;
using System.IO;
using System.Linq;
using Discord;
using System.Collections.Generic;
using System.Timers;
using System.Text;

namespace joinbot
{
	class joinbot
	{
		static void Main(string[] args) => new joinbot().Start();

		public void Start()
		{
			Bot(File.ReadAllLines("token.txt").ElementAt(0), 1);
			Bot(File.ReadAllLines("token.txt").ElementAt(1), 2);
		}

		public void Bot(string token, int botnum)
		{
			DiscordClient _client = new DiscordClient();

			int maxnum = 2;
			ulong banneduser = 000000000000000000;
			string p = "!";

			// _client.Log.Message += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

			_client.MessageReceived += async (s, e) =>
			{
				try
				{
					if (!(botnum == 2 && e.Server.Id == 282219466589208576))
					{
						string msg = e.Message.RawText;
						string cmd = msg.Replace(p, "").Split(' ').FirstOrDefault().ToString();
						var args = msg.Replace(p, "").Split(' ').Skip(1).ToArray();
						string rawargs = e.Message.RawText.Replace($"{p}{cmd} ", "");
						bool isCmd = e.Message.Text.StartsWith(p);
						bool isBot = e.User.IsBot;
						bool isSelf = e.User.Id == _client.CurrentUser.Id;
						bool isBotSpam = e.Channel.Id == 282500390891683841;
						bool isMusic = e.Channel.Name.Contains("music");
						bool isJoinbot = e.Channel.Name.Contains("joinbot");

						bool shouldntLog = isCmd || isBotSpam || isBot || isSelf || isMusic || isJoinbot;

						if (!shouldntLog) { MessageIncrement(botnum, e.User, e.Server.Id); }

						Server htc = _client.GetServer(184755239952318464);
						User user = e.User;

						if (!e.Channel.IsPrivate)
						{
							Server serv = e.Server;
							User bot = e.Server.GetUser(_client.CurrentUser.Id);
							User owner = e.Server.GetUser(140564059417346049);

							bool isOwner = false;
							if (e.Server.GetUser(140564059417346049) != null)
								if (e.User == e.Server.GetUser(140564059417346049))
									isOwner = true;
							else if ((isOwner || e.Channel.Name.Contains("joinbot")) && cmd == "userinfo")
							{
								var users = e.Server.Users.OrderBy(human => user.JoinedAt).ToList(); // refresh user cache for later on

								// find user

								User usr = e.User;
								var txtsearch = e.Server.FindUsers(rawargs, false).FirstOrDefault();

								if (e.Message.MentionedUsers.FirstOrDefault() != null) // checks if there's a mentioned user
									usr = e.Message.MentionedUsers.FirstOrDefault(); // updates 'usr' to be the mentioned user
								else if (rawargs != "")
								{
									if (txtsearch != null)
										usr = txtsearch;
								}
								// else if(e.Server.GetUser(Convert.ToUInt64(argtext)) != null)
								// usr = e.Server.GetUser(Convert.ToUInt64(argtext));

								// get user info

								ulong id = usr.Id;
								string username = clearformatting(usr.Name);
								string discrim = $"#{usr.Discriminator}";

								string nickname = "[none]";
								if (!string.IsNullOrWhiteSpace(usr.Nickname))
									nickname = clearformatting(usr.Nickname);

								string game;
								// string status = usr.Status.ToString();

								if (usr.CurrentGame == null)
									game = "[none]";
								else
								{
									game = clearformatting(usr.CurrentGame.GetValueOrDefault().Name);
									string streamUrl = usr.CurrentGame.GetValueOrDefault().Url;
									//if (usr.CurrentGame.GetValueOrDefault().Url != null)
									//    status = $"streaming # {streamUrl}";
								}

								DateTime joined = usr.JoinedAt;
								var joinedDays = DateTime.Now - joined;
								DateTimeOffset created = CreationDate(usr.Id);
								var createdDays = DateTime.Now - created;
								string avatar = usr.AvatarUrl;

								//	int usercount = 0;
								//	int memnum = 0;
								//	while (memnum == 0)
								//	{
								//		usercount++;
								//		User currentuser = users.ElementAt (usercount);
								//		if(e.User == currentuser)
								//			memnum = usercount;
								//	}

								// send message

								await e.Channel.SendMessage($"```ini\n" +
																$"\n[ID]            {id}\n" +
																$"[Username]      {username}\n" +
																$"[Discriminator] {discrim}\n" +
																$"[Nickname]      {nickname}\n" +
																$"[Current game]  {game}\n" +
																// $"[Status]        {status}\n" +
																$"[Joined]        {joined} ({joinedDays.Days} days ago)\n" +
																$"[Created]       {created.ToString().Replace(" +00:00", "")} ({createdDays.Days} days ago)\n" +
																//$"[Member #]      {memnum}\n" +      | broken for some reason. often shows users as being member #11
																$"[Avatar] {avatar}\n```");
							}
							else if (isOwner && cmd == "roles")
							{
								var roles = e.Server.Roles.OrderBy(role => role.Position).ToList();
								string rolestring = string.Join(", ", roles);
								Console.WriteLine($"{rolestring}");
							}
							else if (isOwner && cmd == "minuser")
							{
								List<User> users = new List<User>();
								List<User> bots = new List<User>();
								foreach (User usr in e.Server.Users)
								{
									if (!usr.IsBot)
										users.Add(usr);
									else
										bots.Add(usr);
								}
								var sortedusers = users.OrderBy(usr => usr.Id);
								var sortedbots = bots.OrderBy(usr => usr.Id);
								User minuser = sortedusers.First();
								User minbot = sortedbots.First();
								await e.Channel.SendMessage($"Lowest User ID: {minuser.Id} ({minuser.Mention})\nLowest Bot ID: {minbot.Id} ({minbot.Mention})");
							}
							else if (isOwner && cmd == "modcount")
							{
								List<User> users = new List<User>();
								List<string> usernames = new List<string>();
								foreach (User dude in e.Server.Users)
								{
									List<char> name = dude.Name.ToList();
									if (name.Count > 2)
									{
										if (name[2] == 'a')
										{
											users.Add(dude);
											usernames.Add(dude.Name);
										}
									}
								}
								Console.WriteLine(users.Count);
								await e.Channel.SendMessage($"Well according to my calculations, there are {users.Count} users with an `a` in the 3rd slot of their username. This means we should have {users.Count * 10} mods.");
								Console.WriteLine(string.Join(", ", usernames));
							}
							else if (isOwner && cmd == "topusers")
							{
								if (args == null) { await e.Channel.SendMessage(MessageTop(botnum, e.Server.Id, 10)); }
								else
								{
									int users = 10;
									bool parse = int.TryParse(args[0], out users);
									string topusers = "idk something borked";
									if (parse)
									{
										if (users > 20 && !isOwner) { await e.Channel.SendMessage($"woah, keep it under 20 will ya?"); }
										else { topusers = MessageTop(botnum, e.Server.Id, users); }
									}
									else { topusers = MessageTop(botnum, e.Server.Id, 10); }
									await e.Channel.SendMessage(topusers);
								}
							}
							else if (isOwner && !e.Channel.Name.Contains("music") && (cmd == "restart" || cmd == "shutdown"))
							{
								await e.Channel.SendMessage("Restarting bot...");
								Console.WriteLine("Restarting bot...");
								Environment.Exit(0);
							}

							// semi-public commands

							if (e.Message.Text == "!gameupdate")
								_client.SetGame("for " + htc.UserCount.ToString() + " users");

							if (isOwner || e.User.ServerPermissions.ManageRoles || e.Channel.Id == 282500390891683841)
							{
								if (e.Message.Text == "!usercount")
									await e.Channel.SendMessage(e.Server.Name + " currently has " + e.Server.UserCount + " users.");

								else if (e.Message.Text.StartsWith($"!discrimsearch "))
								{
									try
									{
										foreach (User usr in serv.Users)
										{
											if (usr.Discriminator.ToString() == e.Message.Text.Replace("!discrimsearch ", ""))
												await e.Channel.SendMessage($"Found user `{usr.Name}` | ID: `{usr.Id}`");
										}
									}
									catch (Exception error)
									{
										Console.WriteLine("[ERROR] An error occured while running !discrimsearch: \n" + error.ToString());
									}
								}
							}
						}
					}
				}
				catch (Exception error) { Console.WriteLine($"[ERROR] An issue occured while trying to send out a message. | {error.ToString()}"); }
			};

			_client.MessageDeleted += (s, e) =>
			{
				try
				{
					if (!(botnum == 2 && e.Server.Id == 282219466589208576))
					{
						bool isCmd = e.Message.Text.StartsWith(p);
						bool isBot = e.User.IsBot;
						bool isSelf = e.User.Id == _client.CurrentUser.Id;
						bool isBotSpam = e.Channel.Id == 282500390891683841;
						bool isMusic = e.Channel.Name.Contains("music");
						bool isJoinbot = e.Channel.Name.Contains("joinbot");

						bool shouldntLog = isCmd || isBotSpam || isBot || isSelf || isMusic || isJoinbot;

						if (!shouldntLog) { MessageDecrement(botnum, e.User, e.Server.Id); }
					}
				}
				catch (Exception error) { Console.WriteLine(error); }
			};

			_client.UserJoined += async (s, e) =>
			{
				if (e.Server.Id == 184755239952318464)
					_client.SetGame($"for {e.Server.UserCount} users");
				var time = DateTime.Now;
				Console.WriteLine($"[{time.Month}/{time.Day} - {time.Hour}:{time.Minute}:{time.Second} - Bot #{botnum}] A user joined {e.Server.Name}: {e.User.Name}#{e.User.Discriminator} ({e.User.Id})");

				Channel logChannel = null;
				Channel backupChannel = null;
				bool validChannel = false;

				if (!(botnum == 2 && e.Server.Id == 282219466589208576)) { logChannel = e.Server.FindChannels("joinbot").FirstOrDefault(); }

				if (logChannel != null)
					validChannel = true;
				if (e.Server.Id == 184755239952318464)
				{
					logChannel = _client.GetChannel(207659596167249920);
					backupChannel = _client.GetChannel(303374467336241152);
				}
				else if (e.Server.Id == 282219466589208576 && botnum != 2)
				{
					logChannel = _client.GetChannel(282477076454309888);
					backupChannel = _client.GetChannel(303374407420608513);
				}

				//the following code is for logging to a hardcoded channel. it can be used with the above code

				//				if(e.Server.Id == 184755239952318464) {
				//					logChannel = e.Server.GetChannel(207659596167249920);
				//					_client.SetGame($"for {e.Server.UserCount} users");
				//					validChannel = true;
				//				}

				string discrim = "" + e.User.Discriminator;
				if (discrim.Length == 3)
					discrim = "0" + discrim;
				else if (discrim.Length == 2)
					discrim = "00" + discrim;
				else if (discrim.Length == 1)
					discrim = "000" + discrim;
				else if (discrim.Length == 0)
					discrim = " [Something has gone horribly wrong and the user doesn't have a discriminator. Please inform Noahkiq of this, thanks!]";

				string msg = $"✅  {e.User.Mention} (`{e.User.Name}#{discrim}` User #{e.Server.UserCount}) user joined the server.";
				if (string.IsNullOrWhiteSpace(e.User.AvatarUrl))
					msg += "\n😶 User doesn't have an avatar.";
				try
				{
					var creationTime = CreationDate(e.User.Id);
					var timeDiff = time - creationTime;
					if (timeDiff.TotalHours <= 24) { msg += $"\n🕐 User's account was created at {creationTime.ToString()}"; }
					else { msg += $"\nUser's account was created at {creationTime.ToString()}"; }
				}
				catch (Exception err)
				{
					Console.WriteLine($"[NoahError(tm)] something happened while tryin to do the timestamp grabby thing: \n{err.ToString()}");
				}

				try
				{
					if (validChannel)
						await logChannel.SendMessage(msg);
					if (backupChannel != null)
						await backupChannel.SendMessage(msg);
					if (e.Server.Id == 303365979444871169)
						await e.Server.GetChannel(303530104339038209).SendMessage(msg);
				}
				catch (Exception error) { Console.WriteLine($"[ERROR] An issue occured while trying to send out a message. | {error.ToString()}"); }
			};

			_client.UserLeft += async (s, e) =>
			{
				if (e.Server.Id == 184755239952318464)
					_client.SetGame($"for {e.Server.UserCount} users");
				System.Threading.Thread.Sleep(250);
				bool isBanned = e.User.Id == banneduser;

				// if (!isBanned) {
				// 	var bans = await e.Server.GetBans();
				//     foreach (User banned in bans)
				//     	if(banned.Id == e.User.Id)
				//     	    isBanned = true;
				// }

				if (!isBanned)
				{
					var time = DateTime.Now;
					Console.WriteLine($"[{time.Month}/{time.Day} - {time.Hour}:{time.Minute}:{time.Second} - Bot #{botnum}] A user left {e.Server.Name}: {e.User.Name}#{e.User.Discriminator} ({e.User.Id})");

					Channel logChannel = null;
					Channel backupChannel = null;
					bool validChannel = false;

					if (!(botnum == 2 && e.Server.Id == 282219466589208576)) { logChannel = e.Server.FindChannels("joinbot").FirstOrDefault(); }
					if (logChannel != null)
						validChannel = true;
					if (e.Server.Id == 184755239952318464)
					{
						logChannel = _client.GetChannel(207659596167249920);
						backupChannel = _client.GetChannel(303374467336241152);
					}
					else if (e.Server.Id == 282219466589208576 && botnum != 2)
					{
						logChannel = _client.GetChannel(282477076454309888);
						backupChannel = _client.GetChannel(303374407420608513);
					}

					// the following code is for logging to a hardcoded channel. it can be used with the above code

					//				if(e.Server.Id == 184755239952318464) {
					//					logChannel = e.Server.GetChannel(207659596167249920);
					//					_client.SetGame($"for {e.Server.UserCount} users");
					//					validChannel = true;
					//				}

					string discrim = "" + e.User.Discriminator;
					if (discrim.Length == 3)
						discrim = "0" + discrim;
					else if (discrim.Length == 2)
						discrim = "00" + discrim;
					else if (discrim.Length == 1)
						discrim = "000" + discrim;
					else if (discrim.Length == 0)
						discrim = " [Something has gone horribly wrong and the user doesn't have a discriminator. Please inform Noahkiq of this, thanks!]";

					try
					{
						string msg = $"❌ {e.User.Mention} (`{e.User.Name}#{discrim}`) left the server.";
						if (validChannel)
							await logChannel.SendMessage(msg);
						if (backupChannel != null)
							await backupChannel.SendMessage(msg);
						if (e.Server.Id == 303365979444871169)
							await e.Server.GetChannel(303530104339038209).SendMessage(msg);
					}
					catch (Exception error) { Console.WriteLine($"[ERROR] An issue occured while trying to send out a message. | {error.ToString()}"); }
				}
			};

			_client.UserBanned += async (s, e) =>
			{
				banneduser = e.User.Id;

				var time = DateTime.Now;
				Console.WriteLine($"[{time.Month}/{time.Day} - {time.Hour}:{time.Minute}:{time.Second} - Bot #{botnum}] A user was banned from {e.Server.Name}: {e.User.Name}#{e.User.Discriminator} ({e.User.Id})");

				Channel logChannel = null;
				Channel backupChannel = null;
				bool validChannel = false;

				if (!(botnum == 2 && e.Server.Id == 282219466589208576)) { logChannel = e.Server.FindChannels("joinbot").FirstOrDefault(); }
				if (logChannel != null)
					validChannel = true;
				if (e.Server.Id == 184755239952318464)
				{
					logChannel = _client.GetChannel(207659596167249920);
					backupChannel = _client.GetChannel(303374467336241152);
				}
				else if (e.Server.Id == 282219466589208576 && botnum != 2)
				{
					logChannel = _client.GetChannel(282477076454309888);
					backupChannel = _client.GetChannel(303374407420608513);
				}

				// the following code is for logging to a hardcoded channel. it can be used with the above code

				//				if(e.Server.Id == 184755239952318464) {
				//					logChannel = e.Server.GetChannel(207659596167249920);
				//					_client.SetGame($"for {e.Server.UserCount} users");
				//					validChannel = true;
				//				}

				string discrim = "" + e.User.Discriminator;
				if (discrim.Length == 3)
					discrim = "0" + discrim;
				else if (discrim.Length == 2)
					discrim = "00" + discrim;
				else if (discrim.Length == 1)
					discrim = "000" + discrim;
				else if (discrim.Length == 0)
					discrim = " [Something has gone horribly wrong and the user doesn't have a discriminator. Please inform Noahkiq of this, thanks!]";

				try
				{
					string msg = $"🔨 {e.User.Mention} (`{e.User.Name}#{discrim}`) was banned from the server.";

					if (validChannel)
						await logChannel.SendMessage(msg);
					if (backupChannel != null)
						await backupChannel.SendMessage(msg);
					if (e.Server.Id == 303365979444871169)
						await e.Server.GetChannel(303530104339038209).SendMessage(msg);
				}
				catch (Exception error) { Console.WriteLine($"[ERROR] An issue occured while trying to send out a message. | {error.ToString()}"); }
			};

			_client.UserUnbanned += async (s, e) =>
			{
				var time = DateTime.Now;
				Console.WriteLine($"[{time.Month}/{time.Day} - {time.Hour}:{time.Minute}:{time.Second} - Bot #{botnum}] A user was unbanned from {e.Server.Name}: {e.User.Name}#{e.User.Discriminator} ({e.User.Id})");

				Channel logChannel = null;
				Channel backupChannel = null;
				bool validChannel = false;

				if (!(botnum == 2 && e.Server.Id == 282219466589208576)) { logChannel = e.Server.FindChannels("joinbot").FirstOrDefault(); }
				if (logChannel != null)
					validChannel = true;
				if (e.Server.Id == 184755239952318464)
				{
					logChannel = _client.GetChannel(207659596167249920);
					backupChannel = _client.GetChannel(303374467336241152);
				}
				else if (e.Server.Id == 282219466589208576 && botnum != 2)
				{
					logChannel = _client.GetChannel(282477076454309888);
					backupChannel = _client.GetChannel(303374407420608513);
				}

				// the following code is for logging to a hardcoded channel. it can be used with the above code

				//				if(e.Server.Id == 184755239952318464) {
				//					logChannel = e.Server.GetChannel(207659596167249920);
				//					_client.SetGame($"for {e.Server.UserCount} users");
				//					validChannel = true;
				//				}

				string discrim = "" + e.User.Discriminator;
				if (discrim.Length == 3)
					discrim = "0" + discrim;
				else if (discrim.Length == 2)
					discrim = "00" + discrim;
				else if (discrim.Length == 1)
					discrim = "000" + discrim;
				else if (discrim.Length == 0)
					discrim = " [Something has gone horribly wrong and the user doesn't have a discriminator. Please inform Noahkiq of this, thanks!]";

				try
				{
					string msg = $"🔓 {e.User.Mention} (`{e.User.Name}#{discrim}` was unbanned from the server.";

					if (validChannel)
						await logChannel.SendMessage(msg);
					if (backupChannel != null)
						await backupChannel.SendMessage(msg);
					if (e.Server.Id == 303365979444871169)
						await e.Server.GetChannel(303530104339038209).SendMessage(msg);
				}
				catch (Exception error) { Console.WriteLine($"[ERROR] An issue occured while trying to send out a message. | {error.ToString()}"); }
			};

			_client.UserUpdated += async (s, e) =>
			{
				if (e.Before.Name != e.After.Name)
				{
					var time = DateTime.Now;
					Console.WriteLine($"[{time.Month}/{time.Day} - {time.Hour}:{time.Minute}:{time.Second} - Bot #{botnum}] A user ({e.After.Id}) changed their name from {e.Before.Name}#{e.Before.Discriminator} to {e.After.Name}#{e.After.Discriminator}");

					Channel logChannel = null;
					Channel backupChannel = null;
					bool validChannel = false;

					if (!(botnum == 2 && e.Server.Id == 282219466589208576)) { logChannel = e.Server.FindChannels("joinbot").FirstOrDefault(); }
					if (logChannel != null)
						validChannel = true;
					if (e.Server.Id == 184755239952318464)
					{
						logChannel = _client.GetChannel(207659596167249920);
						backupChannel = _client.GetChannel(303374467336241152);
					}
					else if (e.Server.Id == 282219466589208576 && botnum != 2)
					{
						logChannel = _client.GetChannel(282477076454309888);
						backupChannel = _client.GetChannel(303374407420608513);
					}

					// the following code is for logging to a hardcoded channel. it can be used with the above code

					//					if(e.Server.Id == 184755239952318464) {
					//						logChannel = e.Server.GetChannel(207659596167249920);
					//						validChannel = true;
					//					}

					string oldDiscrim = "" + e.Before.Discriminator;
					if (oldDiscrim.Length == 3)
						oldDiscrim = "0" + oldDiscrim;
					else if (oldDiscrim.Length == 2)
						oldDiscrim = "00" + oldDiscrim;
					else if (oldDiscrim.Length == 1)
						oldDiscrim = "000" + oldDiscrim;
					else if (oldDiscrim.Length == 0)
						oldDiscrim = " [Something has gone horribly wrong and the user doesn't have a discriminator. Please inform Noahkiq of this, thanks!]";

					string newDiscrim = "" + e.After.Discriminator;
					if (newDiscrim.Length == 3)
						newDiscrim = "0" + newDiscrim;
					else if (newDiscrim.Length == 2)
						newDiscrim = "00" + newDiscrim;
					else if (newDiscrim.Length == 1)
						newDiscrim = "000" + newDiscrim;
					else if (newDiscrim.Length == 0)
						newDiscrim = " [Something has gone horribly wrong and the user doesn't have a discriminator. Please inform Noahkiq of this, thanks!]";

					string msg = $"User **{e.Before.Name}#{oldDiscrim}** changed their name to **{e.After.Name}#{newDiscrim}** ({e.After.Mention})";
					if (oldDiscrim != newDiscrim)
						msg += "\n🔁 *User's discriminator changed!*";

					try
					{
						if (validChannel)
							await logChannel.SendMessage(msg);
						if (backupChannel != null)
							await backupChannel.SendMessage(msg);
						// if (e.Server.Id == 303365979444871169)
						//     await e.Server.GetChannel(303530104339038209).SendMessage(msg);
					}
					catch (Exception error) { Console.WriteLine($"[ERROR] An issue occured while trying to send out a message. | {error.ToString()}"); }
				}
				else if ((e.Before.AvatarId != e.After.AvatarId) && (e.Server.Id == 184755239952318464 || e.Server.Id == 282219466589208576))
				{
					var time = DateTime.Now;
					Console.WriteLine($"[{time.Month}/{time.Day} - {time.Hour}:{time.Minute}:{time.Second} - Bot #{botnum}] {e.After.Name}#{e.After.Discriminator} ({e.After.Id}) changed their avatar from {e.Before.AvatarUrl} to {e.After.AvatarUrl}");

					Channel logChannel = null;

					if (e.Server.Id == 184755239952318464) { logChannel = _client.GetChannel(305337536157450240); }
					else { logChannel = _client.GetChannel(305337565513515008); }

					string discrim = "" + e.After.Discriminator;
					if (discrim.Length == 3)
						discrim = "0" + discrim;
					else if (discrim.Length == 2)
						discrim = "00" + discrim;
					else if (discrim.Length == 1)
						discrim = "000" + discrim;
					else if (discrim.Length == 0)
						discrim = " [Something has gone horribly wrong and the user doesn't have a discriminator. Please inform Noahkiq of this, thanks!]";

					string msg = $"<:bookFace:310922953791504384> User **{e.After.Name}#{discrim}** changed their avatar from {e.Before.AvatarUrl} to {e.After.AvatarUrl} ({e.After.Mention})";

					try
					{
						await logChannel.SendMessage(msg);
					}
					catch (Exception error) { Console.WriteLine($"[ERROR] An issue occured while trying to send out a message. | {error.ToString()}"); }
				}
			};

			_client.ServerAvailable += (s, e) =>
			{
				if (e.Server.Id == 184755239952318464)
					_client.SetGame($"for {e.Server.UserCount} users");
			};

			_client.Ready += (s, e) =>
			{
				Console.WriteLine($"[{botnum}] Connected as {_client.CurrentUser.Name}#{_client.CurrentUser.Discriminator}");
			};

			if (botnum == maxnum)
			{
				_client.ExecuteAndWait(async () =>
				{
					await _client.Connect(token, TokenType.Bot);
				});
			}
			else
			{
				_client.Connect(token, TokenType.Bot);
			}
		}
		public static string TrimString(string input, int charstotrim)
		{
			string output = input.Remove(input.Length - charstotrim);
			return output;
		}
		public static DateTimeOffset CreationDate(ulong id)
		{
			return DateTimeOffset.FromUnixTimeMilliseconds((Convert.ToInt64(id) >> 22) + 1420070400000);
		}
		public void MessageIncrement(int botnum, User user, ulong sid)
		{
			string botstring = "bot-" + botnum;
			var sepchar = Path.DirectorySeparatorChar; // Grab the current operating system's seperation char. (eg. windows: \, linux: /)
			var path = $"{Directory.GetCurrentDirectory() + sepchar + botstring + sepchar + sid.ToString() + sepchar + DateTime.Now.Month + sepchar}"; // get directory of data file
			var datafile = $"{path + user.Id}.txt"; // get data file
			Directory.CreateDirectory(path); // create directory if it doesn't exist

			if (File.Exists(datafile)) // checks if data file exists
			{
				if (new FileInfo(datafile).Length > 1) // checks if data file has data
				{
					string line = File.ReadAllLines(datafile)[0];
					int messages = 0;
					bool parse = int.TryParse(line, out messages);
					if (parse)
					{
						messages++;
						File.WriteAllText(datafile, $"{messages}\n{user.Name}");
					}
					else { File.WriteAllText(datafile, $"1\n{user.Name}"); }
				}
				else { File.WriteAllText(datafile, $"1\n{user.Name}"); }
			}
			else { File.WriteAllText(datafile, $"1\n{user.Name}"); }
		}
		public void MessageDecrement(int botnum, User user, ulong sid)
		{
			string botstring = "bot-" + botnum;
			var sepchar = Path.DirectorySeparatorChar; // Grab the current operating system's seperation char. (eg. windows: \, linux: /)
			var path = $"{Directory.GetCurrentDirectory() + sepchar + botstring + sepchar + sid.ToString() + sepchar + DateTime.Now.Month + sepchar}"; // get directory of data file
			var datafile = $"{path + user.Id}.txt"; // get data file
			Directory.CreateDirectory(path); // create directory if it doesn't exist

			if (File.Exists(datafile)) // checks if data file exists
			{
				if (new FileInfo(datafile).Length > 1) // checks if data file has data
				{
					string line = File.ReadAllLines(datafile)[0];
					int messages = 0;
					bool parse = int.TryParse(line, out messages);
					if (parse)
					{
						messages--;
						File.WriteAllText(datafile, $"{messages}\n{user.Name}");
					}
					else { File.WriteAllText(datafile, $"1\n{user.Name}"); }
				}
				else { File.WriteAllText(datafile, $"1\n{user.Name}"); }
			}
			else { File.WriteAllText(datafile, $"1\n{user.Name}"); }
		}
		public string MessageTop(int botnum, ulong sid, int numberOfUsers)
		{
			string botstring = "bot-" + botnum;
			var sepchar = Path.DirectorySeparatorChar; // Grab the current operating system's seperation char. (eg. windows: \, linux: /)
			var path = $"{Directory.GetCurrentDirectory() + sepchar + botstring + sepchar + sid.ToString() + sepchar + DateTime.Now.Month + sepchar}"; // get directory of data file
			Directory.CreateDirectory(path); // create directory if it doesn't exist

			string[] files = Directory.GetFiles(path);
			Dictionary<string, int> messagecounts = new Dictionary<string, int>();
			foreach (string filename in files)
			{
				try
				{
					string filepath = filename;
					if (!filename.Contains(botstring)) { filepath = $"{path + filename}"; } // didn't bother to check what 'filename' actually results in so this makes sure it's a path

					string messagesString = File.ReadAllLines(filepath)[0];
					string username = File.ReadAllLines(filepath)[1];
					int activecount = 0;
					bool parse = int.TryParse(messagesString, out activecount);
					if (parse)
					{
						messagecounts.Add(username, activecount);
					}
				}
				catch (Exception error)
				{
					Console.WriteLine($"something bad happened while trying to grab/write info about a user during messagetop: {error.ToString()}");
				}
			}
			var filtered = messagecounts.OrderByDescending(x => x.Value).Take(numberOfUsers);
			string topusers = "";
			foreach (var pair in filtered)
			{
				topusers += $"{pair.Key}: {pair.Value}\n";
			}
			return topusers;
		}
		public static string clearformatting(string input)
		{
			var output = "[empty string]";
			if (!string.IsNullOrWhiteSpace(input))
				output = input.Replace("`", "​`").Replace("*", "​*").Replace("_", "​_").Replace("‮", " ");
			return output;
		}
	}
}
