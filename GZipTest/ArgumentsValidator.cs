using System;
using System.IO;

namespace GZipTest {
    public static class ArgumentsValidator {
        private const int ProgramArgumentsCount = 3;

        public static void CheckFiles(string inputFile, string outputFile) {
            CheckFileNameExistence(inputFile);
            CheckFileNameExistence(outputFile);
            CheckFileExistence(inputFile);
            CheckFileNameDifference(inputFile, outputFile);
        }

        public static void CheckProgramArgs(string[] args) {
            if (args.Length != ProgramArgumentsCount) {
                throw new Exception($"Invalid arguments. Please, check arguments format: \"compress/decompress inputFile outputFile.");
            }
        }
        private static void CheckFileExistence(string file) {
            if (!File.Exists(file)) {
                throw new Exception($"File {file} does not exist. Please, check file name and path.");
            }
        }

        private static void CheckFileNameExistence(string fileName) {
            if (string.IsNullOrWhiteSpace(fileName)) {
                throw new Exception("Invalid file argument.");
            }
        }

        private static void CheckFileNameDifference(string inputFileName, string outputFileName) {
            if (inputFileName == outputFileName) {
                throw new Exception($"Compressing and decompressing files have the same names.");
            }
        }
    }
}
