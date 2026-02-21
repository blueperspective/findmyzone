using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ReactiveUI;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;

namespace findmyzoneui.Services
{
    public class NewtonsoftJsonSuspensionDriver : ISuspensionDriver
    {
        private readonly string file;
        private readonly JsonSerializerSettings settings = new()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public NewtonsoftJsonSuspensionDriver(string file, IContractResolver contractResolver)
        {
            this.file = file; 
            settings.ContractResolver = contractResolver;
        }

        public IObservable<Unit> InvalidateState()
        {
            if (File.Exists(file))
                File.Delete(file);
            return Observable.Return(Unit.Default);
        }

        public IObservable<object> LoadState()
        {
            if (!File.Exists(file))
            {
                throw new IOException($"State file {file} could not be found");
            }

            var lines = File.ReadAllText(file);
            var state = JsonConvert.DeserializeObject<object>(lines, settings);

            if (state == null)
            {
                throw new IOException($"Deserialization error when loading file {file}");
            }

            return Observable.Return(state);
        }

        public IObservable<Unit> SaveState(object state)
        {
            var lines = JsonConvert.SerializeObject(state, settings);
            File.WriteAllText(file, lines);
            return Observable.Return(Unit.Default);
        }
    }
}
