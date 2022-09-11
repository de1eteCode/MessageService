using MessageService.Services.HandlerServices.Telegram;
using Microsoft.AspNetCore.Components;

namespace MessageService.Pages;

public class TelegramControlPanelBase : ComponentBase {

    [Inject]
    public ITelegramService TelegramHandlerService { get; set; } = default!;
}