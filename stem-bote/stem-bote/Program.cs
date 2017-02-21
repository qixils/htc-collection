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

				bool isCmd = false;
				if(e.Message.Text.StartsWith(p+p))
					isCmd = false; // if message has the prefix two (or more) times, set isCmd to false
				else if(e.Message.Text.StartsWith(p))
					isCmd = true; // otherwise, if there's only one prefix, set isCmd to true
				// ^ this is a hacky solution but it works

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
						string roll = $"`{p}roll [# of sides] [# of dice to roll]` - rolls some dice";

						string cmds = $"\n{help}\n{usercount}\n{userinfo}\n{serverinfo}\n{roll}";
						string greet = "Hi! I'm the STEM part of the HTC-Bote, a super-exclusive part only for the HTwins STEM server. ";
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
						else if (argtext != "") { // this is broken, need to look into why
							if (e.Server.FindUsers(argtext).FirstOrDefault () != null)
								usr = e.Server.FindUsers(argtext).FirstOrDefault ();
						}

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
					else if (cmd == "roll")
					{
						try
						{
							Random random = new Random(); // Generate some randomness
							string output = $"The 6-sided die rolled a {random.Next(1, 7)}."; // Define default output

							if(args.Length == 2) // tests if there are two arguments
							{
								if(args[1] != null)
								{
									int sides = int.Parse(args[0]); // Set die sides
									int dice = int.Parse(args[1]); // Set # of dice to roll

									int maxRolls = sides + 1; // Set max roll int
									if((dice < 1) || (sides < 1)) // check if # of dice/die sides is under 1
										output = $"Yeahh, sorry, but you can't roll something that doesn't exist.";
									else if(sides == 1)
										output = $"All {dice} of the 1-sided dice shockingly rolled a 1."; // Change output if first arg was 1
									else if (dice <= 30)
									{
										if(sides <= 100)
										{
											int rollTracker = dice; // Create new variable to track the number of rolls left
											string rolledDice = ""; // Set blank string for the dice rolled
											while (rollTracker > 0) // If there are still dice to roll...
											{
												if(rollTracker > 1) /// ...and it's not only one die...
													rolledDice += $"{random.Next(1, maxRolls)}, "; // ... add a random roll to the roledDice string
												else // or, if there is only one die left
													rolledDice += $"and {random.Next(1, maxRolls)}"; // add the final random roll

												rollTracker--; // subtract one die left to roll from roleTracker
											}
											output = $"The {dice} {sides}-sided dice rolled {rolledDice}."; // set output
										}
										else
											output = $"Woahh, that's a lot of sides. Try lowering it below 100?";
									}
									else
										output = "Woahh, that's a lot of dice to roll. Try lowering it below 30?"; // display error message because 30 rolls = lots of spam
								}
							}
							else if (args.Length == 1)
							{
								if (args[0] != null)
								{
									int sides = int.Parse(args[0]); // Set die sides
									int maxRolls = sides + 1; // Set max roll int
									if(sides < 1)
										output = $"Yeahh, sorry, but you can't roll something that doesn't exist.";
									else if(maxRolls == 2)
										output = "The 1-sided die shockingly rolled a 1."; // Change output if first argument was "1"
									else
									{
										output = $"The {sides}-sided die rolled a {random.Next(1, maxRolls)}."; // Set the output message

										// hey, you! yes, you, reading this! don't you dare tell people about these easter eggs! i will be watching...
										if(sides == 666)
											output = $"Satan rolled a nice {random.Next(1, maxRolls)} for you.";
										else if (sides == 1337)
											output = $"Th3 {sides}-51d3d d13 r0ll3d 4 {random.Next(1, maxRolls)}.";
										else if (sides == e.Server.UserCount) {
											int rndNumber = random.Next(1, maxRolls);
											string rndUser = e.Server.Users.ElementAt (rndNumber).Name;
											output = $"{e.Server.UserCount}? That's how many users are on the server! Well, your die rolled a {random.Next(1, maxRolls)}, and according to the cache, that member is `{rndUser}`.";
										}
									}
								}
							}

							await e.Channel.SendMessage(output);
						}
						catch (Exception error)
						{
							Console.WriteLine ($"[ERROR] Something happened while running {p}{cmd}: \n{error.ToString()}");
							await e.Channel.SendMessage($"An error occured while trying to roll the dice. You most likely entered non-integers.");
						}
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
