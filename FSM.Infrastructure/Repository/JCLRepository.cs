using FSM.Core.Entities;
using FSM.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSM.Infrastructure.Repository
{
   public class JCLRepository: GenericRepository<FsmContext, JCL>, IJCLRepository
    {

        public int DeleteJcLITem(Guid jCLId)
        {
            try
            {
                Context.JCLColor_Mapping.RemoveRange(Context.JCLColor_Mapping.Where(i => i.JCLId == jCLId));
                Context.SaveChanges();
                Context.JCLProducts_Mapping.RemoveRange(Context.JCLProducts_Mapping.Where(i => i.JCLId == jCLId));
                Context.SaveChanges();
                Context.JCLSize_Mapping.RemoveRange(Context.JCLSize_Mapping.Where(i => i.JCLId == jCLId));
                Context.SaveChanges();
                Context.JcLExtraproductMapping.RemoveRange(Context.JcLExtraproductMapping.Where(i => i.JCLId == jCLId));
                Context.SaveChanges();
                return 1;
            }

            catch(Exception ex)
            {
                throw ex;
            }
           
        }
    }
}
