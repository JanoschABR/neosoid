using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JanoschR.Neosoid.Shared {
    public class Container <T> {
        protected T value;

        public Container (T initial) {
            value = initial;
        }
        public Container () {
            value = default;
        }

        public virtual void Set (T value) {
            this.value = value;
        }
        public virtual T Get () {
            return this.value;
        }
    }
}
