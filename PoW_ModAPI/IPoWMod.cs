namespace ModAPI
{
    //Dependency for ModAPI since ModAPI handles resource loading redirection

    public interface IPoWMod
    {
        public string GetVersion();

        public string GetName();

        public void Load();

        public void Unload();
    }
}