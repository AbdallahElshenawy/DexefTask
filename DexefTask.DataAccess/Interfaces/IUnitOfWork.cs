namespace DexefTask.DataAccess.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<Book> Books { get; }
        IBaseRepository<BorrowedBook> BorrowedBooks { get; }

        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        Task<int> Complete();

    }
}

