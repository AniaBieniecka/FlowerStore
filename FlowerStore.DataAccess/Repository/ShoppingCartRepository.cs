using FlowerStore.DataAccess.Data;
using FlowerStore.DataAccess.Repository.IRepository;
using FlowerStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlowerStore.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart> , IShoppingCartRepository
    {
        private ApplicationDbContext _db;
        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ShoppingCart obj)
        {
            _db.ShoppingCarts.Update(obj);
        }
    }
}
