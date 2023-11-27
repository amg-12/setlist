namespace slfm
{
    public record class Comp(String name, int count1, int count2)
    {
        public int min()
        {
            return Math.Min(count1, count2);
        }

        public int max()
        {
            return Math.Max(count1, count2);
        }
    }
}