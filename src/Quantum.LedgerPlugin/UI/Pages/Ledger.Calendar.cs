using Microsoft.AspNetCore.Components;
using NOF.Contract;
using Quantum.OfficialPlugins.Ledger.Rpc;
using Quantum.Plugin.Abstraction;
using System.Text.Json;

namespace Quantum.OfficialPlugins.Ledger.UI.Pages;

public partial class Ledger
{
    private readonly IQuantumRequester? _invoker;
    private bool _canListCalendar;
    private bool _canGetCalendar;
    private List<Guid> _selectedCalendarIds = [];
    private CalendarLinkDetails[] _calendarCandidates = [];
    private List<(Guid Id, CalendarLinkDetails? Entry)> _calendarLinks = [];
    private LedgerEntryDetails? _calendarViewing;
    private DateOnly _calendarMonth;
    private async Task RefreshCalendarCapabilitiesAsync()
    {
        _canListCalendar = false;
        _canGetCalendar = false;
        if (_invoker is null)
        {
            return;
        }

        try
        {
            var capabilities = (await _invoker.DiscoverCapabilitiesAsync()).Value?.Capabilities.Select(method => method.Name);
            if (capabilities is null)
            {
                return;
            }

            _canListCalendar = capabilities.Contains("calendar.list");
            _canGetCalendar = capabilities.Contains("calendar.get");
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            // Optional UI actions remain hidden while the catalog is unavailable.
        }
    }

    private async Task LoadCalendarCandidates()
    {
        await RefreshCalendarCapabilitiesAsync();
        _calendarMonth = _candidateMonth;
        _calendarCandidates = [];
        if (!_canListCalendar)
        {
            return;
        }

        try
        {
            await QueryCalendarCandidates();
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            _error = L["Calendar unavailable. Saved links are preserved."];
        }
    }

    private async Task QueryCalendarCandidates()
    {
        var start = new DateOnly(_calendarMonth.Year, _calendarMonth.Month, 1);
        var request = new
        {
            startDate = start,
            endDate = start.AddMonths(1).AddDays(-1)
        };
        var responses = await _invoker!.RequestAsync<CalendarLinkDetails[]>("calendar.list", request, Context.Empty);
        if (!IsSuccessful(responses))
        {
            return;
        }

        var outcomes = responses.Value!.Outcomes;
        _calendarCandidates = outcomes.Where(outcome => outcome.Status == QuantumRequestStatus.Handled).SelectMany(outcome => outcome.Value ?? []).GroupBy(entry => entry.Id).Where(group => group.Count() == 1).Select(group => group.Single()).ToArray();
        if (outcomes.Count == 0 || outcomes.Any(outcome => outcome.Status is QuantumRequestStatus.Failed or QuantumRequestStatus.TimedOut))
        {
            _error = L["Calendar unavailable. Saved links are preserved."];
        }
    }

    private Task SelectCalendarMonth(ChangeEventArgs args) => Run(async () =>
    {
        if (DateOnly.TryParse(args.Value + "-01", out var month) && month.Year is >= 1900 and <= 9998)
        {
            _calendarMonth = month;
            await QueryCalendarCandidates();
        }
    });
    private void AddCalendarLink(ChangeEventArgs args)
    {
        if (Guid.TryParse(args.Value?.ToString(), out var id) && !_selectedCalendarIds.Contains(id))
        {
            _selectedCalendarIds.Add(id);
        }
    }

    private Task ViewCalendarLinks(LedgerEntryDetails item) => Run(async () =>
    {
        await RefreshCalendarCapabilitiesAsync();
        if (!_canGetCalendar)
        {
            _error = L["Calendar unavailable. Saved links are preserved."];
            return;
        }

        List<(Guid Id, CalendarLinkDetails? Entry)> links = [];
        foreach (var id in item.CalendarEntryIds ?? [])
        {
            var responses = await _invoker!.RequestAsync<CalendarLinkLookup>("calendar.get", new { id }, Context.Empty);
            if (!IsSuccessful(responses))
            {
                return;
            }

            var outcomes = responses.Value!.Outcomes;
            var matches = outcomes.Where(outcome => outcome.Status == QuantumRequestStatus.Handled && outcome.Value?.Entry is not null).Select(outcome => outcome.Value!.Entry).ToArray();
            if (matches.Length > 1 || outcomes.Count == 0 || outcomes.Any(outcome => outcome.Status is QuantumRequestStatus.Failed or QuantumRequestStatus.TimedOut))
            {
                _error = L["Calendar unavailable. Saved links are preserved."];
                return;
            }

            links.Add((id, matches.SingleOrDefault()));
        }

        _calendarLinks = links;
        _calendarViewing = item;
    });
}
