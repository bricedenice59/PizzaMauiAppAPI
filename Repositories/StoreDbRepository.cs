
using PizzaMauiApp.API.Core.EntityFramework;
using PizzaMauiApp.API.DbStore;

namespace PizzaMauiApp.API.Repositories;

public class StoreDbRepository<T>(ApplicationDbContext dbContext) : GenericRepository<T>(dbContext)
    where T : BaseModel;