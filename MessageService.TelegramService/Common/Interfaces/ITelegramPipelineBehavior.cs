using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageService.TelegramService.Common.Interfaces;

internal interface ITelegramPipelineBehavior<TRequest, TResponce> : IPipelineBehavior<TRequest, TResponce>
    where TRequest : ITelegramRequest<TResponce> {
}