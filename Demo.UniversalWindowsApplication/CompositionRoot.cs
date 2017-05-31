using System;
using Tampleworks.WindowsApplicationBlock.Demo.AppLogic;
using Tampleworks.WindowsApplicationBlock.Demo.AppLogic.ApplicationLogicData;
using Tampleworks.WindowsApplicationBlock.ApplicationLogicAbstractions;

namespace Tampleworks.WindowsApplicationBlock.Demo.UwpApp
{
    public sealed class CompositionRoot
    {
        private static readonly Lazy<CompositionRoot> instance = new Lazy<CompositionRoot>(() => new CompositionRoot());

        private Lazy<ApplicationLogicFactory> applicationLogicFactory;

        private CompositionRoot()
        {
            applicationLogicFactory = new Lazy<ApplicationLogicFactory>(NewApplicationLogicFactory);
        }

        public static CompositionRoot Instance => instance.Value;

        private ApplicationLogicFactory NewApplicationLogicFactory() => new ApplicationLogicFactory(
            new ApplicationLogicDataProviderFactory(),
            new ApplicationMemoryManger(),
            new SemanticLogger()
        );
        internal ApplicationLogicFactory GetApplicationLogicFactory() => applicationLogicFactory.Value;

        /// <summary>
        /// This class is only for demonstration of injecting some logic from composition root into application logic layer. 
        /// </summary>
        private sealed class ApplicationLogicDataProviderFactory : IApplicationLogicDataProviderFactory
        {
            public IApplicationLogicDataProvider Get(IApplicationLogicAgent applicationLogicAgent, MemoryController memoryBank)
                => new ApplicationLogicDataProvider();

            private sealed class ApplicationLogicDataProvider : IApplicationLogicDataProvider
            {

            }
        }
    }
}
