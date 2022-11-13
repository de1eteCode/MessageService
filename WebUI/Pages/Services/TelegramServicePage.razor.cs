using Infrastructure.Enums;
using Microsoft.AspNetCore.Components;

namespace WebUI.Pages.Services;

public class TelegramServicePageBase : ComponentBase {

    [Inject]
    public TelegramService.TelegramHostedService TelegramHostedService { get; set; } = default!;

    public ServiceState StateService => TelegramHostedService.State;
}