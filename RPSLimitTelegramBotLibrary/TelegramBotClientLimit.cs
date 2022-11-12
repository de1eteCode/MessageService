using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using RPSLimitTelegramBotLibrary.Extensions;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace RPSLimitTelegramBotLibrary.Telegram;

public class TelegramBotClientLimit : TelegramBotClient {
    private readonly SemaphoreSlim _semaphore;

    /// <summary>
    /// Время выполнения <see cref="Rate"/> операций
    /// </summary>
    public TimeSpan TimeForOperation { get; init; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Количество операций, выполняемых за <see cref="TimeForOperation"/> времени
    /// </summary>
    public int Rate { get; init; } = 1;

    /// <summary>
    /// Время ожидания после выполнения 1 операции
    /// </summary>
    public TimeSpan RPS => TimeSpan.FromTicks(TimeForOperation.Ticks / Rate);

    private readonly TelegramBotClientOptions _options;
    private readonly HttpClient _httpClient;

    public new event AsyncEventHandler<ApiRequestEventArgs>? OnMakingApiRequest;

    public new event AsyncEventHandler<ApiResponseEventArgs>? OnApiResponseReceived;

    public TelegramBotClientLimit(TelegramBotClientOptions options, HttpClient? httpClient = null) : base(options, httpClient) {
        _semaphore = new SemaphoreSlim(1, Rate);
        _httpClient = GetFieldObj<HttpClient>(nameof(_httpClient));
        _options = GetFieldObj<TelegramBotClientOptions>(nameof(_options));
    }

    public TelegramBotClientLimit(string token, HttpClient? httpClient = null) : base(token, httpClient) {
        _semaphore = new SemaphoreSlim(1, Rate);
        _httpClient = GetFieldObj<HttpClient>(nameof(_httpClient));
        _options = GetFieldObj<TelegramBotClientOptions>(nameof(_options));
    }

    private T GetFieldObj<T>(string fieldName) {
        var type = typeof(TelegramBotClient);
        var filed = type.GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        return (T)filed!.GetValue(this)!;
    }

    public override async Task<TResponse> MakeRequestAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        var url = $"{_options.BaseRequestUrl}/{request.MethodName}";

        var httpRequest = new HttpRequestMessage(method: request.Method, requestUri: url) {
            Content = request.ToHttpContent()
        };

        if (OnMakingApiRequest is not null) {
            var requestEventArgs = new ApiRequestEventArgs(
                request: request,
                httpRequestMessage: httpRequest
            );
            await OnMakingApiRequest.Invoke(
                botClient: this,
                args: requestEventArgs,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);
        }

        // Ожидание свободного слота
        await _semaphore.WaitAsync(cancellationToken);

        using var httpResponse = await SendRequestAsync(
            httpClient: _httpClient,
            httpRequest: httpRequest,
            cancellationToken: cancellationToken
        ).ConfigureAwait(false);

        await Task.Delay(RPS, cancellationToken);

        // Освобождение свободного слота
        _semaphore.Release();

        if (OnApiResponseReceived is not null) {
            var requestEventArgs = new ApiRequestEventArgs(
                request: request,
                httpRequestMessage: httpRequest
            );
            var responseEventArgs = new ApiResponseEventArgs(
                responseMessage: httpResponse,
                apiRequestEventArgs: requestEventArgs
            );
            await OnApiResponseReceived.Invoke(
                botClient: this,
                args: responseEventArgs,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);
        }

        if (httpResponse.StatusCode != HttpStatusCode.OK) {
            var failedApiResponse = await httpResponse
                .DeserializeContentAsync<ApiResponse>(
                    guard: response =>
                        response.ErrorCode == default ||
                        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                        response.Description is null
                )
                .ConfigureAwait(false);

            throw ExceptionsParser.Parse(failedApiResponse);
        }

        var apiResponse = await httpResponse
            .DeserializeContentAsync<ApiResponse<TResponse>>(
                guard: response => response.Ok == false ||
                                   response.Result is null
            )
            .ConfigureAwait(false);

        return apiResponse.Result!;

        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        static async Task<HttpResponseMessage> SendRequestAsync(
            HttpClient httpClient,
            HttpRequestMessage httpRequest,
            CancellationToken cancellationToken) {
            HttpResponseMessage? httpResponse;
            try {
                httpResponse = await httpClient
                    .SendAsync(request: httpRequest, cancellationToken: cancellationToken)
                    .ConfigureAwait(continueOnCapturedContext: false);
            }
            catch (TaskCanceledException exception) {
                if (cancellationToken.IsCancellationRequested) {
                    throw;
                }

                throw new RequestException(message: "Request timed out", innerException: exception);
            }
            catch (Exception exception) {
                throw new RequestException(
                    message: "Exception during making request",
                    innerException: exception
                );
            }

            return httpResponse;
        }
    }
}