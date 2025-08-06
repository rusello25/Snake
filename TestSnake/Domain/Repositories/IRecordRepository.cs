namespace TestSnake.Domain.Repositories
{
    public interface IRecordRepository
    {
        Task<int> GetRecordAsync();
        Task SaveRecordAsync(int record);
    }
}