namespace ProcFS.Wrapper
{
    using System.Collections.Generic;

    public interface IIOWrapper
    {
        public IEnumerable<string> EnumerateDirectories(string path);

        public bool Exists(string path);

        public string ReadAllText(string path);

        public string[] ReadAllLines(string path);
    }
}