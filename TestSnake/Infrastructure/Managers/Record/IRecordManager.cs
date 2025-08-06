namespace TestSnake.Infrastructure.Managers.Record
{
    public interface IRecordManager
    {
        int LoadRecord();
        void SaveRecord(int record);
        void CheckAndUpdateRecord(int score);
    }
}