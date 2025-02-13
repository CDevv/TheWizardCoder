using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GdUnit4;
using static GdUnit4.Assertions;

namespace TheWizardCoder.Tests
{
    [TestSuite]
    public class BattleEffectTests
    {
        [TestCase]
        public void TestAsset()
        {
            AssertBool(true).IsTrue();
        }
    }
}
