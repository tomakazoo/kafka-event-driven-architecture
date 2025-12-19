using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Spectre.Console;

var rootCommand = new RootCommand("Event-Driven NAV Calculator Load Tester");

var fundCountOption = new Option<int>(
    name: "--funds",
    description: "Number of funds to process",
    getDefaultValue: () => 28);

var concurrentOption = new Option<bool>(
    name: "--concurrent",
    description: "Process funds concurrently (simulates month-end spike)",
    getDefaultValue: () => true);

var serviceUrlOption = new Option<string>(
    name: "--service-url",
    description: "Pricing service URL",
    getDefaultValue: () => "http://localhost:5001");

var iterationsOption = new Option<int>(
    name: "--iterations",
    description: "Number of test iterations to run",
    getDefaultValue: () => 1);

rootCommand.AddOption(fundCountOption);
rootCommand.AddOption(concurrentOption);
rootCommand.AddOption(serviceUrlOption);
rootCommand.AddOption(iterationsOption);

rootCommand.SetHandler(async (fundCount, concurrent, serviceUrl, iterations) =>
{
    await RunLoadTest(fundCount, concurrent, serviceUrl, iterations);
}, fundCountOption, concurrentOption, serviceUrlOption, iterationsOption);

return await rootCommand.InvokeAsync(args);

async Task RunLoadTest(int fundCount, bool concurrent, string serviceUrl, int iterations)
{
    AnsiConsole.Write(
        new FigletText("Load Tester")
            .LeftJustified()
            .Color(Color.Blue));

    AnsiConsole.MarkupLine($"[blue]Configuration:[/]");
    AnsiConsole.MarkupLine($"  Funds: [yellow]{fundCount}[/]");
    AnsiConsole.MarkupLine($"  Mode: [yellow]{(concurrent ? "Concurrent (Month-End)" : "Sequential")}[/]");
    AnsiConsole.MarkupLine($"  Service: [yellow]{serviceUrl}[/]");
    AnsiConsole.MarkupLine($"  Iterations: [yellow]{iterations}[/]");
    AnsiConsole.WriteLine();

    var allResults = new List<LoadTestResult>();

    for (int iteration = 1; iteration <= iterations; iteration++)
    {
        if (iterations > 1)
        {
            AnsiConsole.MarkupLine($"[cyan]Iteration {iteration}/{iterations}[/]");
        }

        var result = await AnsiConsole.Progress()
            .Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn(),
            })
            .StartAsync(async ctx =>
            {
                var task = ctx.AddTask($"[green]Processing {fundCount} funds[/]", maxValue: fundCount);

                var sw = Stopwatch.StartNew();
                var results = new List<FundResult>();
                var httpClient = new HttpClient { BaseAddress = new Uri(serviceUrl) };

                if (concurrent)
                {
                    var tasks = Enumerable.Range(1, fundCount).Select(async i =>
                    {
                        var fundId = $"LUX-{i:D3}";
                        var fundResult = await TriggerPricing(httpClient, fundId, $"Test Fund {i}");
                        task.Increment(1);
                        return fundResult;
                    });

                    results = (await Task.WhenAll(tasks)).ToList();
                }
                else
                {
                    for (int i = 1; i <= fundCount; i++)
                    {
                        var fundId = $"LUX-{i:D3}";
                        var fundResult = await TriggerPricing(httpClient, fundId, $"Test Fund {i}");
                        results.Add(fundResult);
                        task.Increment(1);
                    }
                }

                sw.Stop();

                return new LoadTestResult
                {
                    TotalDuration = sw.Elapsed,
                    FundResults = results,
                    Concurrent = concurrent
                };
            });

        allResults.Add(result);
        DisplayResults(result, iteration);

        if (iteration < iterations)
        {
            AnsiConsole.MarkupLine("[dim]Waiting 5 seconds before next iteration...[/]");
            await Task.Delay(5000);
            AnsiConsole.WriteLine();
        }
    }

    if (iterations > 1)
    {
        AnsiConsole.WriteLine();
        DisplayAggregateResults(allResults);
    }
}

