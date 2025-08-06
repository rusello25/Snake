namespace TestSnake.Infrastructure.Managers.Record
{
    public class RecordManager : IRecordManager
    {
        private readonly string _filePath;
        public RecordManager(string appName = "TestSnake")
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string dir = Path.Combine(appData, appName);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            _filePath = Path.Combine(dir, "record.txt");
        }
        public int LoadRecord()
        {
            if (File.Exists(_filePath))
            {
                string content = File.ReadAllText(_filePath);
                if (int.TryParse(content, out int record))
                    return record;
            }
            return 0;
        }
        public void SaveRecord(int record)
        {
            File.WriteAllText(_filePath, record.ToString());
        }
        public void CheckAndUpdateRecord(int score)
        {
            int record = LoadRecord();
            if (score > record)
                SaveRecord(score);
        }
    }
}