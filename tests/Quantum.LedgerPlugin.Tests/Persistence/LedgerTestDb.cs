using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NOF.Application;
using NOF.Hosting;
using NOF.Infrastructure;
using NOF.Infrastructure.EntityFrameworkCore;
using Quantum.OfficialPlugins.Ledger;
using Xunit;

namespace Quantum.LedgerPlugin.Tests.Persistence;

public sealed class LedgerTestDb : NOFDbContext
{
    public LedgerTestDb(DbContextOptions<LedgerTestDb> options) : base(options)
    {
    }
}
