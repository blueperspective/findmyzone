using Newtonsoft.Json.Serialization;
using System;

namespace findmyzoneui.Json
{
    class ServiceProviderResolver : DefaultContractResolver
    {
        private readonly IServiceProvider serviceProvider;

        public ServiceProviderResolver(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            if (serviceProvider.GetService(objectType) != null)
            {
                JsonObjectContract contract = base.CreateObjectContract(objectType);
                contract.DefaultCreator = () =>
                {
                    var obj = serviceProvider.GetService(objectType);
                    if (obj == null)
                    {
                        throw new ArgumentException($"Service Provider could not create service {objectType}");
                    }
                    return obj;
                };

                return contract;
            }

            return base.CreateObjectContract(objectType);
        }
    }
}
