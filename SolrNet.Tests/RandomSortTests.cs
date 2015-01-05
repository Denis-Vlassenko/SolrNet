#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
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

using System;
using NUnit.Framework;

namespace SolrNet.Tests {
    [TestFixture]
    public class RandomSortTests {
        [Test]
        public void Random() {
            var r = new RandomSortOrder("random");
            var rndSort = r.ToString();
            Console.WriteLine(rndSort);
            StringAssert.IsMatch("random_\\d+ asc", rndSort);
        }

        [Test]
        public void RandomWithSeed() {
            const string seed = "234asd";
            var r = new RandomSortOrder("random", seed);
            var rndSort = r.ToString();
            Console.WriteLine(rndSort);
            Assert.AreEqual(rndSort, string.Format("random_{0} asc", seed));
        }

        [Test]
        public void RandomWithOrder() {
            var r = new RandomSortOrder("random", Order.DESC);
            var rndSort = r.ToString();
            Console.WriteLine(rndSort);
            StringAssert.IsMatch("random_\\d+ desc", rndSort);
        }

        [Test]
        public void RandomWithSeedAndOrder() {
            const string seed = "234asd";
            var r = new RandomSortOrder("random", seed, Order.DESC);
            var rndSort = r.ToString();
            Console.WriteLine(rndSort);
            Assert.AreEqual(rndSort, string.Format("random_{0} desc", seed));
        }
    }
}