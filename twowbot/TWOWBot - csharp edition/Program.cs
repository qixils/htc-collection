using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Modules;
using Discord.Audio;
using System.IO;
using System.Timers;

class Program
{
    static void Main(string[] args) => new Program().Start();

    private DiscordClient _client;
    public static ulong CreatorID;
    public static string PromptInput = "";
    public static int RoundNumber;
    public static int PlayerCount;
    public static ulong BotHost = 140564059417346049;
    public static bool VotingTime = false;
    public static bool MiniParticipating = false;
    public static int CurrentSecond;
    public static int MiniTime;
    public static DateTime CurrentTime;
    public static int Minute;
    public static int Second;
    public static bool SubmissionTime = false;
    public static Channel MiniChannel;

    public async void Start()
    {
        _client = new DiscordClient();
        Console.WriteLine("Booting up bot...");

        _client.UsingCommands(x =>
        {
            x.PrefixChar = '-';
            x.HelpMode = HelpMode.Public;
        });

        _client.Log.Message += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

        int MiniPhase = 0;
        CurrentTime = DateTime.Now; //grab current system time
        Minute = CurrentTime.Minute; //grab current second
        Second = CurrentTime.Second; //grab current second
        List<User> MiniPlaying = new List<User>(); // create list for users playing a mini twow
        List<string> SubmitterID = new List<string>(); // create list for users submitting responses
        List<string> SubmitterResponse = new List<string>(); // create list for prompt responses

        _client.MessageReceived += async (s, e) =>
        {
            // Check to make sure that the bot is not the author
            if (e.Message.Text.ToLower() == "-mini")
                await e.Channel.SendMessage($"{e.User.Mention}, try using `-mini help`.");
            else if (e.Channel.IsPrivate)
            {
                if (e.Message.IsAuthor) { }
                else if (e.Message.Text.ToLower().Contains("-mini"))
                {
                    await e.Channel.SendMessage($"You cannot run -mini commands in DM's.");
                }
            }
        };
        while (MiniTime == 60)
        {
            MiniTime = 0;
            if (VotingTime == false)
            {
                VotingTime = true;
                MiniPhase++;
                await MiniChannel.SendMessage($"**Round {RoundNumber}** voting has begun!\n" +
                                             "**Prompt: " + PromptInput + "**\n" +
                                             "Remember, you viewers can vote whether you're a contestant or not!\n" +
                                             "If you're not a player or spectator, to vote use `-mini spec`\n" +
                                             "There is 1 minute to vote! Get going!\n" +
                                             "If you did NOT get the options, use `-mini options`. Sometimes options can take a little bit to get sent to you, though.");
                PromptInput = "";
                foreach (User u in MiniPlaying)
                {
                    await u.SendMessage("its been 1 min");
                }
            }
        }

        _client.GetService<CommandService>().CreateCommand("info") //create command
                .Alias(new string[] { "botinfo", "twowbotinfo" }) //add 2 aliases
                .Description("Displays bot info.") //add description, it will be shown when ~help is used
                .Do(async e =>
                {
                    await e.Channel.SendMessage($"Heya {e.User.Mention}. I'm TWOWBot, nice to meet you! I was coded in C# by Noahkiq. You can check out my github page if you're interested at https://github.com/Noahkiq/TWOWBot");
                    //sends a message to channel with the given text
                });

        _client.GetService<CommandService>().CreateCommand("invite") //create command
                .Alias(new string[] { "joinserver", "join" }) //add aliases
                .Description("Posts the link to get the bot on your server.") //add description
                .Do(async e =>
                {
                    await e.Channel.SendMessage($"You can add me to your server using https://discordapp.com/oauth2/authorize?client_id=222869815650418690&scope=bot - make sure you have `Manage Server` permissions!");
                    //sends a message to channel with the given text
                });

        _client.GetService<CommandService>().CreateCommand("msg-test") //create command
               .Hide()
               .Description("sends a pm to the user that ran the command") //add description
               .Do(async e =>
               {
                   await e.User.SendMessage("hi " + e.User.Name);
                   //sends a message to channel with the given text
               });

        _client.GetService<CommandService>().CreateCommand("twowr") //create command
                .Alias(new string[] { "twowrandomizer" }) //add alias
                .Description("Starts a TWOW Randomizer.") //add description
                .Do(async e =>
                {
                    string seasonString = File.ReadAllText("season.txt");
                    int season = Convert.ToInt32(seasonString);
                    season++;
                    File.WriteAllLines("season.txt", new string[] {$"{season}"});
                    var response = new List<string> { "Meester Tweester", "Joseph Howard", "Crafty7", "Yessoan", "Ping Pong Cup Shots", "Tak Ajnin", "TheMightyMidge", "Mike Ramsay", "Spicyman33", "Midnight Light", "GreenTree", "QwerbyKing", "taopwnh6427", "some_nerd (The Pi Guy)", "Riley", "fryUaj", "alexlion0511", "Sam Billinge", "xXBombs_AwayXx", "The Futech Hacker", "steveminecraft46", "Jennings AsYetUntitled", "rinkrat02", "Ean EStone", "beanme100", "THATcommentor", "FlareonTheFlareon", "Juhmatok", "Quinn Ruddy", "DeeandEd", "Ni Hao Guylan", "Stalemate", "pokemonmanic3595", "Christian deWeever", "John Petrucci", "John Dubuc", "NitroNinja", "Smileyworkscompany", "chichinitz", "TimVideo 326", "AnimationCreated", "legotd61", "Ulises Castillo", "Ronan Sandoval", "Not Pro", "Will Zhang", "Mariobrosaa Productions", "The All Rounder | Marble Races", "WhoNeedsAName", "Erikfassett", "Diamondcup67", "Endr Dragon", "DanTheStripe", "Henry Scott", "BlueLucario98", "Reid Pozzi", "Carrotkid08", "Ben1178", "Thecatsonice", "Truttle1", "Aaron Rubin", "Aiden .C", "TehPizzaMan", "LucidSigma", "1Kick 234", "Xenon Virus", "JayBud", "RAM Turtle", "dancingfb18", "RedAdamA (Anything Ever)", "greenninjalizard123", "The Slime Bros", "Brand Utger", "swimswum", "Mapmaker Mapping (Parker Pearmain)", "BFDIBOYERSFTW", "Conor OMalley", "SergeantSnivy", "RainingPlates", "InfinitySnapz", "Hazel Cricket", "goldenzoomi", "Milo Jacquet", "LengthxWidth", "edtringaming", "funkydoge", "TheKoolkid209", "jerri76 likes trees", "AnyatheArtist", "Theo L", "American Camper", "NoobWithAFez", "zoroark SixtyFive", "Tyler Chai", "AugustMK7", "ItsSoooooFluffy", "Ali161102", "Bazinga_9000", "book81able", "Whiskerando", "XclockXanimations", "Jeelhu77", "zRAGE", "~Mr Sunny~", "GudPiggeh", "ZerOcarina", "John Mars (4DJumpman)", "TheMagicalKBMan", "TTGuy10000", "funnyboy044", "ahahahajee", "Villager #9", "ButterFlamingo", "Victini and Infinity Productions", "SeanyBoy", "Andyman620", "WindowVoid", "Vivek Saravanan", "Jack Spero", "DaKillahAidan", "Jacoub The Person", "Max McKeay", "MMMIK13", "Izumi Yoshida", "Paintspot Infez", "Lpcarver", "deepdata1", "Duck Wrath", "Roston11", "J_duude", "Ieatpie Prodcutions", "tumble", "james cooper", "AnEpikReshiramRBLX - Probably Crappy Machinimas!", "DylanMultiProduction", "Experimental Account", "SuperTurtle408", "Tile is kuel", "Poilik098", "Geometry Dash BlokPurrsun", "SolarBolt4 (TheShinyRayquaza47)", "PhantomJax", "PacoMan", "wifishark", "RockDudeMKW", "Sam Baker (thecool1204)", "Sqlslammer", "Timm638", "SubscribeSubscribedUnsubscribe", "David Rycan", "Zachary Ridall", "iAnimate38", "random57877", "YearsAnimations", "mmKALLL", "Varth_EDM", "XO Mapping", "elfireball42", "Wyatt Buss", "Ploot Adrain", "Thedrievienden", "RunToastRun", "Reviloja753 (Magnavox)", "cahlos 11", "2NoobFriendz", "richcolour", "Mar View", "snowruntlvr", "TardistheTardis", "lempamo", "Justyn Rodriguez", "I made this channel for TWOW", "Sean Leong", "TheUtubedude101", "GoldenMinecraft 29", "Peter N", "Bryan Rodin (bryshmy126)", "Loong Yaw Lee", "Luke C. (PenBFDI)", "cool chansey", "AnonymousMango", "ShinyStoutland", "KandyCreeper", "Roger Houses", "10gamerguy", "YellowJellow12 Gaming", "RidgeRR4", "Meduza yt", "Emation 1", "6j108", "Roman Neill", "bling popo", "ScienceFreakProdcutions", "Seth Rollins", "Awesomeness765", "loɹʇuoɔ soɐɥɔ", "MineWraith", "Donnie Melton", "Law Dog", "TheAwesomeDude", "Mae Young", "Citing Ostrich (StridentBoss)", "Gabe LaSalle", "BinaryBubbleGaming", "Pie Plays", "Stephen Kamenar", "catsanddogs333", "Oscar Field", "Cid Styles", "matheweon", "Thecactigod Oh", "Vaughn P", "Andrew Ratzlaff", "CrystaltheCool", "sc9849", "Battle for Something Official (BFST)", "Austin Eastwood", "KirbyRider1337", "justthebomb \"justthebo50\" blue", "Quinson Hon", "JerusalemStrayCat", "MasterOfZoroark", "Sawygo", "Bunpuffy", "MrMGamer", "Wolfster J", "Bendariaku", "Kenneth Ruff", "WildKat", "pokpower8", "evan skolnick", "TheDonuts42", "Devoid Amoeba", "Vcr-San", "Isaiah Y", "Krangle", "Mista Fown", "Kathie Bennett", "Brady Chan", "Awesome Animator", "Snoop Frogg", "Krishna Mandal", "bubblestars", "Lyndon Fan", "TheITChap", "livingpyramid", "Harry Odiakosa", "Computer 2468", "Stadin6", "WhaiJay", "FrogEatsGames", "minecraftwithkitties", "Justin Lo", "Wobbuffet64", "TheGenoYoshi", "Cyffreddinol", "Shaymin Lover", "Phant0mInfinity", "Huri Churi", "stephan Eijgelaar", "Sam-Fone Cheung", "GizmotheGamer", "REX CAVALIER (Sprite)", "Pandadude12345", "Matthew Doan", "Tantusar", "Ann Ant", "Izach Castro", "Carroll Addington", "misterjakester", "Scrooty 6362161296", "SkyBox101", "dakillahbunnyz", "Marco Mora lorca", "AnimationEpic", "CyanDrop", "Theelementalraccoon", "StarSphere08", "Leo O'Connor", "Jacob Thomas", "Bahgg Muhhhbah (of the many realms)", "lorri peterman", "Objectdude73", "Object Awards", "sharunkis", "BeastModeON !", "IzlePox", "2004froggy CP AJ", "Jack Orange", "NOT a gaming channel", "DONT VOTE IN THE COMMENTS", "TheKimpesShow", "basedzach", "geisterfurz007", "Cactus Power", "FireProofPotatoes", "Aliendude 321", "WhattheDerp", "ElectricPichu", "Minecraft Player", "TheNamelessChannel", "Rainbow drugg", "NumberDerp", "pokemonwalkingdead", "Nazrininator", "shannon spriggs", "nathan T", "Hugheseyboy 103", "CobaltGameMixer676", "PlasmaEmpire", "Novakobx22", "Br Miller", "CrazyCyanWaffle", "AsrielAvenue", "Silver Koopa888", "Brandon Olague", "Lukas Chan", "Miniwa the adventurer", "Cy Duck", "CubeBag", "Mr Leafy The Leafeon2", "AnimationArtist", "CommentingEevee", "retro boy", "Minh Nguyễn Nhật", "Mason Liu (ThePiGuy31415926535)", "MatrVincent- Servicing the Object Community!", "Denis Kogevnikov", "A Doctor Who", "akka777", "Kyle Stubbs", "Andy Pham", "Colin Ho", "Ruby Ninja", "ישראלה מויאל", "Nate Animations", "Pie Mc Pie", "Mudkipian Emporer", "SupremeGoldenRockies", "bra me", "brettsmart58", "Aqua Vulpes", "dombie brains", "TheRedArmy", "Coo haa", "east3myway", "M&M Awesome", "Thatkomedykid - great comedy videos!", "Auto Cats", "SBem14", "TDSwaggyBoy", "danielordeath \"BFRI SOON\" 2015", "Magical Genie", "Ue Hang Wong", "Phantastrophy", "Jacob Ward", "Joseph ~ TNTcreeper", "Mortuary Manager", "TheTGrodz", "Loftix", "Noah Alperovitz", "Caspar Coster", "thesuperbouncyballs", "the worst person in the object community", "Ken \"Kidsy 128\"", "Jake Beaulieu (Jakegames2)", "MrTacoLazer", "ღEeveeLikesToastღ", "Kyle Lazorko", "MICHAEL MCQUAY", "YpCoProductions", "Anton Christensen", "WebzForevz / Excellent Entities !", "coolguy20000000", "Brendan Glascock", "MetalMan Studios", "4613theo", "lilczar00", "Tornusge Bladestorm", "PuffQuit", "KennyPlex InterWorld", "SkyMix_RMT", "hyates animations", "SnowballBFDI", "Grand Landmaster", "M00ZE", "RedTedGaming", "정보규", "josh9969", "Faith Erdem Kɪᴢɪlkaya", "Sam Lee", "Stanley Huh", "Rogers Six", "Achoo", "Timothy Salaway", "joshuathemagician (Joshua Mackenzie)", "Voyager Studio", "Nick Grant", "EvilOrso", "Nathan Hon", "Leonardo Viera de Souza", "Speedgoy", "Recent .Snowfall", "Awesomez53", "SilverLighter 11", "Vincent Valor", "pegasus6540", "Raylover9462", "CheckerboardsMC", "Oprah gives you a Flowey meme in your face", "Toad8508", "SoocieSoocie", "Numero Uno Apple", "KID GAMING!", "Kasper Christensen", "Tachibanana", "Official Confused Sign (OCS)", "Lautrix Gaming", "EmperorMagenta", "piplupfan32", "Eviyon Wygant", "CampsOfficial 123", "Nico Balint", "EverestAnimationz", "Lance Craig", "Silver Knight", "Firey", "ObjectsVsMinecraft", "PENGUN STUDIOS", "Dan \"rubberducky8000\" C.", "IronCow Tom", "Dalton Bledrose", "Panzer Inversion", "Lance Alip", "Wintar", "FUUUUman01", "pokelol97", "Popcorn", "Ferok", "Ryan Chow", "zie mij gamen", "Kyle Hubbard", "YTPmachine123", "Mikenkanikal", "RedBreloom", "naltronix", "This is my name", "Henrique Prestes", "MangledObject", "BlackPhanth0m", "vladik huk", "Minsel", "ChavoMixel Commentry", "Jamie Keane", "[MNI]TronCrusher", "kurpingspace productions", "Joshua Wong", "William Sze-to", "Rasengan Master 23", "Shattrdpixel", "stanley Jr. Huh", "Jacob Schuøszernouszęl 9001", "theawesomenessdude", "Samer Gamer", "The Fun Gang", "hamizannaruto", "Ashr", "Cool Mac Gaming", "wholewheat", "kaycalgamer 53", "bloon 104", "Nathan Show", "HeyLookInsteadIMadeAChannel !", "firstname iskowitz", "OmegaPsi", "Total Craft", "Dj Beta", "HVLetsPlay", "Animation Field", "sandroshubitidize HAH", "Kashif Chaudhry", "Samuel Argar", "WaterBrickCraft WBC DoubleUBeeCee", "Tai Mau", "ads13000", "Nashriq Akmal", "Alex Canine", "Geometry Dash Demonsodemonic", "Jaime Mora Lorca", "Bannapal Dog", "A.N.9K" };
                    await e.Channel.SendMessage($"Welcome everyone to TWOW #{season}! We have a huge list of contestants, so many I can't even list them all, so let's just see who's being eliminated this first episode.");

                    //sends a message to channel with the given text
                });

        _client.GetService<CommandService>().CreateGroup("mini", cgb =>
        {
            cgb.CreateCommand("help")
                        .Description("Displays list of subcommands.")
                        .Do(async (e) =>
                        {
                            await e.Channel.SendMessage($"__Mini TWOW Subcommands:__\n\n" +
                                                      "`create` - Create a Mini TWOW.\n" +
                                                      "`join` - Join a Mini TWOW.\n" +
                                                      "`start` - Start a Mini TWOW.");
                        });

            cgb.CreateCommand("debug")
                        .Description("Displays debug info about Mini TWOWs.")
                        .Hide()
                        .Do(async (e) =>
                        {
                            if (e.User.Id == BotHost)
                            {
                                await e.Channel.SendMessage($"__Debug Info:__\n" +
                                                          "CreatorID: `" + CreatorID + "`\n" +
                                                          "MiniPhase: `" + MiniPhase + "`\n" +
                                                          "PromptInput: `" + PromptInput + "`\n" +
                                                          "RoundNumber: `" + RoundNumber + "`\n" +
                                                          "PlayerCount: `" + PlayerCount + "`\n" +
                                                          "MiniTime: `" + MiniTime + "`\n");
                            }
                            else
                            {
                                await e.Channel.SendMessage($"You are not permitted to use this command!");
                            }
                        });

            cgb.CreateCommand("create")
                    .Description("Create a Mini TWOW.")
                    .Do(async (e) =>
                    {
                        //checks if a mini twow is in progress
                        if (MiniPhase > 0)
                        {
                            await e.Channel.SendMessage($"A Mini TWOW is already in progress.");
                            //sends message
                        }
                        else
                        {
                            Program.CreatorID = e.User.Id;
                            MiniPhase++; //adds 1 to "MiniPhase" int
                            await e.Channel.SendMessage($"{e.User.Mention} has created a Mini TWOW!\n" +
                                                 "Use `-mini join` to join.\n" +
                                                 "Use `-mini leave` to leave.\n" +
                                                 "Use `-mini spec` to spectate.");
                        }
                    });

            cgb.CreateCommand("join")
                        .Description("Join a Mini TWOW.")
                        .Do(async (e) =>
                        {
                            //check if game has started
                            if (MiniPhase > 1)
                            {
                                await e.Channel.SendMessage($"The game has already started.");
                                //send message
                            }
                            else if (MiniPhase == 0)
                            {
                                await e.Channel.SendMessage($"There is no game running yet! Try doing `-mini create` first.");
                                //surprisingly, this sends a message.
                            }
                            else
                            {
                                string UserID = e.User.Id.ToString();  //create string for user's id
                                foreach (var item in MiniPlaying)
                                {
                                    if (item == e.Server.GetUser(e.User.Id))
                                    {
                                        MiniParticipating = true;
                                    }
                                }
                                //^ check if user's id is in the miniplaying list already

                                if (MiniParticipating == true)
                                {
                                    await e.Channel.SendMessage($"{e.User.Mention}, you've already joined the game!");
                                    //send message
                                }
                                else
                                {
                                    PlayerCount++;
                                    await e.Channel.SendMessage($"{e.User.Mention} has joined the game."); //send message
                                    MiniPlaying.Add(e.User.Server.GetUser(e.User.Id));
                                    MiniParticipating = false;
                                }
                            }
                        });

            cgb.CreateCommand("leave")
                        .Description("Leave a Mini TWOW.")
                        .Do(async (e) =>
                        {
                            string UserID = e.User.Id.ToString();  //create string for user's id
                            foreach (var item in MiniPlaying)
                            {
                                if (item == e.Server.GetUser(e.User.Id))
                                {
                                    MiniParticipating = true;
                                }
                            }
                            //^ check if user's id is in the miniplaying list already

                            if (MiniParticipating == false)
                            {
                                await e.Channel.SendMessage($"{e.User.Mention}, you're not in a Mini TWOW!");
                                //send message
                            }
                            else
                            {
                                PlayerCount--;
                                await e.Channel.SendMessage($"{e.User.Mention} has left the game."); //send message
                                MiniPlaying.Remove(e.User.Server.GetUser(e.User.Id));
                                MiniParticipating = false;
                            }
                        });

            cgb.CreateCommand("start")
                        .Description("Start a Mini TWOW.")
                        .Do(async (e) =>
                        {
                            //check if game has started
                            if (MiniPhase > 1)
                            {
                                await e.Channel.SendMessage($"The game has already started.");
                                //send message
                            }
                            else if (MiniPhase == 0)
                            {
                                await e.Channel.SendMessage($"There is no game running yet! Try doing `-mini create` first.");
                                //surprisingly, this sends a message.
                            }
                            else
                            {
                                if (e.User.Id != Program.CreatorID)
                                {
                                    await e.Channel.SendMessage($"Only the game creator can use this command.");
                                }
                                else
                                {
                                    if (PlayerCount > 1)
                                    {
                                        MiniPhase++; //add one to MiniPhase
                                        await e.Channel.SendMessage($"The game has been started. Nobody else is allowed to join.");
                                    }
                                    else
                                    {
                                        await e.Channel.SendMessage($"You need atleast 2 players to start the game.");
                                    }
                                }
                            }
                        });

            cgb.CreateCommand("prompt")
                        .Description("Enter the round's prompt.")
                        .Parameter("Prompt", ParameterType.Unparsed)
                        .Do(async (e) =>
                        {
                            //check if game is running
                            if (MiniPhase == 0)
                            {
                                await e.Channel.SendMessage($"There is no game running yet! Try doing `-mini create` first.");
                                //surprisingly, this sends a message.
                            }
                            //check if command runner is creator
                            else if (e.User.Id != CreatorID)
                            {
                                await e.Channel.SendMessage($"Only the game creator can run this command!");
                            }
                            //check if game has started yet
                            else if (MiniPhase == 1)
                            {
                                await e.Channel.SendMessage($"You need to start the game first. (`-mini start`)");
                                //send message
                            }
                            else
                            {
                                if (PromptInput == "")
                                {
                                    if (e.GetArg("Prompt") != "")
                                    {
                                        MiniChannel = e.Channel.Server.GetChannel(e.Channel.Id);
                                        CurrentSecond = Second;
                                        RoundNumber++;
                                        MiniPhase++;
                                        PromptInput = e.GetArg("Prompt");
                                        await e.Channel.SendMessage($"**Round {RoundNumber} prompt** ({PlayerCount} players):\n" +
                                                                     "**" + PromptInput + "**\n" +
                                                                     "You have 2 minutes!\n" +
                                                                     "If you are a player, send me a private message: `-mini submit <response>`\n" +
                                                                     "e.g. `-mini submit Anything relating to the prompt in ten words or fewer!`\n");
                                        foreach (User u in MiniPlaying)
                                        {
                                            await u.SendMessage("hi");
                                        }

                                        System.Timers.Timer aTimer = new System.Timers.Timer();
                                        aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                                        aTimer.Interval = 1000;
                                        aTimer.Enabled = true;
                                    }
                                    else
                                    {
                                        await e.Channel.SendMessage($"You must enter a prompt.");
                                    }
                                }
                                else
                                {
                                    await e.Channel.SendMessage($"It's not time to enter a prompt yet!");
                                }
                            }
                        });
        });

        _client.GetService<CommandService>().CreateCommand("userinfo")
                .Description("Displays info about a user.")
                .Parameter("User", ParameterType.Optional)
                .Do(async e =>
                {
                    string mension = e.GetArg("User");
                    ulong id = e.User.Id;
                    string username = e.User.Name;
                    string avatar = e.User.AvatarUrl;
                    Discord.User roles = (Discord.User)e.User.Roles;
                    if (e.GetArg("User") != "")
                    {
                        if (mension.Contains("!"))
                            id = ulong.Parse(mension.Split('!')[1].Split('>')[0]);
                        else
                            id = ulong.Parse(mension.Split('@')[1].Split('>')[0]);

                        username = e.Server.GetUser(id).Name;
                        avatar = e.Server.GetUser(id).AvatarUrl;
                        roles = (Discord.User)e.Server.GetUser(id).Roles;
                    }

                    await e.Channel.SendMessage($"```\nID:       {id}\n" +
                                                     $"Username: {username}\n" +
                                                     $"Roles: {roles}\n```" +
                                                     $"\n{avatar}\n");
                });

        string token = File.ReadAllText("token.config");
        _client.ExecuteAndWait(async () =>
        {
            await _client.Connect("Bot " + token);
            _client.SetGame("some Mini TWOWs. And losing. :(");
        });
    }
    private static void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        MiniTime++;
    }
}
