namespace ProcFS.Wrapper
{
    using System.Collections.Generic;

    /// <summary>
    /// The IOwrapper interface
    /// </summary>
    public interface IIOWrapper
    {
        /// <summary>
        /// The method that returns enumeration of directories in the path
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The enumeration of directory names</returns>
        public IEnumerable<string> EnumerateDirectories(string path);

        /// <summary>
        /// The method that checks if a file exists or not
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The bool value</returns>
        public bool Exists(string path);

        /// <summary>
        /// The method that returns all lines in the file
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The lines in the file</returns>
        public string[] ReadAllLines(string path);

        /// <summary>
        /// The method that returns the content of the file
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The content of the file</returns>
        public string ReadAllText(string path);
    }
}