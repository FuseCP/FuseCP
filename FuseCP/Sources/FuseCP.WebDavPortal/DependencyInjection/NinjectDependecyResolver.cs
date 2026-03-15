// Copyright (C) 2025 FuseCP
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

#if NETFRAMEWORK
using System.Web.Http.Dependencies;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FuseCP.WebDavPortal.DependencyInjection
{
    public class NinjectDependecyResolver : System.Web.Mvc.IDependencyResolver, System.Web.Http.Dependencies.IDependencyResolver
    {
        IKernel kernal;

        public NinjectDependecyResolver()
        {
            kernal = new StandardKernel(new NinjectSettings { AllowNullInjection = true });

            AddBindings();
        }

        public object GetService(Type serviceType)
        {
            return kernal.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernal.GetAll(serviceType);
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        private void AddBindings()
        {
            PortalDependencies.Configure(kernal);
        }

        public void Dispose()
        {
            
        }
    }
}
#endif
