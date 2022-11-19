using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using TelegramService.Models;

namespace TelegramService.IntegrationTests;

[TestClass]
public class LimitTests {
    private TelegramSettings _settings = default!;

    [TestInitialize]
    public void Init() {
        var config = new ConfigurationBuilder()
            .AddUserSecrets(Assembly.GetExecutingAssembly())
            .Build();

        var configSectionTgSettings = config.GetSection("TelegramSettings");

        _settings = new TelegramSettings() {
            Token = configSectionTgSettings["Token"]!
        };
    }

    [TestMethod]
    public async Task Test_Max5Calls_1Call_1Sec() {
        var bot = BuildBot(5);
        var stopwatch = new Stopwatch();

        stopwatch.Start();

        var tgIdent = await bot.GetMeAsync();

        stopwatch.Stop();

        Assert.IsNotNull(tgIdent);
        Assert.IsTrue(1 >= stopwatch.Elapsed.Seconds);
    }

    [TestMethod]
    public async Task Test_Max5Calls_5Call_1Sec() {
        var bot = BuildBot(5);
        var stopwatch = new Stopwatch();
        var repeat = 5;
        var tasks = Enumerable.Range(0, repeat).Select(async e => await GetRandomChat_Exception(bot));

        stopwatch.Start();

        var res = await Task.WhenAll(tasks);

        stopwatch.Stop();

        Assert.IsFalse(res.Any(e => e == false));
        Assert.IsTrue(1 >= stopwatch.Elapsed.Seconds);
    }

    [TestMethod]
    public async Task Test_Max5Calls_15Call_Minimum3Sec() {
        var bot = BuildBot(5);
        var stopwatch = new Stopwatch();
        var repeat = 15;
        var tasks = Enumerable.Range(0, repeat).Select(async e => await GetRandomChat_Exception(bot));

        stopwatch.Start();

        var res = await Task.WhenAll(tasks);

        stopwatch.Stop();

        Assert.IsFalse(res.Any(e => e == false));
        Assert.IsTrue(3 <= stopwatch.Elapsed.Seconds);
    }

    [TestMethod]
    public async Task Test_Max15Calls_15Call_Minimum1Sec() {
        var bot = BuildBot(15);
        var stopwatch = new Stopwatch();
        var repeat = 15;
        var tasks = Enumerable.Range(0, repeat).Select(async e => await GetRandomChat_Exception(bot));

        stopwatch.Start();

        var res = await Task.WhenAll(tasks);

        stopwatch.Stop();

        Assert.IsFalse(res.Any(e => e == false));
        Assert.IsTrue(1 <= stopwatch.Elapsed.Seconds);
    }

    [TestMethod]
    public async Task Test_Max15Calls_60Call_Minimum4Sec() {
        var bot = BuildBot(15);
        var stopwatch = new Stopwatch();
        var repeat = 60;
        var tasks = Enumerable.Range(0, repeat).Select(async e => await GetRandomChat_Exception(bot));

        stopwatch.Start();

        var res = await Task.WhenAll(tasks);

        stopwatch.Stop();

        Assert.IsFalse(res.Any(e => e == false));
        Assert.IsTrue(4 <= stopwatch.Elapsed.Seconds);
    }

    private async Task<bool> GetRandomChat_Exception(TelegramBotClient bot) {
        long chatIdRnd = LongRandom();
        try {
            await bot.GetChatAsync(new ChatId(chatIdRnd));
            return true;
        }
        catch (ApiRequestException ex) when (ex.ErrorCode == 400) { // not found chat
            return true;
        }
        catch (Exception e) {
            return false;
        }
    }

    private long LongRandom(long min = long.MinValue, long max = long.MaxValue) {
        byte[] buf = new byte[8];
        Random.Shared.NextBytes(buf);
        long longRand = BitConverter.ToInt64(buf, 0);

        return (Math.Abs(longRand % (max - min)) + min);
    }

    private TelegramBotClientLimit BuildBot(int limit) {
        if (limit < 1) {
            throw new ArgumentException(nameof(limit));
        }

        return new TelegramBotClientLimit(_settings.Token) {
            Rate = limit
        };
    }
}