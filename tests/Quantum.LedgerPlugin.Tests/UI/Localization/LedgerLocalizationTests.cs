using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Quantum.OfficialPlugins.Ledger;
using Quantum.OfficialPlugins.Ledger.UI.Localization;
using System.Globalization;
using Xunit;

namespace Quantum.LedgerPlugin.Tests.UI.Localization;

public sealed class LedgerLocalizationTests
{
    [Fact]
    public void ChineseAndEnglishResourcesResolveFromTheCompiledAssembly()
    {
        var services = new ServiceCollection().AddLogging().AddLocalization().BuildServiceProvider();
        var localizer = services.GetRequiredService<IStringLocalizer<PluginStrings>>();
        var original = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("zh-CN");
            Assert.Equal("记账", localizer["Ledger"].Value);
            Assert.False(localizer["Save"].ResourceNotFound);
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            Assert.Equal("Ledger", localizer["Ledger"].Value);
        }
        finally
        {
            CultureInfo.CurrentUICulture = original;
            services.Dispose();
        }
    }
}
