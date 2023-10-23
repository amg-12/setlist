using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;

public class api
{
    static HttpClient client = initClient();
    const string setlistUrl = "https://api.setlist.fm/rest/1.0/user/";

    public static HttpClient initClient()
    {
        HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("x-api-key", Secret.key);

        return client;
    }

    static async Task<SetlistPage> getPage(string user, int p)
    {
        HttpResponseMessage? response = null;

        do
        {
            response = await client.GetAsync(setlistUrl + user + "/attended?p=" + p.ToString());
            await Console.Out.WriteLineAsync(response.StatusCode.ToString());

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new SetlistPage(0, new List<Setlist>());
            }

        } while (!response.IsSuccessStatusCode);

        Stream stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<SetlistPage>(stream);
    }

    public static async Task<List<Setlist>> getAllSetlists(string user)
    {
        List<Setlist> setlists = new List<Setlist>();
        int total = 1;

        for (int p = 1; setlists.Count < total; p++)
        {
            SetlistPage page = await getPage(user, p);
            total = page.total;
            setlists.AddRange(page.setlist);
        }
        await Console.Out.WriteLineAsync("retrieved " + total.ToString() + " sets for " + user);

        return setlists;
    }

    public static List<Setlist> compare(List<Setlist> sets1, List<Setlist> sets2)
    {
        return sets1.Intersect(sets2).ToList();
    }

    public static List<String> compareArtists(List<Setlist> sets1, List<Setlist> sets2)
    {
        return sets1.Where(x => sets2.Exists(y => x.artist == y.artist)).Select(z => z.artist.name).Distinct().ToList();
    }
}