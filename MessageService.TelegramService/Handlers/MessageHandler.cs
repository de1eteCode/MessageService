﻿using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using MediatR;
using MessageService.TelegramService.Common.Enums;
using MessageService.TelegramService.Common.Extends;
using MessageService.TelegramService.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;

namespace MessageService.TelegramService.Handlers;

internal class MessageHandler : ITelegramUpdateHandler<Message> {
    private static CommandHelper _commandHelper = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly IMediator _mediator;
    private readonly BotClient _botClient;
    private readonly ILogger<MessageHandler> _logger;

    public MessageHandler(IServiceProvider serviceProvider, IMediator mediator, BotClient botClient, ILogger<MessageHandler> logger) {
        _serviceProvider = serviceProvider;
        _mediator = mediator;
        _botClient = botClient;
        _logger = logger;
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

        var cmd = allowCommands.FirstOrDefault(e => e.BotCommand.Command.Equals(command.Command));

        if (cmd == null) {
            return Task.CompletedTask;
        }

        _logger.LogWarning($"Todo: call command \"{command.Command}\"");

        return Task.CompletedTask;
    }

    private Task HandleGroupChat(Message handleUpdate, CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    private Task HandleSuperGroupChat(Message handleUpdate, CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

    private Task HandleChanelChat(Message handleUpdate, CancellationToken cancellationToken) {
        return Task.CompletedTask;
    }

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

        public CommandMatch(string command, string @params, bool success) {
            Command = command;
            Params = @params;
            Success = success;
        }

        public static CommandMatch Empty => new CommandMatch(string.Empty, string.Empty, false);
    }
}