async Task<FundResult> TriggerPricing(HttpClient httpClient, string fundId, string fundName)
{
    var sw = Stopwatch.StartNew();
    try
    {
        var response = await httpClient.PostAsJsonAsync("/api/pricing/trigger", new
        {
            fundId,
            fundName
        });

        sw.Stop();

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadFromJsonAsync<TriggerResponse>();
            return new FundResult
            {
                FundId = fundId,
                Success = true,
                Duration = sw.Elapsed,
                CorrelationId = content?.CorrelationId ?? "unknown"
            };
        }
        else
        {
            return new FundResult
            {
                FundId = fundId,
                Success = false,
                Duration = sw.Elapsed,
                Error = $"HTTP {response.StatusCode}"
            };
        }
    }
    catch (Exception ex)
    {
        sw.Stop();
        return new FundResult
        {
            FundId = fundId,
            Success = false,
            Duration = sw.Elapsed,
            Error = ex.Message
        };
    }
}

void DisplayResults(LoadTestResult result, int iteration)
{
    var successful = result.FundResults.Count(f => f.Success);
    var failed = result.FundResults.Count(f => !f.Success);
    var durations = result.FundResults.Select(f => f.Duration.TotalMilliseconds).ToList();

    var table = new Table();
    table.AddColumn("[bold]Metric[/]");
    table.AddColumn("[bold]Value[/]");

    table.AddRow("Mode", result.Concurrent ? "[yellow]Concurrent (Month-End)[/]" : "[cyan]Sequential[/]");
    table.AddRow("Total Duration", $"[green]{result.TotalDuration.TotalSeconds:F2}s[/]");
    table.AddRow("Total Funds", result.FundResults.Count.ToString());
    table.AddRow("Successful", $"[green]{successful}[/]");
    table.AddRow("Failed", failed > 0 ? $"[red]{failed}[/]" : "0");
    table.AddRow("Success Rate", $"{(successful * 100.0 / result.FundResults.Count):F2}%");
    table.AddRow("Avg Duration", $"{durations.Average():F0}ms");
    table.AddRow("Min Duration", $"{durations.Min():F0}ms");
    table.AddRow("Max Duration", $"{durations.Max():F0}ms");
    table.AddRow("P95 Duration", $"{Percentile(durations, 95):F0}ms");
    table.AddRow("Throughput", $"{result.FundResults.Count / result.TotalDuration.TotalSeconds:F2} funds/sec");

    AnsiConsole.Write(table);
    AnsiConsole.WriteLine();
}

void DisplayAggregateResults(List<LoadTestResult> results)
{
    AnsiConsole.Write(new Rule("[bold blue]Aggregate Results[/]"));
    
    var allDurations = results.SelectMany(r => r.FundResults.Select(f => f.Duration.TotalMilliseconds)).ToList();
    var totalSuccessful = results.Sum(r => r.FundResults.Count(f => f.Success));
    var totalFailed = results.Sum(r => r.FundResults.Count(f => !f.Success));

    var table = new Table();
    table.AddColumn("[bold]Metric[/]");
    table.AddColumn("[bold]Value[/]");

    table.AddRow("Total Iterations", results.Count.ToString());
    table.AddRow("Total Funds", (totalSuccessful + totalFailed).ToString());
    table.AddRow("Successful", $"[green]{totalSuccessful}[/]");
    table.AddRow("Failed", totalFailed > 0 ? $"[red]{totalFailed}[/]" : "0");
    table.AddRow("Avg Duration", $"{allDurations.Average():F0}ms");
    table.AddRow("P95 Duration", $"{Percentile(allDurations, 95):F0}ms");

    AnsiConsole.Write(table);
}

double Percentile(List<double> sequence, double percentile)
{
    var sorted = sequence.OrderBy(x => x).ToList();
    int n = sorted.Count;
    double index = (percentile / 100.0) * (n - 1);
    int lower = (int)Math.Floor(index);
    int upper = (int)Math.Ceiling(index);
    
    if (lower == upper)
        return sorted[lower];
    
    return sorted[lower] + (sorted[upper] - sorted[lower]) * (index - lower);
}

record LoadTestResult
{
    public TimeSpan TotalDuration { get; init; }
    public List<FundResult> FundResults { get; init; } = new();
    public bool Concurrent { get; init; }
}

record FundResult
{
    public string FundId { get; init; } = "";
    public bool Success { get; init; }
    public TimeSpan Duration { get; init; }
    public string? CorrelationId { get; init; }
    public string? Error { get; init; }
}

record TriggerResponse
{
    public string CorrelationId { get; init; } = "";
    public string FundId { get; init; } = "";
}
