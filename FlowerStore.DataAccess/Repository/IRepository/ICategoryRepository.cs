using FlowerStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerStore.DataAccess.Repository.IRepository
{
    public interface ICategoryRepository: IRepository<Category>
    {
        void Update(Category obj);
    }
}
