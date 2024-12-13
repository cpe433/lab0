using System.Text.RegularExpressions;

namespace SimpleCrawler;

/// <summary>
/// Class <c>Crawler</c> accesses a webpage based on the given URL, retrieves the content,
/// and recursively accesses linked pages.
/// </summary>
public partial class Crawler
{
    private string? _baseFolder;
    private int _maxLinksPerPage = 3;

    /// <summary>
    /// Sets the base folder to store retrieved content.
    /// </summary>
    /// <param name="folder">The name of the base folder</param>
    public void SetBaseFolder(string folder)
    {
        if (string.IsNullOrEmpty(folder))
        {
            throw new ArgumentNullException(nameof(folder));
        }
        _baseFolder = folder;
    }

    /// <summary>
    /// Sets the maximum number of links that will be recursively accessed from a page.
    /// </summary>
    /// <param name="max">The maximum number of links</param>
    public void SetMaxLinksPerPage(int max)
    {
        _maxLinksPerPage = max;
    }

    /// <summary>
    /// Recursively gets a web page and its linked pages up to the specified level.
    /// </summary>
    /// <param name="url">The URL of the webpage to retrieve</param>
    /// <param name="level">The remaining recursion depth</param>
    public async Task GetPage(string url, int level)
    {
        if (_baseFolder == null)
        {
            throw new Exception("Please set the base folder using the SetBaseFolder method first.");
        }
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentNullException(nameof(url));
        }

        if (level <= 0)
        {
            return; // Stop recursion when levels are exhausted
        }

        HttpClient client = new();

        try
        {
            Console.WriteLine($"Fetching: {url}");

            // Get content from URL
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();

                // Save the webpage to a file
                string fileName = MakeValidFileName(url) + ".html";
                string filePath = Path.Combine(_baseFolder, fileName);
                Directory.CreateDirectory(_baseFolder); // Ensure the folder exists
                await File.WriteAllTextAsync(filePath, responseBody);
                Console.WriteLine($"Saved: {filePath}");

                // Extract links from the page
                ISet<string> links = GetLinksFromPage(responseBody);

                int count = 0;
                foreach (string link in links)
                {
                    if (link.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // Only process up to _maxLinksPerPage links
                        if (++count > _maxLinksPerPage) break;

                        // Recursively fetch the linked page
                        await GetPage(link, level - 1);
                    }
                }
            }
            else
            {
                Console.WriteLine($"Failed to fetch {url}: {response.StatusCode}");
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request exception for {url}: {ex.Message}");
        }
        finally
        {
            client.Dispose();
        }
    }

    /// <summary>
    /// Extracts links (i.e., <a href="...">) from the HTML content.
    /// </summary>
    /// <param name="content">HTML page content</param>
    /// <returns>A set of extracted links</returns>
    public static ISet<string> GetLinksFromPage(string content)
    {
        Regex regexLink = MyRegex();
        HashSet<string> links = new();

        foreach (Match match in regexLink.Matches(content))
        {
            string? link = match.Value;
            if (!string.IsNullOrEmpty(link))
            {
                links.Add(link);
            }
        }
        return links;
    }

    /// <summary>
    /// Converts a URL into a valid filename by replacing invalid characters.
    /// </summary>
    /// <param name="url">The URL to convert</param>
    /// <returns>A valid filename string</returns>
    private static string MakeValidFileName(string url)
    {
        return url.Replace(":", "_").Replace("/", "_").Replace(".", "_");
    }

    // Regex pattern to extract href links
    [GeneratedRegex("(?<=<a\\s*href=(?:'|\")).*?(?=(?:'|\"))", RegexOptions.IgnoreCase)]
    private static partial Regex MyRegex();
}

/// <summary>
/// Program entry point.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Crawler crawler = new();
        crawler.SetBaseFolder("./CrawledPages");
        crawler.SetMaxLinksPerPage(5);

        string startUrl = "https://example.com"; // Replace with your starting URL
        int recursionDepth = 2; // Set recursion level

        await crawler.GetPage(startUrl, recursionDepth);

        Console.WriteLine("Crawling completed.");
    }
}
