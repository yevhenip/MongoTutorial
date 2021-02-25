namespace MongoTutorial.Core.Settings.CacheSettings
{
    public abstract class CacheBaseSettings
    {
        public int AbsoluteExpiration { get; set; }
        public int SlidingExpiration { get; set; }
    }
}