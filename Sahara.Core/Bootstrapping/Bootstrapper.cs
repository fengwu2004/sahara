using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sahara.Core.Bootstrapping
{
    public abstract class Bootstrapper : IBootstrapper
    {
        public virtual int Order
        {
            get { return int.MaxValue; }
        }

        protected virtual void BeforeInitialize() { }
        protected virtual void AfterInitialize() { }

        protected abstract void OnInitialize();

        public void Initialize()
        {
            this.BeforeInitialize();

            this.OnInitialize();

            this.AfterInitialize();
        }
    }
}
