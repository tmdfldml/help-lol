using System.Diagnostics;
using System.Text;

namespace LoLMatchup.Services.Logging;

public static class SafeExceptionFormatter
{
    /// <summary>Never reads Message, Data, ToString or source-file paths: those may contain identity/secrets.</summary>
    public static string Format(Exception exception)
    {
        var output = new StringBuilder();
        var remaining = 12;
        Append(exception, 0);
        return output.ToString();

        void Append(Exception current, int depth)
        {
            if (remaining-- <= 0) return;
            output.AppendLine($"Exception[{depth}]: {current.GetType().FullName}");
            output.AppendLine("Message (privacy-safe): " + (current switch
            {
                System.IO.IOException => "File operation failed; original details omitted.",
                System.Net.Http.HttpRequestException => "Network operation failed; original details omitted.",
                OperationCanceledException => "Operation cancelled; original details omitted.",
                System.Text.Json.JsonException => "Data could not be read; original details omitted.",
                _ => "Unexpected application error; original details omitted."
            }));
            output.AppendLine($"HResult: 0x{current.HResult:X8}");
            var frames = new StackTrace(current, false).GetFrames();
            output.AppendLine("Stack trace (method names only):");
            if (frames is null || frames.Length == 0) output.AppendLine("  unavailable");
            foreach (var frame in (frames ?? []).Take(48))
            {
                var method = frame.GetMethod();
                output.AppendLine($"  at {method?.DeclaringType?.FullName}.{method?.Name}");
            }
            if (current is AggregateException aggregate)
            {
                foreach (var inner in aggregate.InnerExceptions.Take(12)) Append(inner, depth + 1);
            }
            else if (current.InnerException is { } inner) Append(inner, depth + 1);
        }
    }
}
