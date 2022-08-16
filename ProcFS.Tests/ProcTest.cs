namespace ProcFS.Tests
{
    using System.Collections.Generic;
    using Moq;
    using ProcFS;
    using ProcFS.Wrapper;

    /// <summary>
    /// The Proc Test class 
    /// </summary>
    [TestClass]
    public class ProcTest
    {
        /// <summary>
        /// The mock IOWrapper instance
        /// </summary>
        private Mock<IIOWrapper>? mockIOWrapper;

        /// <summary>
        /// The Proc instance
        /// </summary>
        private Proc? procObject;

        /// <summary>
        /// The test initialize method that gets executed before every test execution
        /// </summary>
        [TestInitialize]
        public void Initialize()
        {
            mockIOWrapper = new Mock<IIOWrapper>();
            mockIOWrapper.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            mockIOWrapper.Setup(x => x.EnumerateDirectories("/proc")).Returns(
                new List<string>
                {
                    "/proc/acpi",
                    "/proc/bus",
                    "/proc/driver",
                    "/proc/tty",
                    "/proc/1",
                    "/proc/2",
                    "/proc/3",
                    "/proc/4",
                }
            );

            for (int i = 1; i <= 4; i++)
            {
                var proc = i;
                mockIOWrapper.Setup(x => x.ReadAllText($"/proc/{proc}/stat")).Returns(
                    $"{proc} (command ) {proc}) S 0 1 1 0 -1 4194560 20196 2249222 157 4204 {93 * proc} {228 * proc} 62152 13189 20 0 1 0 43 {900000000 * proc} 3121 18446744073709551615 1 1 0 0 0 0 671173123 4096 1260 0 0 0 17 0 0 0 0 0 0 0 0 0 0 0 0 0 0"
                );
            }

            mockIOWrapper.Setup(x => x.ReadAllLines("/etc/passwd")).Returns(
                new string[] {
                    "root:x:0:0:root:/root:/bin/bash",
                    "daemon:x:1:1:daemon:/usr/sbin:/usr/sbin/nologin",
                    "bin:x:2:2:bin:/bin:/usr/sbin/nologin",
                    "sys:x:3:3:sys:/dev:/usr/sbin/nologin",
                    "gnats:x:41:41:Gnats Bug-Reporting System (admin):/var/lib/gnats:/usr/sbin/nologin",
                    "nobody:x:65534:65534:nobody:/nonexistent:/usr/sbin/nologin",
                    "systemd-network:x:100:102:systemd Network Management,,,:/run/systemd:/usr/sbin/nologin",
                    "systemd-resolve:x:101:103:systemd Resolver,,,:/run/systemd:/usr/sbin/nologin",
                    "_chrony:x:112:120:Chrony daemon,,,:/var/lib/chrony:/usr/sbin/nologin",
                    "systemd-coredump:x:999:999:systemd Core Dumper:/:/usr/sbin/nologin",
                    "robadrin:x:4:4:Ubuntu:/home/robadrin:/bin/bash",
                    "lxd:x:998:100::/var/snap/lxd/common/lxd:/bin/false"
                }
            );

            for (int i = 1; i <= 4; i++)
            {
                var proc = i;
                mockIOWrapper.Setup(x => x.ReadAllLines($"/proc/{proc}/status")).Returns(
                    new string[] {
                        $"Name:\tcommand{proc}",
                        "Umask:  0000",
                        "State:  S (sleeping)",
                        "Tgid:   1",
                        "Ngid:   0",
                        $"Pid:/t{proc}",
                        "PPid:   0",
                        "TracerPid:      0",
                        $"Uid:\t{proc}\t{proc}\t{proc}\t{proc}",
                        "Gid:    0       0       0       0",
                        "FDSize: 128",
                        "Groups:",
                        "NStgid: 1",
                        "NSpid:  1",
                        "NSpgid: 1",
                        "NSsid:  1",
                        "VmPeak:   169408 kB",
                        "VmSize:   103872 kB",
                        "VmLck:         0 kB",
                        "VmPin:         0 kB",
                        "VmHWM:     12484 kB",
                        "VmRSS:     12484 kB",
                        "RssAnon:            4316 kB",
                        "RssFile:            8168 kB",
                        "RssShmem:              0 kB",
                        "VmData:    19552 kB",
                        "VmStk:      1032 kB",
                        "VmExe:       760 kB",
                        "VmLib:      9548 kB",
                        "VmPTE:        84 kB",
                        "VmSwap:        0 kB",
                        "HugetlbPages:          0 kB",
                        "CoreDumping:    0",
                        "THP_enabled:    1",
                        "Threads:        1",
                        "SigQ:   0/31772",
                        "SigPnd: 0000000000000000",
                        "ShdPnd: 0000000000000000",
                        "SigBlk: 7be3c0fe28014a03",
                        "SigIgn: 0000000000001000",
                        "SigCgt: 00000001800004ec",
                        "CapInh: 0000000000000000",
                        "CapPrm: 000001ffffffffff",
                        "CapEff: 000001ffffffffff",
                        "CapBnd: 000001ffffffffff",
                        "CapAmb: 0000000000000000",
                        "NoNewPrivs:     0",
                        "Seccomp:        0",
                        "Seccomp_filters:        0",
                        "Speculation_Store_Bypass:       vulnerable",
                        "SpeculationIndirectBranch:      always enabled",
                        "Cpus_allowed:   3",
                        "Cpus_allowed_list:      0-1",
                        "Mems_allowed:   00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000000,00000001",
                        "Mems_allowed_list:      0",
                        "voluntary_ctxt_switches:        7674",
                        "nonvoluntary_ctxt_switches:     747"
                    }
                );
            }

            procObject = new Proc(mockIOWrapper.Object);
        }

        /// <summary>
        /// The test method that evaluates for suceess output
        /// </summary>
        [TestMethod]
        public void Proc_Test_Valid_Output()
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            procObject?.StartProcess();

            var output = stringWriter.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Assert.IsNotNull(output, "Output is not valid");
            Assert.AreEqual(5, output.Length, "Output is not valid");
            Assert.IsFalse(output.Contains("Directory cannot be accessed"), "Output is not valid");
            Assert.IsFalse(output.Contains("Directory is not found"), "Output is not valid");
            Assert.IsFalse(output.Contains("Exception"), "Output is not valid");
        }

        /// <summary>
        /// The test method that evaluates output when few stat files are not there 
        /// </summary>
        [TestMethod]
        public void Proc_Test_Valid_Outut_Stat_File_Not_Present()
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            // Only 2 stat files
            for (int i = 3; i <= 4; i++)
            {
                var proc = i;
                mockIOWrapper?.Setup(x => x.ReadAllText($"/proc/{proc}/stat")).Returns<string>(null);
            }

            procObject?.StartProcess();

            var output = stringWriter.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Assert.IsNotNull(output, "Output is not valid");
            Assert.AreEqual(3, output.Length, "Output is not valid");
            Assert.IsFalse(output.Contains("Directory cannot be accessed"), "Output is not valid");
            Assert.IsFalse(output.Contains("Directory is not found"), "Output is not valid");
            Assert.IsFalse(output.Contains("Exception"), "Output is not valid");
        }

        /// <summary>
        /// The test method that evaluates output when few status files are not there 
        /// </summary>
        [TestMethod]
        public void Proc_Test_Valid_Outut_Status_File_Not_Present()
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            // Only 2 status files
            for (int i = 1; i <= 2; i++)
            {
                var proc = i;
                mockIOWrapper?.Setup(x => x.ReadAllLines($"/proc/{proc}/status")).Returns<string>(null);
            }

            procObject?.StartProcess();

            var output = stringWriter.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Assert.IsNotNull(output, "Output is not valid");
            Assert.AreEqual(3, output.Length, "Output is not valid");
            Assert.IsFalse(output.Contains("Directory cannot be accessed"), "Output is not valid");
            Assert.IsFalse(output.Contains("Directory is not found"), "Output is not valid");
            Assert.IsFalse(output.Contains("Exception"), "Output is not valid");
        }

        /// <summary>
        /// The test method that evaluates output when few stat files are invalid 
        /// </summary>
        [TestMethod]
        public void Proc_Test_Valid_Outut_Stat_File_Invalid()
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            // Only 2 stat files
            for (int i = 3; i <= 4; i++)
            {
                var proc = i;
                mockIOWrapper?.Setup(x => x.ReadAllText($"/proc/{proc}/stat")).Returns("xyz");
            }

            procObject?.StartProcess();

            var output = stringWriter.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Assert.IsNotNull(output, "Output is not valid");
            Assert.AreEqual(3, output.Length, "Output is not valid");
            Assert.IsFalse(output.Contains("Directory cannot be accessed"), "Output is not valid");
            Assert.IsFalse(output.Contains("Directory is not found"), "Output is not valid");
            Assert.IsFalse(output.Contains("Exception"), "Output is not valid");
        }

        /// <summary>
        /// The test method that evaluates output when /etc/passwd file is invalid 
        /// </summary>
        [TestMethod]
        public void Proc_Test_Valid_Outut_Passwd_File_Invalid()
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            mockIOWrapper?.Setup(x => x.ReadAllLines("/etc/passwd")).Returns(new string[] {
                ""
            });

            if (mockIOWrapper != null)
            {
                procObject = new Proc(mockIOWrapper.Object);
            }
            procObject?.StartProcess();

            var output = stringWriter.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);
            Assert.IsNotNull(output, "Output is not valid");
            Assert.AreEqual(5, output.Length, "Output is not valid");
            Assert.IsFalse(output.Contains("Directory cannot be accessed"), "Output is not valid");
            Assert.IsFalse(output.Contains("Directory is not found"), "Output is not valid");
            Assert.IsFalse(output.Contains("Exception"), "Output is not valid");
        }
    }
}