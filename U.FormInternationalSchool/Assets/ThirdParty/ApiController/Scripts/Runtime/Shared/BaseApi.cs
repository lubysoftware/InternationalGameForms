namespace API
{
    public abstract class BaseApi
    {        
        public void Initialize(string basePath, IApi api, bool debugMode)
        {
            _basePath = basePath;
            Api = api;
            DebugMode = debugMode;
        }

        protected string GetFormattedPath(string path)
        {
            return _basePath + path;
        }
        
        protected IApi Api { get; private set; }
        protected bool DebugMode { get; private set; }
        
        protected string _basePath;
    }
}