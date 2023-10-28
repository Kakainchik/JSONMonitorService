using FileMonitorService.JsonService;

namespace FileMonitorServiceTest
{
    public class JsonMonitorTest
    {
        private const string RELATIVE_PATH = "./JsonResources";

        [Fact]
        public void MonitorUnchangedFile_ModifyFile_MonitorReturnsTrue()
        {
            string path = Path.GetFullPath($"{RELATIVE_PATH}/AskReddit.json");
            using IJsonMonitor monitor = new JsonMonitor(path);

            //First check without ran monitoring will always false cause the data is default
            //We initialize the data first
            monitor.ForceCheck();

            //Check second time with no changes in the file
            bool containChanges = monitor.ForceCheck();
            Assert.False(containChanges);

            //Try to modify the file by swapping the first character of property to 0 symbol
            using FileStream fs = File.OpenWrite(path);
            using StreamWriter sw = new StreamWriter(fs);
            fs.Seek(6, SeekOrigin.Begin);
            sw.Write(0);
            sw.Close();
            //The JSON structure is not supposed to be corrupted, nevertheless the service must notice the modification
            containChanges = monitor.ForceCheck();

            Assert.True(containChanges);
        }
    }
}