namespace GZipTest.ParallelGZipArchiver {
    public static class ParallelGzipArchiver {
        public static void Operate(Arguments args) {
            if (args.OperationType == OperationType.Compress) {
                var compressor = new ParallelCompressor(args.InputFile, args.OutputFile);
                compressor.Execute();
            } else {
                var decompressor = new ParallelDecompressor(args.InputFile, args.OutputFile);
                decompressor.Execute();
            }
        }
    }
}
