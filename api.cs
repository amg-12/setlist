﻿using System.Net.Http.Headers;
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

    static async Task<List<Setlist>> compareUsers(HttpClient client, string user1, string user2)
    {
        List<Setlist> sets1 = await getAllSetlists(user1);
        List<Setlist> sets2 = await getAllSetlists(user2);

        return sets1.Intersect(sets2).ToList();
    }
}