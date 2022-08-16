namespace ProcFS
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using ProcFS.Wrapper;

    /// <summary>
    /// The Proc class that contains helpers to diplay process details
    /// </summary>
    public class Proc
    {
        /// <summary>
        /// The object that has list of all process details
        /// </summary>
        private List<ProcEntity> Procs { get; set; }

        /// <summary>
        /// The /etc/passwd file content
        /// </summary>
        private string[]? passwdFileContent;

        /// <summary>
        /// The _SC_CLK_TCK value that represents clock ticks per second
        /// </summary>
        private static int CLK_TCK { get; set; }

        private IIOWrapper Wrapper { get; set; }

        /// <summary>
        /// The Proc constructor
        /// </summary>
        public Proc(IIOWrapper wrapper)
        {
            Procs = new List<ProcEntity>();
            Wrapper = wrapper;
            passwdFileContent = GetPasswdFileContent();
            CLK_TCK = 100; // Default value of clock ticks per second
        }

        /// <summary>
        /// The public method that is called to start the computation
        /// </summary>
        public void StartProcess()
        {
            try
            {
                // Get all PIDs in /proc directory
                var PIDs = GetProcess("/proc");

                foreach (var PID in PIDs)
                {
                    var proc = new ProcEntity();
                    if (ParseStatFile(proc, PID) && GetUser(proc, PID))
                    {
                        Procs.Add(proc);
                    }
                }

                Display();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception : {ex}");
            }
        }

        /// <summary>
        /// The method used to get content of /etc/passwd file
        /// </summary>
        /// <returns>Lines of /etc/passwd file</returns>
        private string[]? GetPasswdFileContent()
        {
            // Read /etc/passwd file for username-UID mappings
            var passwdFilePath = "/etc/passwd";
            if (Wrapper.Exists(passwdFilePath))
            {
                return Wrapper.ReadAllLines(passwdFilePath);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// The method used to retrieve all PIDs based on the files in /proc
        /// </summary>
        /// <param name="path">The path of Proc FS : /proc</param>
        /// <returns>List of PIDs</returns>
        private List<int> GetProcess(string path)
        {
            var PIDs = new List<int>();

            try
            {
                var dirs = new List<string>(Wrapper.EnumerateDirectories(path));

                foreach (var dir in dirs)
                {
                    if (int.TryParse(dir.Split('/').LastOrDefault(), out int PID))
                    {
                        PIDs.Add(PID);
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Directory cannot be accessed : {ex.Message}");
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Directory is not found : {ex.Message}");
            }

            return PIDs;
        }

        /// <summary>
        /// The method used to parse /proc/[pid]/stat file
        /// </summary>
        /// <param name="proc">The object that contains process details for a PID</param>
        /// <param name="PID">The PID of the process</param>
        private bool ParseStatFile(ProcEntity proc, int PID)
        {
            try
            {
                var statFilePath = $"/proc/{PID}/stat";
                if (Wrapper.Exists(statFilePath))
                {
                    var content = Wrapper.ReadAllText(statFilePath);
                    if (!string.IsNullOrEmpty(content))
                    {
                        (int commStartIndex, int commLastIndex) = GetCommandIndices(content);
                        if (commStartIndex != -1 && commLastIndex != -1)
                        {
                            proc.PID = int.Parse(content.Substring(0, commStartIndex - 1));
                            proc.Name = content.Substring(commStartIndex + 1, (commLastIndex - commStartIndex) - 1);

                            // Split the rest of the stat file content by whitespace
                            var restOftheContent = content.Substring(commLastIndex + 2)?.Split(' ');

                            if (restOftheContent != null && restOftheContent.Length > 21)
                            {
                                proc.PPID = int.Parse(restOftheContent[1]);
                                proc.VSize = GetVirtualMemorySize(double.Parse(restOftheContent[20]));
                                proc.Utime = Math.Round(double.Parse(restOftheContent[11]) / CLK_TCK, 2).ToString() + "sec";
                                proc.Stime = Math.Round(double.Parse(restOftheContent[12]) / CLK_TCK, 2).ToString() + "sec";
                                return true;
                            }
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Directory cannot be accessed : {ex.Message}");
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Directory is not found : {ex.Message}");
            }
            return false;
        }

        /// <summary>
        /// The method used to get user who created the process
        /// </summary>
        /// <param name="proc">The object that contains process details for a PID</param>
        /// <param name="PID">The PID of the process</param>
        private bool GetUser(ProcEntity proc, int PID)
        {
            try
            {
                var statusFilePath = $"/proc/{PID}/status";
                if (Wrapper.Exists(statusFilePath))
                {
                    // Find the line that contains "Uid:" in the status file
                    var uidLine = Wrapper.ReadAllLines(statusFilePath)?.Where(x => x.StartsWith("Uid:")).FirstOrDefault();
                    if (!string.IsNullOrEmpty(uidLine))
                    {
                        // Retrieve Real UID after spliting by tab space
                        var uid = uidLine.Split('\t')?[1];

                        // Find the line in /etc/passwd which contains the UID
                        var passwdUidLine = passwdFileContent?.Where(x => x.Contains($":{uid}:")).FirstOrDefault();
                        if (!string.IsNullOrEmpty(passwdUidLine))
                        {
                            // Username is the first element after spliting the line by ':' 
                            proc.User = passwdUidLine.Split(':').First();
                        }
                        else
                        {
                            // Username not present in /etc/passwd file and hence using UID as the user
                            proc.User = uid;
                        }
                        return true;
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Directory cannot be accessed : {ex.Message}");
            }
            catch (DirectoryNotFoundException ex)
            {
                Console.WriteLine($"Directory is not found : {ex.Message}");
            }
            return false;
        }

        /// <summary>
        /// The method used to find the starting and ending indices of command in /proc/[pid]/stat file
        /// </summary>
        /// <param name="content">The contents of /proc/[pid]/stat file</param>
        /// <returns>The starting and ending indices of command in /proc/[pid]/stat file</returns>
        private (int, int) GetCommandIndices(string content)
        {
            try
            {
                //// command parsing
                //// command name may contain whitespace
                int commStartIndex = -1, commLastIndex = -1;
                for (int i = 0; i < content.Length; i++)
                {
                    if (content[i] == '(')
                    {
                        commStartIndex = i;
                        break;
                    }
                }

                for (int j = content.Length - 1; j > 0; j--)
                {
                    if (content[j] == ')')
                    {
                        commLastIndex = j;
                        break;
                    }
                }
                return (commStartIndex, commLastIndex);
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine($"Index out of bounds : {content} : {ex.Message}");
                return (-1, -1);
            }
        }

        /// <summary>
        /// The method used to parse Virtual Memory size 
        /// </summary>
        /// <param name="vSize">The virtual memory size of type double</param>
        /// <returns>The virtual memory size value in string with units</returns>
        private string GetVirtualMemorySize(double vSize)
        {
            var unit = "B";

            if (vSize > 1024)
            {
                vSize = vSize / 1024;
                unit = "KB";
            }

            if (vSize > 1024)
            {
                vSize = vSize / 1024;
                unit = "MB";
            }

            if (vSize > 1024)
            {
                vSize = vSize / 1024;
                unit = "GB";
            }

            return $"{Math.Round(vSize, 1)}{unit}";
        }

        /// <summary>
        /// The method used to display process details
        /// </summary>
        private void Display()
        {
            Console.WriteLine(string.Format("{0, 18} {1, 6} {2, 6} {3, 8} {4, 12} {5, 12} {6}", "USER", "PID", "PPID", "VMSIZE", "UTIME", "STIME", "NAME"));
            foreach (var proc in Procs)
            {
                Console.WriteLine(string.Format("{0, 18} {1, 6} {2, 6} {3, 8} {4, 12} {5, 12} {6}", proc.User, proc.PID, proc.PPID, proc.VSize, proc.Utime, proc.Stime, proc.Name));
            }
        }
    }
}