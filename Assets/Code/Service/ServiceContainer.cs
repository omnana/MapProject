//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Omnana
//{
//    public class ServiceContainer
//    {
//        private static Dictionary<Type, ServiceBase> serviceContainer = new Dictionary<Type, ServiceBase>();

//        /// <summary>
//        /// 清空容器
//        /// </summary>
//        public static void Clear()
//        {
//            serviceContainer.Clear();
//        }

//        /// <summary>
//        /// 添加服务
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        public static void AddService<T>(GameObject parent) where T : ServiceBase
//        {
//            var type = typeof(T);

//            if (serviceContainer.ContainsKey(type))
//            {
//                return;
//            }

//            var service = parent.AddComponent<T>();

//            service.Loaded();

//            serviceContainer.Add(type, service);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <returns></returns>
//        public static T Resolve<T>() where T : ServiceBase
//        {
//            var type = typeof(T);

//            if (serviceContainer.ContainsKey(type))
//            {
//                return serviceContainer[type] as T;
//            }

//            return null;
//        }
//    }

//}