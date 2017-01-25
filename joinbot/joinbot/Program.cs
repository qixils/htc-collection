using System;
using System.IO;
using System.Linq;
using Discord;

namespace joinbot
{
	class joinbot
	{
		static void Main(string[] args) => new joinbot().Start();

		private DiscordClient _client;

		public void Start()
		{
			_client = new DiscordClient();

			_client.Log.Message += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

			_client.UserJoined += async (s, e) => {
				Channel logChannel = e.Server.AllChannels.FirstOrDefault();
				bool validChannel = false;

				// logChannel = e.Server.FindChannels("joinbot").FirstOrDefault();
				// if(logChannel != null)
				//     validChannel = true;

				// If you want to log messages to #joinbot instead of one hardcoded channel, comment the next 2 lines and uncomment the last three

				if(e.Server.Id == 184755239952318464) {
					logChannel = e.Server.GetChannel(207659596167249920);
					validChannel = true;
				}

				if (validChannel)
					await logChannel.SendMessage($"{e.User.Mention} (User #{e.Server.UserCount}) user joined the server.");
			};
				
			_client.UserLeft += async (s, e) => {
				Channel logChannel = e.Server.AllChannels.FirstOrDefault();
				bool validChannel = false;

				// logChannel = e.Server.FindChannels("joinbot").FirstOrDefault();
				// if(logChannel != null)
				//     validChannel = true;

				// If you want to log messages to #joinbot instead of one hardcoded channel, comment the next 2 lines and uncomment the last three

				if(e.Server.Id == 184755239952318464) {
					logChannel = e.Server.GetChannel(207659596167249920);
					validChannel = true;
				}

				if (validChannel)
					await logChannel.SendMessage($"{e.User.Mention} left the server.");
			};

			_client.UserUpdated += async (s, e) => {
				if (e.Before.Name != e.After.Name)
				{
					Channel logChannel = e.Server.AllChannels.FirstOrDefault();
					bool validChannel = false;

					// logChannel = e.Server.FindChannels("joinbot").FirstOrDefault();
					// if(logChannel != null)
					//     validChannel = true;

					// If you want to log messages to #joinbot instead of one hardcoded channel, comment the next 2 lines and uncomment the last three

					if(e.Server.Id == 184755239952318464) {
						logChannel = e.Server.GetChannel(207659596167249920);
						validChannel = true;
					}

					if (validChannel)
						await logChannel.SendMessage($"User **{e.Before.Name}** changed their name to **{e.After.Name}** ({e.After.Mention})");
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
