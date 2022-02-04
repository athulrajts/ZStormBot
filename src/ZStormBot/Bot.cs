using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenDotaApi;
using ZStormBot.Interfaces;
using ZStormBot.Services;

namespace ZStormBot;

internal class Bot
{
    private readonly CommandsNextExtension _commands;
    private readonly DiscordClient _client;

    private static IServiceProvider ConfigureServices(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false)
               .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<OpenDota>();
        services.AddSingleton<IDotaConstants, DotaConstants>();
        services.AddSingleton<IUserRepository, UserRepository>();
        services.AddSingleton<ISummaryImageRepository, SummaryImageRepository>();
        services.AddSingleton<ISoundRepository, SoundRepository>();

        return services.BuildServiceProvider();
    }

    public Bot()
    {        
        var services = ConfigureServices(new ServiceCollection());
        var configuration = services.GetRequiredService<IConfiguration>();

        var token = configuration.GetValue<string>(AppSettings.BotToken);
        var prefix = configuration.GetValue<string>(AppSettings.BotPrefix);


        var ccfg = new CommandsNextConfiguration
        {
            StringPrefixes = new[] { prefix },
            EnableDms = true,
            EnableMentionPrefix = true,
            Services = services
        };

        var cfg = new DiscordConfiguration
        {
            Token = token,
            TokenType = TokenType.Bot,
            AutoReconnect = true,
            MinimumLogLevel = LogLevel.Debug
        };

        _client = new DiscordClient(cfg);

        _client.Ready += Client_Ready;
        _client.GuildAvailable += Client_GuildAvailable;
        _client.ClientErrored += Client_ClientError;

        _commands = _client.UseCommandsNext(ccfg);
        _client.UseVoiceNext();
        _commands.CommandExecuted += Commands_CommandExecuted;
        _commands.CommandErrored += Commands_CommandErrored;
        _commands.RegisterCommands(typeof(Program).Assembly);

    }

    public async Task StartAsync()
    {
        await _client.ConnectAsync();
        await Task.Delay(-1);
    }

    Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
    {
        // let's log the fact that this event occured
        sender.Logger.LogInformation("Client is ready to process events.");

        // since this method is not async, let's return
        // a completed task, so that no additional work
        // is done
        return Task.CompletedTask;
    }

    Task Client_GuildAvailable(DiscordClient sender, GuildCreateEventArgs e)
    {
        // let's log the name of the guild that was just
        // sent to our client
        sender.Logger.LogInformation("Guild available: {GuildName}", e.Guild.Name);

        // since this method is not async, let's return
        // a completed task, so that no additional work
        // is done
        return Task.CompletedTask;
    }

    Task Client_ClientError(DiscordClient sender, ClientErrorEventArgs e)
    {
        // let's log the details of the error that just 
        // occured in our client
        sender.Logger.LogError(e.Exception, "Exception occured");

        // since this method is not async, let's return
        // a completed task, so that no additional work
        // is done
        return Task.CompletedTask;
    }

    Task Commands_CommandExecuted(CommandsNextExtension sender, CommandExecutionEventArgs e)
    {
        // let's log the name of the command and user
        e.Context.Client.Logger.LogInformation("{UserName} successfully executed '{CommandName}'", e.Context.User.Username, e.Command.QualifiedName);

        // since this method is not async, let's return
        // a completed task, so that no additional work
        // is done
        return Task.CompletedTask;
    }

    async Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
    {
        // let's log the error details
        var message = $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}";
        e.Context.Client.Logger.LogError(message);

        // let's check if the error is a result of lack
        // of required permissions
        if (e.Exception is ChecksFailedException)
        {
            // yes, the user lacks required permissions, 
            // let them know

            var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

            // let's wrap the response into an embed
            var embed = new DiscordEmbedBuilder
            {
                Title = "Access denied",
                Description = $"{emoji} You do not have the permissions required to execute this command.",
                Color = new DiscordColor(0xFF0000) // red
            };

            await e.Context.RespondAsync(embed);
        }
    }
}
