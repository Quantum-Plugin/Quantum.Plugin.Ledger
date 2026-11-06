using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NOF.Contract;
using Quantum.OfficialPlugins.Ledger.UI.Localization;
using System.Reflection;
using System.Text.Json;
using LedgerPage = Quantum.OfficialPlugins.Ledger.UI.Pages.Ledger;

namespace Quantum.LedgerPlugin.Tests.UI.Pages;

public sealed class CalendarIntegrationTests
{
    [Theory]
    [InlineData("calendar.entry.list", true, false)]
    [InlineData("calendar.entry.get", false, true)]
    [InlineData("calendar.list", false, false)]
    public async Task ResourceReadCapabilitiesAreDiscoveredIndependently(string capability, bool canList, bool canGet)
    {
        var requester = new CalendarRequester { Capabilities = [capability] };
        var page = Page(requester);
        await Invoke(page, "RefreshCalendarCapabilitiesAsync");
        Assert.Equal(canList, Field(page, "_canListCalendar"));
        Assert.Equal(canGet, Field(page, "_canGetCalendar"));
    }

    [Theory]
    [InlineData(false, 1)]
    [InlineData(true, 0)]
    public async Task RepeatedOccurrencesGroupWithinProvidersAndConflictingProvidersRemainAmbiguous(bool conflict, int expected)
    {
        var requester = new CalendarRequester { Conflict = conflict };
        var page = Page(requester);
        Set(page, "_candidateMonth", new DateOnly(2026, 12, 1));
        await Invoke(page, "LoadCalendarCandidates");
        Assert.Equal("calendar.entry.list", requester.LastCapability);
        Assert.Equal("2026-12-01", requester.LastPayload.GetProperty("startDate").GetString());
        Assert.Equal("2026-12-31", requester.LastPayload.GetProperty("endDate").GetString());
        Assert.Equal(expected, ((Array)Field(page, "_calendarCandidates")!).Length);
    }

    private static LedgerPage Page(CalendarRequester requester)
    {
        var page = new LedgerPage(null!, requester);
        var factory = new ResourceManagerStringLocalizerFactory(Options.Create(new LocalizationOptions()), NullLoggerFactory.Instance);
        typeof(LedgerPage).GetProperty("L", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(page, new StringLocalizer<PluginStrings>(factory));
        return page;
    }

    private static object? Field(LedgerPage page, string name) => typeof(LedgerPage).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(page);
    private static void Set(LedgerPage page, string name, object value) => typeof(LedgerPage).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(page, value);
    private static Task Invoke(LedgerPage page, string method) => (Task)typeof(LedgerPage).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(page, null)!;

    private sealed class CalendarRequester : IQuantumRequester
    {
        public string[] Capabilities { get; init; } = ["calendar.entry.list", "calendar.entry.get"];
        public bool Conflict { get; init; }
        public string? LastCapability { get; private set; }
        public JsonElement LastPayload { get; private set; }
        public Task<Result<QuantumRequestReport<T>>> RequestAsync<T>(string capabilityName, object payload, Context context, CancellationToken cancellationToken = default)
        {
            object value;
            if (capabilityName == QuantumCapabilityCatalog.CapabilityName)
            {
                value = new QuantumCapabilityCatalog(2, Capabilities.Select(name => new QuantumCapabilityDescription(name, QuantumCapabilityKind.Request, 1, null, ["calendar.provider"], null, null, null)).ToArray());
            }
            else
            {
                LastCapability = capabilityName;
                LastPayload = JsonSerializer.SerializeToElement(payload, JsonSerializerOptions.NOF);
                var entry = new { id = Guid.NewGuid(), title = "Series", date = "2026-12-03", kind = "Event", notes = "", startTime = "09:00:00", endTime = (string?)null, isCompleted = false };
                value = new[] { entry, entry with { date = "2026-12-10" } };
            }

            var result = JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value, JsonSerializerOptions.NOF), JsonSerializerOptions.NOF);
            var outcomes = new List<QuantumRequestOutcome<T>> { new("calendar.provider", QuantumRequestStatus.Handled, result, null, null) { HasValue = true } };
            if (Conflict && capabilityName != QuantumCapabilityCatalog.CapabilityName)
            {
                outcomes.Add(new("conflicting.provider", QuantumRequestStatus.Handled, result, null, null) { HasValue = true });
            }

            return Task.FromResult(Result.Success(new QuantumRequestReport<T>(outcomes, false)));
        }
    }
}
