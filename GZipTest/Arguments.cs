namespace GZipTest {
    public class Arguments {
        public OperationType OperationType { get; private set; }
        public string InputFile { get; private set; }
        public string OutputFile { get; private set; }
        public Arguments(OperationType operationType, string inputFile, string outputFile) {
            OperationType = operationType;
            InputFile = inputFile;
            OutputFile = outputFile;
        }
    }
}
