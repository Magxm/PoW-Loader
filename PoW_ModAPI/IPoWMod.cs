namespace ModAPI
{
    public interface IPoWMod
    {
        string GetVersion();

        string GetName();

        void Load();

        void Unload();
    }
}