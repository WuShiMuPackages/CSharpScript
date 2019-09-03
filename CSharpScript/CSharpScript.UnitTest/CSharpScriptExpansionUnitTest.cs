using System;
using Xunit;
using CSharpScript;

namespace CSharpScript.UnitTest
{
    public class CSharpScriptExpansionUnitTest
    {
        [Fact]
        public void GenerateResult_Test1()
        {
            var csharpFragment = @"return $""abc_{DateTime.Parse(""2018-05-01"").ToString(""yyyy-MM-dd"")}"";";
            Assert.Equal("abc_2018-05-01", csharpFragment.GenerateResult());
        }

        [Fact]
        public void GenerateResult_Test2()
        {
            var csharpFragment = @"return $""{DateTime.Parse(""2019-05-01"").ToString(""yyMMdd"")}_account_dd"";";
            Assert.Equal("190501_account_dd", csharpFragment.GenerateResult());
        }
    }
}
