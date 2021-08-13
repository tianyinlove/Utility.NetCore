using NoticeWorkerService.Model;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoticeWorkerService.Core
{
    /// <summary>
    /// AutoMapper设置
    /// </summary>
    public static class AutoMapperConfig
    {
        static readonly Lazy<IMapper> _mapper = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(ConfigMapper);
            return config.CreateMapper();
        });

        /// <summary>
        /// 模型配置
        /// </summary>
        /// <param name="config"></param>
        public static void ConfigMapper(IMapperConfigurationExpression config)
        {
        }

        public static IMapper CreateMapper()
        {
            return _mapper.Value;
        }


        /// <summary>
        /// 数据转换
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> Convert<S, T>(this List<S> list)
        {
            if (list == null)
            {
                return null;
            }
            return list.Select(d => _mapper.Value.Map<S, T>(d)).ToList();
        }

        /// <summary>
        /// 数据转换
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static T Convert<S, T>(this S model)
        {
            if (model == null)
            {
                return default(T);
            }
            return _mapper.Value.Map<S, T>(model);
        }
    }
}
