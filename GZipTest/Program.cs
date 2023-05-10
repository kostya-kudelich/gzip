using GZipTest.ParallelGZipArchiver;
using System;

namespace GZipTest {
    class Program {
        static void Main(string[] args) {
            try {
                var arguments = ArgumentsProvider.GetArguments(args);
                ParallelGzipArchiver.Operate(arguments);
                Console.WriteLine(0);
            } catch (Exception ex) {
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine(1);
            }
        }
    }
}
