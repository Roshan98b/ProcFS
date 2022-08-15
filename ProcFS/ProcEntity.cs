namespace ProcFS
{
    /// <summary>
    /// The Process Entity class that contains properties of a process
    /// </summary>
    public class ProcEntity
    {
        /// <summary>
        /// The process ID
        /// </summary>
        public int PID { get; set; }

        /// <summary>
        /// The command name of the process 
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The parent process ID 
        /// </summary>
        public int PPID { get; set; }

        /// <summary>
        /// The virtual memory size of a process
        /// </summary>
        public string? VSize { get; set; }

        /// <summary>
        /// The time consumed by the process in user mode
        /// </summary>
        public string? Utime { get; set; }

        /// <summary>
        /// The time consumed by the process in kernel mode
        /// </summary>
        public string? Stime { get; set; }

        /// <summary>
        /// The user that created the process
        /// </summary>
        public string? User { get; set; }
    }
}