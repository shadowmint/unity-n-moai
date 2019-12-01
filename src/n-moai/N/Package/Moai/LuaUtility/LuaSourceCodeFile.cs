namespace N.Package.Moai.LuaUtility
{
    public struct LuaSourceCodeFile
    {
        public string SourceFile;
        public string DestinationFile;
        public string DestinationPath;
        public bool DestinationPathExists { get; set; }
        public bool DestinationFileExists { get; set; }
        public bool DestinationFileChanged { get; set; }
    }
}