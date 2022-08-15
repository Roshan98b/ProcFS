namespace ProcFS.Tests
{
    using System.Collections.Generic;
    using Moq;
    using ProcFS;
    using ProcFS.Wrapper;

    [TestClass]
    public class ProcTest
    {
        private Mock<IIOWrapper>? mockIOWrapper;

        private Proc? procObject;

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
            mockIOWrapper.Setup(x => x.ReadAllText(It.Is<string>(x => x.Contains("stat")))).Returns(
                "1 (systemd) S 0 1 1 0 -1 4194560 20196 2249222 157 4204 93 228 62152 13189 20 0 1 0 43 106364928 3121 18446744073709551615 1 1 0 0 0 0 671173123 4096 1260 0 0 0 17 0 0 0 0 0 0 0 0 0 0 0 0 0 0"
            );
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
                    "robadrin:x:1000:1000:Ubuntu:/home/robadrin:/bin/bash",
                    "lxd:x:998:100::/var/snap/lxd/common/lxd:/bin/false"
                }
            );
            mockIOWrapper.Setup(x => x.ReadAllLines(It.Is<string>(x => x.Contains("status")))).Returns(
                new string[] {
                    "Name:\tsystemd",
                    "Umask:  0000",
                    "State:  S (sleeping)",
                    "Tgid:   1",
                    "Ngid:   0",
                    "Pid:    1",
                    "PPid:   0",
                    "TracerPid:      0",
                    "Uid:\t0\t0\t0\t0",
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
            procObject = new Proc(mockIOWrapper.Object);
        }

        [TestMethod]
        public void Proc_Test_Valid_Output()
        {
            var stringWriter = new StringWriter();
            Console.SetOut(stringWriter);

            procObject?.StartProcess();

            Assert.IsNotNull(stringWriter.ToString(), "Output is not valid");
            Assert.IsFalse(stringWriter.ToString().Contains("Directory cannot be accessed"), "Output is not valid");
            Assert.IsFalse(stringWriter.ToString().Contains("Directory is not found"), "Output is not valid");
            Assert.IsFalse(stringWriter.ToString().Contains("Exception"), "Output is not valid");
        }
    }
}