using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NOF.Contract;
using Quantum.OfficialPlugins.Ledger.Rpc;
using System.Reflection;
using Xunit;
using LedgerPage = Quantum.OfficialPlugins.Ledger.UI.Pages.Ledger;

namespace Quantum.LedgerPlugin.Tests.UI.Pages;

public sealed class UIFailedResultTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task FailedLedgerSavePreservesEditorAndDoesNotReportSuccess(bool update)
    {
        var client = DispatchProxy.Create<ILedgerRpcServiceClient, FailedResultClient>();
        var page = new LedgerPage(client);
        Localize<OfficialPlugins.Ledger.UI.Localization.PluginStrings>(page);
        Set(page, "_editing", true);
        Set(page, "_note", "Keep this note");
        if (update)
        {
            Set(page, "_editId", Guid.NewGuid());
        }

        await Invoke(page, "Save");
        Assert.True((bool)Get(page, "_editing")!);
        Assert.Equal("Keep this note", Get(page, "_note"));
        Assert.Equal("Request rejected.", Get(page, "_error"));
        Assert.Null(Get(page, "_message"));
        Assert.Equal(new[] { update ? "UpdateEntryAsync" : "CreateEntryAsync" }, ((FailedResultClient)(object)client).Calls);
    }

    private static object? Get(object page, string name) => page.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(page);
    private static void Set(object page, string name, object value) => page.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(page, value);
    private static Task Invoke(object page, string name) => (Task)page.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(page, null)!;
    private static void Localize<T>(object page)
    {
        var factory = new ResourceManagerStringLocalizerFactory(Options.Create(new LocalizationOptions()), NullLoggerFactory.Instance);
        page.GetType().GetProperty("L", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(page, new StringLocalizer<T>(factory));
    }

    public class FailedResultClient : DispatchProxy
    {
        public List<string> Calls { get; } = [];

        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            Calls.Add(targetMethod!.Name);
            var resultType = targetMethod.ReturnType.GetGenericArguments()[0];
            return typeof(FailedResultClient).GetMethod(nameof(Fail), BindingFlags.Static | BindingFlags.NonPublic)!.MakeGenericMethod(resultType.GetGenericArguments()[0]).Invoke(null, null);
        }

        private static Task<Result<T>> Fail<T>() => Task.FromResult<Result<T>>(Result.Fail("rejected", "Request rejected."));
    }
}
