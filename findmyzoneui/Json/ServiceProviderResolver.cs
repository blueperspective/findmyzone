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
                contract.DefaultCreator = () => serviceProvider.GetService(objectType);
                return contract;
            }

            return base.CreateObjectContract(objectType);
        }
    }
}
