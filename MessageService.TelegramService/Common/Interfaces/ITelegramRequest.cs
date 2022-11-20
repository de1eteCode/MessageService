﻿using MediatR;
using Telegram.BotAPI.AvailableTypes;

namespace MessageService.TelegramService.Common.Interfaces;

internal interface ITelegramRequest<out TResponce> : IRequest<TResponce> {
    public abstract BotCommand BotCommand { get; }
}

internal interface ITelegramRequest : ITelegramRequest<Unit> {
}