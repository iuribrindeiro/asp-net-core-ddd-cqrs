using System.Threading.Tasks;

namespace ProductsService.Presentation.Infra.ApplicationContext
{
    public interface IUnitOfWork
    {
        void SaveChanges();
        Task SaveChangesAsync();
    }
}