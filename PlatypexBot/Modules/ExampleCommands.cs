using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reddit;
using Reddit.Controllers;

namespace PlatypexBot.Modules
{
    public partial class ExampleCommands : ModuleBase<SocketCommandContext>
    {
        IGuildUser currentUser;
        [Command("Platy")]
        public async Task What() => await Context.Channel.SendMessageAsync("what?");

        [Command("meme")]
        public async Task Meme(bool special = false)
        {
            RedditClient reddit = new RedditClient("MXPB2_xcoBgEaA", "607075880651-xTcpUUS_Auvt8qPp0zRYBJYiRb4");
            Post post = reddit.Subreddit(special ? "ProgrammerHumor" : "dankmemes").Posts.Hot[Program.Randomizer.Next(0, 99)];

            var embed = new EmbedBuilder();
            embed.Title = $"{post.Title}";
            embed.Url = ((LinkPost)post).URL;
            embed.Color = Color.Green;
            embed.ImageUrl = ((LinkPost)post).URL;
            await ReplyAsync(null, false, embed.Build());
        }

        [Command("howgay"), Alias("gay")]
        public async Task Gay(IUser user=null)
        {
            if (user == null) user = Context.Message.Author;
            int pc = Program.Randomizer.Next(0, 100);
            string[] emotes = new string[] { ":smirk:", ":kissing_heart:", ":heart:", ":rainbow_flag:" };

            string reaction = "";
            if (pc <= 30) reaction = new string[] { "You re not a gay.", "NO-HOMO", "Nothing special **bro**" }[Program.Randomizer.Next(3)];
            else if (pc <= 60) reaction = new string[] { "A little gay.", "huh you smell like a gay.", "huhu Are you a gay? :smirk:" }[Program.Randomizer.Next(3)];
            else reaction = new string[] { "GAY ALERT", "My computer says you are a gay, aren't u?", "GAY ~100%", "GAY GAY GAY scuyw ( ͡° ͜ʖ ͡°)" }[Program.Randomizer.Next(4)];

            var embed = new EmbedBuilder();
            embed.Title = $"{pc}% gay {emotes[Program.Randomizer.Next(emotes.Length)]}";
            embed.Color = new Color(0xFFC0CB);
            embed.WithImageUrl("https://upload.wikimedia.org/wikipedia/commons/thumb/4/48/Gay_Pride_Flag.svg/1024px-Gay_Pride_Flag.svg.png");
            embed.AddField(name: $"Gay in {pc}%", value: $"{user.Mention}, {reaction}");
            await ReplyAsync(null, false, embed.Build());
        }

        [Command("help")]
        public async Task Help()
        {
            var embed = new EmbedBuilder();
            var sb = new StringBuilder();

            embed.WithColor(new Color(0, 255, 0));
            embed.Title = $"Hello {Context.Message.Author.ToString().Remove(Context.Message.Author.ToString().Length-5)}!";

            sb.AppendLine($"Use `pp [command]` to use any command.");
            sb.AppendLine($":tools: `Configuration`: \n" +
                $"`config (set chan where bot'll send welcome/goodbye messages)`");
            sb.AppendLine($":wrench: `Moderation`: \n" +
                $"`kick, ban, getroles, deletechannel, deletemessages, ownnick (change your own nick), nick (change sb's nick)`");
            sb.AppendLine($":smile: `Fun`: \n" +
                $"`howgay [user], meme [true if you want to see special kind of meme]`");
            embed.Description = sb.ToString();
            await ReplyAsync(null, false, embed.Build());
        }

        [Command("config")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Config()
        {
            Program.client.MessageReceived += Client_MessageReceived;
            await Context.Channel.SendMessageAsync("Where i should send welcome/goodbye messages?");
            currentUser = Context.User as IGuildUser;
        }

        public  async Task Client_MessageReceived(SocketMessage arg)
        {
            if (arg.Content.All(x => char.IsDigit(x)) & arg.Content.Length == 18)
            {
                if (arg.Author == currentUser)
                {
                    await arg.AddReactionAsync(new Emoji("\uD83D\uDC4D"));
                    Program.WelcomeID = Convert.ToUInt64(arg.Content);
                    Program.client.MessageReceived -= Client_MessageReceived;
                    Console.WriteLine($"{Program.WelcomeID}");
                }
            }
        }
    }
}
