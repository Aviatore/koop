using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Koop.Models.Repositories
{
    public class GenericUnitOfWork : IDisposable, IGenericUnitOfWork
    {
        private bool disposed = false;
        private KoopDbContext _koopDbContext;
        private Dictionary<Type, object> _repositories;
        private Dictionary<Type, object> _repositoriesView;

        public GenericUnitOfWork(KoopDbContext koopDbContext)
        {
            _koopDbContext = koopDbContext;
            _repositories = new Dictionary<Type, object>();
            _repositoriesView = new Dictionary<Type, object>();
        }

        public IRepository<T> Repository<T>() where T : class
        {
            if (_repositories.ContainsKey(typeof(T)))
            {
                return _repositories[typeof(T)] as IRepository<T>;
            }

            IRepository<T> newRepository = new GenericRepository<T>(_koopDbContext);
            
            _repositories.Add(typeof(T), newRepository);

            return newRepository;
        }

        public IRepositoryView<T> RepositoryView<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            _koopDbContext.SaveChanges();
        }

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _koopDbContext.Dispose();
                }
            }

            this.disposed = true;
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}