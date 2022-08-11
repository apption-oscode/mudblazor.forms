using MudBlazor.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Elemental.Tests
{
    public class LabelizeTests
    {
        [Fact]
        public void Test_Labelize1()
        {
            Assert.Equal("Class 1 Fighter", MudModelFormTools.Labelize("Class1Fighter"));
        }
    }
}
