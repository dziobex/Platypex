using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PlatypexBot
{
    class Program
    {
        static string token;
        public static string Prefix;

        public static ulong WelcomeID;
        public static DiscordSocketClient client;
        Commands commands;

        static void Main(string[] args)
        {
            using (StreamReader sr = new StreamReader("Info.json"))
            {
                string json = sr.ReadToEnd();
                Dictionary<string, string> results = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                Prefix = results["Prefix"];
                token = results["Token"];
            }

            WelcomeID = 0;
            new Program().StartAsync().GetAwaiter().GetResult();
        }

        public async Task StartAsync()
        {
            client = new DiscordSocketClient();
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            commands = new Commands(client);

            client.UserJoined += Client_UserJoined;
            client.UserLeft += Client_UserLeft;
            await Task.Delay(-1);
        }

        public async Task Client_UserLeft(SocketGuildUser arg)
        {
            SocketTextChannel channel;
            if (WelcomeID != 0)
            {
                channel = client.GetChannel(WelcomeID) as SocketTextChannel;
                await channel.SendMessageAsync($":wave: Bye {arg.Mention}!");
            }
            else Console.WriteLine("WelcomeID not found.");
        }

        public async Task Client_UserJoined(SocketGuildUser arg)
        {
            SocketTextChannel channel;
            if (WelcomeID != 0)
            {
                channel = client.GetChannel(WelcomeID) as SocketTextChannel;
                await channel.SendMessageAsync($"Hello {arg.Mention}!");
            }
            else Console.WriteLine("WelcomeID not found.");
        }
    }
}
