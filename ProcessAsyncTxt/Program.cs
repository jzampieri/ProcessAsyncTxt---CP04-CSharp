using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace ProcessAsyncTxtApp;

record FileMetrics(string FileName, int Lines, int Words);

class Program
{
    static async Task Main()
    {
        Console.WriteLine("=== Processador Assíncrono de .TXT (.NET 8) ===");
        Console.Write("Informe o caminho do diretório com .txt: ");
        var dir = Console.ReadLine()?.Trim('"', ' ') ?? string.Empty;

        if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
        {
            Console.WriteLine("Diretório inválido ou inexistente. Encerrando.");
            return;
        }

        var txtFiles = Directory.EnumerateFiles(dir, "*.txt", SearchOption.TopDirectoryOnly).ToList();
        if (txtFiles.Count == 0)
        {
            Console.WriteLine("Nenhum .txt encontrado nesse diretório.");
            return;
        }

        Console.WriteLine($"\nArquivos encontrados ({txtFiles.Count}):");
        foreach (var f in txtFiles) Console.WriteLine(" - " + Path.GetFileName(f));

        Console.WriteLine("\nIniciando processamento assíncrono...\n");

        var cts = new CancellationTokenSource();
        var results = new ConcurrentBag<FileMetrics>();

        var degreeOfParallelism = Math.Min(Environment.ProcessorCount * 2, 16);
        using var semaphore = new SemaphoreSlim(degreeOfParallelism);

        var tasks = txtFiles.Select(async filePath =>
        {
            await semaphore.WaitAsync(cts.Token);
            try
            {
                var fileName = Path.GetFileName(filePath);
                Console.WriteLine($"Processando arquivo {fileName}...");
                var metrics = await ProcessFileAsync(filePath, cts.Token);
                results.Add(metrics);
                Console.WriteLine($"Concluído {fileName}: {metrics.Lines} linhas, {metrics.Words} palavras.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO] {filePath}: {ex.Message}");
            }
            finally
            {
                semaphore.Release();
            }
        }).ToList();

        await Task.WhenAll(tasks);

        var exportDir = Path.Combine(AppContext.BaseDirectory, "export");
        Directory.CreateDirectory(exportDir);
        var reportPath = Path.Combine(exportDir, "relatorio.txt");

        var reportLines = results
            .OrderBy(r => r.FileName, StringComparer.OrdinalIgnoreCase)
            .Select(r => $"{r.FileName} - {r.Lines} linhas - {r.Words} palavras");

        await File.WriteAllLinesAsync(reportPath, reportLines);

        Console.WriteLine($"\nRelatório gerado em: {reportPath}");
        Console.WriteLine("Finalizado.");
    }

    static async Task<FileMetrics> ProcessFileAsync(string path, CancellationToken ct)
    {
        int lines = 0;
        int words = 0;

        using var stream = File.OpenRead(path);
        using var reader = new StreamReader(stream);

        string? line;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            ct.ThrowIfCancellationRequested();
            lines++;

            if (!string.IsNullOrWhiteSpace(line))
            {
                words += Regex.Matches(line, @"\p{L}[\p{L}\p{N}_'-]*").Count;
            }
        }

        return new FileMetrics(Path.GetFileName(path), lines, words);
    }
}
