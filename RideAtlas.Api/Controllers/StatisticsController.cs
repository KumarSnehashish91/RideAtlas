using MediatR;
using Microsoft.AspNetCore.Mvc;
using RideAtlas.Api.Common;
using RideAtlas.Application.Statistics.Queries;

namespace RideAtlas.Api.Controllers;

public class StatisticsController(ISender sender) : ApiControllerBase(sender)
{
    [HttpGet("summary")]
    public async Task<ActionResult<StatisticsSummaryDto>> Summary(CancellationToken ct)
    {
        var summary = await Sender.Send(new StatisticsSummaryQuery(RiderAccountId), ct);
        return Ok(summary);
    }

    [HttpGet("trends")]
    public async Task<ActionResult<IReadOnlyList<MonthlyTrendPointDto>>> Trends([FromQuery] int months = 6, CancellationToken ct = default)
    {
        var trends = await Sender.Send(new StatisticsTrendsQuery(RiderAccountId, months), ct);
        return Ok(trends);
    }

    [HttpGet("bike-comparison")]
    public async Task<ActionResult<IReadOnlyList<BikeComparisonDto>>> BikeComparison(CancellationToken ct)
    {
        var comparison = await Sender.Send(new BikeComparisonQuery(RiderAccountId), ct);
        return Ok(comparison);
    }

    [HttpGet("trends/export")]
    public async Task<IActionResult> ExportTrends([FromQuery] int months = 6, CancellationToken ct = default)
    {
        var trends = await Sender.Send(new StatisticsTrendsQuery(RiderAccountId, months), ct);

        var headers = new[] { "Month", "Distance (km)", "Spend" };
        var rows = trends.Select(t => (IReadOnlyList<string?>)
            [t.Month, t.DistanceKm.ToString("0.#"), t.Spend.ToString("0.##")]);

        var csv = CsvWriter.Write(headers, rows);
        return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", "statistics-trends.csv");
    }
}
