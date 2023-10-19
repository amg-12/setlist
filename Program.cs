using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;

using HttpClient client = new();
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(
    new MediaTypeWithQualityHeaderValue("application/json"));
client.DefaultRequestHeaders.Add("x-api-key", Secret.key);
const string setlistUrl = "https://api.setlist.fm/rest/1.0/user/";

var hi = await compareUsers(client, "Amitai", "harrymgaze");
hi.ForEach(x => Console.WriteLine(x));


static async Task<SetlistPage> getPage(HttpClient client, string user, int p)
{
    await using Stream stream =
    await client.GetStreamAsync(setlistUrl + user + "/attended?p=" + p.ToString());
    var page =
        await JsonSerializer.DeserializeAsync<SetlistPage>(stream);

    return page;
}

static async Task<List<Setlist>> getAllSetlists(HttpClient client, string user)
{
    List<Setlist> setlists = new List<Setlist>();
    int total = 1;

    for(int p=1; setlists.Count < total; p++)
    {
        SetlistPage page = await getPage(client, user, p);
        total = page.total;
        setlists.AddRange(page.setlist);
        Thread.Sleep(550);
    }

    return setlists;
}

static async Task<List<Setlist>> compareUsers(HttpClient client, string user1, string user2)
{
    List<Setlist> sets1 = await getAllSetlists(client, user1);
    List<Setlist> sets2 = await getAllSetlists(client, user2);

    return sets1.Intersect(sets2).ToList();
}