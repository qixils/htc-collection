using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.IO;
using Discord;

class Program
{
    static void Main(string[] args) => new Program().Start();

    // start config
    public static List<string> config = File.ReadAllLines("config.txt").ToList(); // config text file but it's a List<string>
    string token = config[0];
    public static string p = config[1]; // bot prefix
    public static ulong mtchannel = Convert.ToUInt64(config[2]); // ID of the mini twow mtchannel
                                                                 // end config

    // variable declaring
    private static DiscordClient _client = new DiscordClient();
    public static System.Timers.Timer mtcreateTimer = new System.Timers.Timer();
    public static int mtstatus = 0;
    public static User mthost;
    public static List<User> mtusers;
    public static List<User> mtvoters;
    public static List<User> mtvoted;
    // end of variables

    // mtstatus values:
    // 0 - no game
    // 1 - game created, waiting for players
	// 2 - game started, awaiting prompt
	// 3 - awaiting prompt responses
	// 4 - voting time

    public void Start()
    {
        mtcreateTimer.Elapsed += new ElapsedEventHandler(mtcreateTimeout);
        mtcreateTimer.Interval = 180 * 1000; // first digit is # of seconds you want timeout to last
        mtcreateTimer.Enabled = false;

        _client.Log.Message += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

        _client.MessageReceived += async (s, e) =>
        {
            bool isCmd = e.Message.Text.StartsWith(p) && !e.Message.Text.StartsWith(p + p) && (e.Channel.Id == mtchannel);
            bool dmCmd = e.Message.Text.StartsWith(p) && !e.Message.Text.StartsWith(p + p) && e.Channel.IsPrivate;
            string msg = e.Message.Text;
            string rawmsg = e.Message.RawText;

            if (isCmd)
            {
                string cmd = msg.Replace(p, "").Split(' ').FirstOrDefault().ToString();
                var args = msg.Replace(p, "").Split(' ').Skip(1).ToArray();

                string mention = e.User.Mention;

                if (cmd == "help")
                {
                    string cmdhelp = $"`{p}help` - lists the bot commands";
					string cmdcreate = $"`{p}create` - create a Mini TWOW";
					string cmdjoin = $"`{p}join` - join the current Mini TWOW";
					string cmdstart = $"`{p}start` - start your Mini TWOW";

                    string cmds = $"\n{cmdhelp}\n{cmdcreate}\n{cmdjoin}\n{cmdstart}";
                    string helpmsg = $"TWOWBot command list: \n{cmds}";

                    await e.Channel.SendMessage(helpmsg);
                }
                else if (cmd == "create")
                {
                    if (mtstatus == 0)
                    {
                        mthost = e.User;
                        mtusers.Add(e.User);
                        mtvoters.Add(e.User);
                        mtstatus++;
                        mtcreateTimer.Enabled = true;
                        await e.Channel.SendMessage($"{mention} has created a Mini TWOW! Type `{p}join` to join, or `{p}spec` to spectate!");
                    }
                    else
                    {
                        await e.Channel.SendMessage($"A game is curently already in progress!");
                    }
                }
                else if (cmd == "join")
                {
                    if (mtstatus == 1)
                    {
                        bool inGame = false;
                        foreach (User usr in mtusers)
                            if (usr.Id == e.User.Id)
                                inGame = true;

                        if (!inGame)
                        {
                            mtusers.Add(e.User);
                            mtvoters.Add(e.User);
                            await e.Channel.SendMessage($"{mention} has been added to the game.");
                        }
                        else
                            await e.Channel.SendMessage($"{mention}: You're already in the game!");
                    }
                    else if (mtstatus == 0)
                        await e.Channel.SendMessage($"There is currently no game running!");
                    else
                        await e.Channel.SendMessage($"The game has already started!");
                }
                else if (cmd == "start")
                {
                    if (e.User.Id == mthost.Id)
                    {
                        if (mtstatus == 1)
                        {
                            mtstatus++;
                            mtcreateTimer.Enabled = false;
                            await e.Channel.SendMessage($"{mention}: The game is now started. No further people can join the game.");
                        }
                        else if (mtstatus == 0)
                        {
                            await e.Channel.SendMessage($"{mention}: ...er. hm. you shouldn't be seeing this message. im just gunna summon <@140564059417346049> to check this out.");
                        }
                        else
                        {
                            await e.Channel.SendMessage($"{mention}: The game has already been started!");
                        }
                    }
                    else if (mtstatus == 0)
                    {
                        await e.Channel.SendMessage($"{mention}: No game is currently running!");
                    }
                    else
                    {
                        await e.Channel.SendMessage($"{mention}: Only the host can start the game!");
                    }
                }
            }
        };


        _client.ExecuteAndWait(async () =>
        {
            await _client.Connect(token, TokenType.Bot);
            Console.WriteLine($"Connected as {_client.CurrentUser.Name}#{_client.CurrentUser.Discriminator}");
        });

    }

    public static string clearformatting(string input)
    {
        var output = "[empty string]"; // set placeholder variable incase input was null
        if (!string.IsNullOrWhiteSpace(input)) // replace various markdown or msg-breaking characters with nothing
            output = input.Replace("`", "'").Replace("'''", "​`​`​`").Replace("*", "​\*").Replace("_", "\​_").Replace("‮", "");
        return output;
    }

    private static void mtcreateTimeout(object source, ElapsedEventArgs e)
    {
        User oldhost = mthost; // create temp variable for old host
        mthost = mtusers.FirstOrDefault(); // change host

        mtusers.Remove(oldhost); // remove host from user list
        mtvoters.Remove(oldhost); // remove host from voters list
        _client.GetChannel(mtchannel).SendMessage($"{oldhost.Mention}: you were inactive for too long, so {mthost.Mention} is now the host of the game.");
    }
}
