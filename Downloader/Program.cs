using System.Web;
using HtmlAgilityPack;
using SevenZip;

try
{
    const string GoogleDriveUrl = "https://drive.google.com/uc?export=download&id=";

    var html = @"https://forum.gamer.com.tw/C.php?bsn=2954&snA=2719&tnum=44";
    var web = new HtmlWeb();
    var htmlDoc = web.Load(html);

    var node = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='cf10083']/div/div/font/a");
    if (node == null)
    {
        Console.WriteLine("Can not found!");
        return;
    }

    var name = node.InnerHtml;
    Console.WriteLine(name);

    var href = node.Attributes["href"].Value;
    Console.WriteLine("Url: " + href);

    var query = HttpUtility.ParseQueryString(new Uri(href).Query);
    var target = query["url"] = query["url"];
    Console.WriteLine("Target: " + target);
    var driveId = target?.Split('/');
    if (driveId == null || driveId.Length < 6)
    {
        Console.WriteLine("Can not found!");
        return;
    }

    var id = driveId[5];
    Console.WriteLine("Id: " + id);

    var url = GoogleDriveUrl + id;

    Console.WriteLine("Downloading...");
    var httpClient = new HttpClient();
    var response = await httpClient.GetAsync(url);

    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine("Can not found!");
        return;
    }

    Console.WriteLine("Downloaded!");

    await using (var fs = new FileStream($"./{name}.rar", FileMode.Create))
    {
        await response.Content.CopyToAsync(fs);
        fs.Flush();
        response.Dispose();
        await fs.DisposeAsync();
    }

    // Unrar rar file
    Console.WriteLine("Unzipping...");
    SevenZipBase.SetLibraryPath("./7z.dll");
    using (var extractor = new SevenZipExtractor($"./{name}.rar", InArchiveFormat.Rar))
    {
        await extractor.ExtractArchiveAsync("output");
    }

    Console.WriteLine("Unzipped!");
}
catch (Exception e)
{
    Console.WriteLine(e);
}