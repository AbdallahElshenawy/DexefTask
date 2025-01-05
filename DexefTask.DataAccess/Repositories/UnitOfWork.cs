namespace DexefTask.DataAccess.Repositories
{
    internal class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
    {
        // Holds the current database transaction, allowing multiple operations to be grouped into a single atomic unit.
        private IDbContextTransaction? _transaction;

        // Private fields for lazy initialization of repositories.
        private IBaseRepository<Book>? _bookRepository;
        private IBaseRepository<BorrowedBook>? _borrowedBookRepository;


        // Repository property for Book entities.
        public IBaseRepository<Book> Books
        {
            get
            {
                _bookRepository ??= new BaseRepository<Book>(context);
                return _bookRepository;
            }
        }

        // Repository property for BorrowedBook entities.

        public IBaseRepository<BorrowedBook> BorrowedBooks
        {
            get
            {
                _borrowedBookRepository ??= new BaseRepository<BorrowedBook>(context);
                return _borrowedBookRepository;
            }
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        public async Task<int> Complete()
        {
            int result;

            try
            {
                result = await context.SaveChangesAsync();

                if (_transaction != null)
                {
                    await CommitTransactionAsync();
                }
            }
            catch (Exception)
            {
                if (_transaction != null)
                {
                    await RollbackTransactionAsync();
                }

                throw;
            }

            return result;
        }
        public void Dispose()
        {
            context.Dispose();
            _transaction?.Dispose();
        }
    }
}
