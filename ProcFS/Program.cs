namespace ProcFS
{
    using System.Diagnostics.CodeAnalysis;
    using ProcFS.Wrapper;

    /// <summary>
    /// The Program class which contains the main method for execution
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Program
    {
        /// <summary>
        /// The main method
        /// </summary>
        /// <param name="args">The arguments array</param>
        static void Main(string[] args)
        {
            var proc = new Proc(new IOWrapper());
            proc.StartProcess();
        }
    }
}
