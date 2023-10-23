public record class Setlist(string id, string eventDate, Named artist, Named venue)
{
    public override string ToString()
    {
        return artist.name + " @ " + venue.name + ", " + eventDate.Replace('-', '/');
    }
}