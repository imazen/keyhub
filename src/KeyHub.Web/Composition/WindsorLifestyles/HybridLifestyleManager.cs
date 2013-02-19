#region license
// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Lifestyle;

namespace KeyHub.Web.Composition.WindsorLifestyles {
    /// <summary>
    /// Abstract hybrid lifestyle manager, with two underlying lifestyles
    /// </summary>
    /// <typeparam name="TM1">Primary lifestyle manager</typeparam>
    /// <typeparam name="TM2">Secondary lifestyle manager</typeparam>
    public abstract class HybridLifestyleManager<TM1, TM2> : AbstractLifestyleManager
        where TM1 : ILifestyleManager, new()
        where TM2 : ILifestyleManager, new() {
        protected readonly TM1 Lifestyle1 = new TM1();
        protected readonly TM2 Lifestyle2 = new TM2();

        public override void Dispose() {
            Lifestyle1.Dispose();
            Lifestyle2.Dispose();
        }

        public override void Init(IComponentActivator componentActivator, IKernel kernel, ComponentModel model) {
            Lifestyle1.Init(componentActivator, kernel, model);
            Lifestyle2.Init(componentActivator, kernel, model);
        }

        public override bool Release(object instance) {
            var r1 = Lifestyle1.Release(instance);
            var r2 = Lifestyle2.Release(instance);
            return r1 || r2;
        }

        public abstract override object Resolve(CreationContext context, IReleasePolicy releasePolicy);
        }
}