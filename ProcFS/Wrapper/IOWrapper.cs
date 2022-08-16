namespace ProcFS.Wrapper
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    /// <summary>
    /// The IOWrapper class which acts as a wrapper for all IO operations and also helps in creating mocking instance for tests
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class IOWrapper : IIOWrapper
    {
        /// <summary>
        /// The method that returns enumeration of directories in the path
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The enumeration of directory names</returns>
        public IEnumerable<string> EnumerateDirectories(string path)
        {
            return Directory.EnumerateDirectories(path);
        }

        /// <summary>
        /// The method that checks if a file exists or not
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The bool value</returns>
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// The method that returns all lines in the file
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The lines in the file</returns>
        public string[] ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }

        /// <summary>
        /// The method that returns the content of the file
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The content of the file</returns>
        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }
    }
}