using TestSnake.Domain.Repositories;

namespace TestSnake.Core.Persistence
{
    /// <summary>
    /// Unit of Work pattern interface for managing transactional operations.
    /// Coordinates the work of multiple repositories and maintains consistency.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the record repository.
        /// </summary>
        IRecordRepository Records { get; }

        /// <summary>
        /// Gets the game state repository.
        /// </summary>
        IGameStateRepository GameStates { get; }

        /// <summary>
        /// Commits all pending changes to the underlying storage.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Number of entities affected</returns>
        Task<int> CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolls back all pending changes.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Begins a new transaction scope.
        /// </summary>
        /// <returns>Transaction scope</returns>
        ITransactionScope BeginTransaction();
    }

    /// <summary>
    /// Represents a transaction scope for database operations.
    /// </summary>
    public interface ITransactionScope : IDisposable
    {
        /// <summary>
        /// Commits the transaction.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the commit operation</returns>
        Task CommitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolls back the transaction.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Gets whether the transaction has been completed (committed or rolled back).
        /// </summary>
        bool IsCompleted { get; }
    }

    /// <summary>
    /// File-based implementation of the Unit of Work pattern.
    /// Manages file operations as transactions with rollback capability.
    /// </summary>
    public sealed class FileUnitOfWork(
        IRecordRepository recordRepository,
        IGameStateRepository gameStateRepository) : IUnitOfWork
    {
        private readonly IRecordRepository _recordRepository = recordRepository ?? throw new ArgumentNullException(nameof(recordRepository));
        private readonly IGameStateRepository _gameStateRepository = gameStateRepository ?? throw new ArgumentNullException(nameof(gameStateRepository));
        private readonly List<IFileOperation> _pendingOperations = [];
        private readonly Dictionary<string, string> _backupFiles = [];
        private bool _disposed;

        /// <inheritdoc />
        public IRecordRepository Records => _recordRepository;

        /// <inheritdoc />
        public IGameStateRepository GameStates => _gameStateRepository;

        /// <inheritdoc />
        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            try
            {
                // Create backups before executing operations
                await CreateBackupsAsync(cancellationToken);

                // Execute all pending operations
                int affectedCount = 0;
                foreach (var operation in _pendingOperations)
                {
                    await operation.ExecuteAsync(cancellationToken);
                    affectedCount++;
                }

                // Clear pending operations after successful commit
                _pendingOperations.Clear();
                _backupFiles.Clear();

                return affectedCount;
            }
            catch
            {
                // Rollback on any failure
                await RollbackAsync();
                throw;
            }
        }

        /// <inheritdoc />
        public void Rollback()
        {
            ThrowIfDisposed();

            try
            {
                // Restore from backups
                foreach (var backup in _backupFiles)
                {
                    if (File.Exists(backup.Value))
                    {
                        File.Copy(backup.Value, backup.Key, overwrite: true);
                        File.Delete(backup.Value);
                    }
                }

                _pendingOperations.Clear();
                _backupFiles.Clear();
            }
            catch (Exception ex)
            {
                // Log error but don't throw to avoid masking original exception
                Console.WriteLine($"Error during rollback: {ex.Message}");
            }
        }

        /// <inheritdoc />
        public ITransactionScope BeginTransaction()
        {
            ThrowIfDisposed();
            return new FileTransactionScope(this);
        }

        /// <summary>
        /// Adds a file operation to the pending operations list.
        /// </summary>
        /// <param name="operation">Operation to add</param>
        internal void AddOperation(IFileOperation operation)
        {
            ThrowIfDisposed();
            _pendingOperations.Add(operation);
        }

        private async Task CreateBackupsAsync(CancellationToken cancellationToken)
        {
            foreach (var operation in _pendingOperations)
            {
                if (operation.RequiresBackup && File.Exists(operation.TargetFilePath))
                {
                    var backupPath = $"{operation.TargetFilePath}.backup.{DateTime.UtcNow:yyyyMMddHHmmss}";
                    await File.WriteAllBytesAsync(
                        backupPath, 
                        await File.ReadAllBytesAsync(operation.TargetFilePath, cancellationToken), 
                        cancellationToken);
                    
                    _backupFiles[operation.TargetFilePath] = backupPath;
                }
            }
        }

        private async Task RollbackAsync()
        {
            foreach (var backup in _backupFiles)
            {
                if (File.Exists(backup.Value))
                {
                    await File.WriteAllBytesAsync(
                        backup.Key, 
                        await File.ReadAllBytesAsync(backup.Value));
                    File.Delete(backup.Value);
                }
            }

            _pendingOperations.Clear();
            _backupFiles.Clear();
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                // Cleanup any remaining backup files
                foreach (var backupPath in _backupFiles.Values)
                {
                    try
                    {
                        if (File.Exists(backupPath))
                            File.Delete(backupPath);
                    }
                    catch
                    {
                        // Ignore cleanup errors
                    }
                }

                _backupFiles.Clear();
                _pendingOperations.Clear();
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// File-based transaction scope implementation.
    /// </summary>
    internal sealed class FileTransactionScope(FileUnitOfWork unitOfWork) : ITransactionScope
    {
        private readonly FileUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private bool _disposed;
        private bool _completed;

        /// <inheritdoc />
        public bool IsCompleted => _completed;

        /// <inheritdoc />
        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            ThrowIfCompleted();

            await _unitOfWork.CommitAsync(cancellationToken);
            _completed = true;
        }

        /// <inheritdoc />
        public void Rollback()
        {
            ThrowIfDisposed();
            ThrowIfCompleted();

            _unitOfWork.Rollback();
            _completed = true;
        }

        private void ThrowIfDisposed()
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
        }

        private void ThrowIfCompleted()
        {
            if (_completed)
                throw new InvalidOperationException("Transaction has already been completed.");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (!_disposed)
            {
                if (!_completed)
                {
                    try
                    {
                        Rollback();
                    }
                    catch
                    {
                        // Ignore rollback errors during disposal
                    }
                }

                _disposed = true;
            }
        }
    }

    /// <summary>
    /// Represents a file operation that can be executed as part of a transaction.
    /// </summary>
    public interface IFileOperation
    {
        /// <summary>
        /// Gets the target file path for this operation.
        /// </summary>
        string TargetFilePath { get; }

        /// <summary>
        /// Gets whether this operation requires a backup before execution.
        /// </summary>
        bool RequiresBackup { get; }

        /// <summary>
        /// Executes the file operation.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task representing the operation</returns>
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// File write operation implementation.
    /// </summary>
    public sealed class WriteFileOperation(string targetFilePath, string content) : IFileOperation
    {
        private readonly string _content = content ?? throw new ArgumentNullException(nameof(content));

        /// <inheritdoc />
        public string TargetFilePath { get; } = targetFilePath ?? throw new ArgumentNullException(nameof(targetFilePath));

        /// <inheritdoc />
        public bool RequiresBackup => true;

        /// <inheritdoc />
        public async Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var directory = Path.GetDirectoryName(TargetFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await File.WriteAllTextAsync(TargetFilePath, _content, cancellationToken);
        }
    }
}