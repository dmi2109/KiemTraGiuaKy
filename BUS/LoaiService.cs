using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class LoaiService
    {
        
        private readonly SPModel context = new SPModel();


        public List<LoaiSP> GetAll()
        {
            return context.LoaiSP.ToList();
        }
    }
}

