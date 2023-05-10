using System;

namespace GZipTest {
    public static class ArgumentsProvider {
        public static Arguments GetArguments(string[] args) {
            ArgumentsValidator.CheckProgramArgs(args);

            var operationType = ParseOperationType(args[0]);
            var inputFile = args[1];
            var outputFile = args[2];

            ArgumentsValidator.CheckFiles(inputFile, outputFile);

            return new Arguments(operationType, inputFile, outputFile);
        }

        private static OperationType ParseOperationType(string operation) {
            if (operation.ToLower() == "compress") {
                return OperationType.Compress;
            } else if (operation.ToLower() == "decompress") {
                return OperationType.Decompress;
            } else {
                throw new Exception($"\"{operation}\" is not a valid operation type. Please, use compress/decompress.");
            }
        }
    }
}
