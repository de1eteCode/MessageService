using MediatR;
using MessageService.TelegramService.Common.Enums;
using MessageService.TelegramService.Common.Extends;
using MessageService.TelegramService.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using TelegramChat = Telegram.BotAPI.AvailableTypes.Chat;
using TelegramUser = Telegram.BotAPI.AvailableTypes.User;

namespace MessageService.TelegramService.Handlers;

internal class MessageHandler : ITelegramUpdateHandler<Message> {
    private static CommandHelper _commandHelper = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly IMediator _mediator;
    private readonly ILogger<MessageHandler> _logger;
    private readonly IEnumerable<IValidator> _telegramValidators;
    private readonly BotClient _botClient;

    public MessageHandler(
        IServiceProvider serviceProvider,
        IMediator mediator,
        ILogger<MessageHandler> logger,
        IEnumerable<IValidator> telegramValidators,
        BotClient botClient) {
        _serviceProvider = serviceProvider;
        _mediator = mediator;
        _logger = logger;
        _telegramValidators = telegramValidators;
        _botClient = botClient;
    }

    private async Task<bool> IsValidAndError<T>(TelegramUser user, T obj, TelegramChat tgChat)
        where T : class, ITelegramRequest {
        var listOfRes = new List<ValidatorResult>();
        foreach (var validator in _telegramValidators) {
            var curRes = await validator.IsValidAsync(user, obj);
            listOfRes.Add(curRes);
        }

        if (listOfRes.Any(e => e == ValidatorResult.Allow)) {
            return true;
        }

        if (listOfRes.Any(e => e == ValidatorResult.Deny)) {
            await _botClient.SendMessageAsync(tgChat.Id, "У Вас не достаточно прав для выполнения данной операции");
            return false;
        }

        return true;
    }

    public Task HandleUpdate(Message handleUpdate, UpdateType updateType, CancellationToken cancellationToken) {
        var chat = handleUpdate.Chat;

        if (chat == null) {
            return Task.CompletedTask;
        }

        Func<Message, CancellationToken, Task> handler = chat.GetTypeChat() switch {
            TypeChat.Private => HandlePrivateChat,
            TypeChat.Group => HandleGroupChat,
            TypeChat.SuperGroup => HandleSuperGroupChat,
            TypeChat.Chanel => HandleChanelChat,
            _ => throw new NotImplementedException()
        };

        return handler(handleUpdate, cancellationToken);
    }

    private Task HandlePrivateChat(Message handleUpdate, CancellationToken cancellationToken) {
        var text = handleUpdate.Text!;

        if (string.IsNullOrEmpty(text)) {
            return Task.CompletedTask;
        }

        var command = _commandHelper.Match(text);

        if (command.Success == false) {
            return Task.CompletedTask;
        }

        var allowCommands = (IEnumerable<ITelegramRequest>)_serviceProvider.GetServices(typeof(ITelegramRequest));

        if (command.Command.Equals("getcommands")) {
            var sb = new StringBuilder();
            sb.AppendLine("Доступные команды");
            foreach (var cmd in allowCommands.Select(e => e.BotCommand).OrderBy(e => e.Command)) {
                sb.AppendLine(string.Format("- /{0} - {1}", cmd.Command, cmd.Description));
            }

            return _botClient.SendMessageAsync(handleUpdate.Chat.Id, sb.ToString());
        }

        var request = allowCommands.FirstOrDefault(e => e.BotCommand.Command.Equals(command.Command));

        if (request == null) {
            return Task.CompletedTask;
        }

        if (IsValidAndError(handleUpdate.From!, request, handleUpdate.Chat!).Result == false) {
            return Task.CompletedTask;
        }

        var typeBuilder = typeof(ITelegramRequestParamsBuilder<>).MakeGenericType(request.GetType());

        var paramsBuilder = _serviceProvider.GetService(typeBuilder); /// <see cref="ITelegramRequestParamsBaseBuilder<>"/>

        if (paramsBuilder == null || paramsBuilder.GetType().IsAbstract) {
            throw new Exception($"Not found pasrams builder for {request.GetType().Name}");
        }

        var fillParamsMth = paramsBuilder.GetType().GetMethod("BuildParams") ?? throw new Exception();

        fillParamsMth.Invoke(paramsBuilder, new object[] { new Telegram.BotAPI.GettingUpdates.Update() { Message = handleUpdate }, command.Args, request });

        return _mediator.Send(request);
    }

    private Task HandleGroupChat(Message handleUpdate, CancellationToken cancellationToken) => Task.CompletedTask;

    private Task HandleSuperGroupChat(Message handleUpdate, CancellationToken cancellationToken) => Task.CompletedTask;

    private Task HandleChanelChat(Message handleUpdate, CancellationToken cancellationToken) => Task.CompletedTask;

    private class CommandHelper {
        private const string COMMAND = "command";
        private const string PARAMS = "params";
        private const string BASE_COMMAND_PATTERN = @"^\/(?<command>\w+)(?:|@{0})(?:$|\s(?<params>.*))";
        private readonly Regex _regex;

        public CommandHelper([Optional] bool useRegexCompiled) {
            var options = RegexOptions.None;

            if (useRegexCompiled) {
                options = RegexOptions.Compiled;
            }

            _regex = new Regex(BASE_COMMAND_PATTERN, options);
        }

        public CommandMatch Match(string text) {
            if (string.IsNullOrEmpty(text)) {
                return CommandMatch.Empty;
            }

            var match = _regex.Match(text);
            if (match.Success) {
                var cmd = match.Groups[COMMAND].Value;
                var @params = match.Groups[PARAMS].Value;
                return new CommandMatch(cmd, @params, true);
            }
            else {
                return CommandMatch.Empty;
            }
        }
    }

    private class CommandMatch {
        public string Command { get; }
        public string Params { get; }
        public bool Success { get; }

        public IEnumerable<string> Args => Params.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        public CommandMatch(string command, string @params, bool success) {
            Command = command;
            Params = @params;
            Success = success;
        }

        public static CommandMatch Empty => new CommandMatch(string.Empty, string.Empty, false);
    }
}