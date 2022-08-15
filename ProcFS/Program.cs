namespace ProcFS
{
    /// <summary>
    /// The Program class which contains the main method for execution
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main method
        /// </summary>
        /// <param name="args">The arguments array</param>
        static void Main(string[] args)
        {
            var proc = new Proc();
            proc.StartProcess();
        }
    }
}
