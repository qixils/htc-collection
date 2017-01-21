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

			// Register a Hook into the UserJoined event using a Lambda
			_client.UserJoined += async (s, e) => {
				Channel logChannel = e.Server.GetChannel(207659596167249920);
				if(logChannel != null)
					await logChannel.SendMessage($"{e.User.Name} ({e.User.Id}) joined the server.");
			};

			// Register a Hook into the UserUnanned event using a Lambda
			_client.UserLeft += async (s, e) => {
				Channel logChannel = e.Server.GetChannel(207659596167249920);
				if (logChannel != null)
					await logChannel.SendMessage($"{e.User.Name} ({e.User.Id}) left the server.");
			};

			string token = File.ReadAllText("token.txt");
			_client.ExecuteAndWait(async () => {
				await _client.Connect(token, TokenType.Bot);
				Console.WriteLine($"Connected as {_client.CurrentUser.Name}#{_client.CurrentUser.Discriminator}");
			});

		}
	}

}
