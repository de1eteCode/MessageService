using MessageService.Services.HandlerServices.TelegramService;
using Microsoft.AspNetCore.Components;

namespace MessageService.Pages;

public class TelegramControlPanelBase : ComponentBase {

    [Inject]
    public ITelegramHandlerService TelegramHandlerService { get; set; } = default!;
}