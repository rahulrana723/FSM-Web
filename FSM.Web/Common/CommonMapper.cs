using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FSM.Web.Common
{
    public class CommonMapper<T, C> where T : class where C : class
    {
        public C Mapper(T sourceObject)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<T, C>();
            });

            IMapper mapper = config.CreateMapper();
            T sourceViewModel = sourceObject;
            C resultEntity = mapper.Map<T, C>(sourceViewModel);

            return resultEntity;
        }

        public List<C> MapToList(List<T> sourceObject)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<T, C>();
            });

            IMapper mapper = config.CreateMapper();
            List<T> sourceViewModel = sourceObject;
            List<C> resultEntity = mapper.Map<List<T>, List<C>>(sourceViewModel);

            return resultEntity;
        }
    }
}