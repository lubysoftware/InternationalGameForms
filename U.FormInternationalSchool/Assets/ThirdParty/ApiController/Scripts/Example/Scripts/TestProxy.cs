using System;

namespace API.Example
{
    [Serializable]
    public class ExampleModel
    {
        public int userId;
        public int id;
        public string title;
        public string body;
        public bool completed;
    }
}