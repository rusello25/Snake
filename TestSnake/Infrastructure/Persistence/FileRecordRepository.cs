using TestSnake.Domain.Repositories;

namespace TestSnake.Infrastructure.Persistence
{
    public class FileRecordRepository : IRecordRepository
    {
        private const string RecordFileName = "record.txt";
        
        public async Task<int> GetRecordAsync()
        {
            if (!File.Exists(RecordFileName))
                return 0;

            try
            {
                var text = await File.ReadAllTextAsync(RecordFileName);
                return int.TryParse(text.Trim(), out int record) ? record : 0;
            }
            catch
            {
                return 0;
            }
        }

        public async Task SaveRecordAsync(int record)
        {
            try
            {
                await File.WriteAllTextAsync(RecordFileName, record.ToString());
            }
            catch
            {
                // Silently fail if we can't save the record
            }
        }
    }
}