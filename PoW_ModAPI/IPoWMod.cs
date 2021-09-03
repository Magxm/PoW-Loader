namespace ModAPI
{
    public interface IPoWMod
    {
        public string GetVersion();

        public string GetName();

        public void Load();

        public void Unload();
    }
}