using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using PermissionAttributeCheck;

namespace PermissionAttributeCheck.Test
{
    [TestClass]
    public class UnitTest : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void RequirePermission_Is_Not_Set_On_Ctor_Show_No_Error()
        {
            var test = @"
namespace BBR.Application
{
    public class FordelingsarealService : IFordelingsarealService
    {
        public FordelingsarealService(){}
    }
}";

            VerifyCSharpDiagnostic(test);
        }

        //No diagnostics expected to show up
        [TestMethod]
        public void RequirePermission_Is_Set_Show_No_Error()
        {
            var test = @"
namespace BBR.Application
{
    public class FordelingsarealService : IFordelingsarealService
    {
        [RequirePermission(BBRPermission.BBRData)]
        [GetServiceMethod]
        public virtual EntityWithMetadata<FordelingsarealDto> GetById(Guid uuid)
        {
            var fordelingsareal = _fordelingsarealManager.GetEntityWithHistoryById(uuid, _dateTimeProvider.PointInTime);

            var ret = _fordelingsarealMapper.ToEntityWithMetadata(fordelingsareal, _dateTimeProvider.PointInTime);

            return ret;
        }
    }
}";

            VerifyCSharpDiagnostic(test);
        }

        //Diagnostic and CodeFix both triggered and checked for
        [TestMethod]
        public void RequirePermission_Is_Not_Set_Show_Error()
        {
            var test = @"
namespace BBR.Application
{
    public class FordelingsarealService : IFordelingsarealService
    {
        [GetServiceMethod]
        public virtual EntityWithMetadata<FordelingsarealDto> GetById(Guid uuid)
        {
            var fordelingsareal = _fordelingsarealManager.GetEntityWithHistoryById(uuid, _dateTimeProvider.PointInTime);

            var ret = _fordelingsarealMapper.ToEntityWithMetadata(fordelingsareal, _dateTimeProvider.PointInTime);

            return ret;
        }
    }
}";
            var expected = new DiagnosticResult
            {
                Id = "PermissionAttributeCheck",
                Message = String.Format("Method 'GetById' has no RequirePermissions attribute"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 7, 63)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
namespace BBR.Application
{
    public class FordelingsarealService : IFordelingsarealService
    {
        [RequirePermission(BBRPermission.BBRData)]
        [GetServiceMethod]
        public virtual EntityWithMetadata<FordelingsarealDto> GetById(Guid uuid)
        {
            var fordelingsareal = _fordelingsarealManager.GetEntityWithHistoryById(uuid, _dateTimeProvider.PointInTime);

            var ret = _fordelingsarealMapper.ToEntityWithMetadata(fordelingsareal, _dateTimeProvider.PointInTime);

            return ret;
        }
    }
}";
            //VerifyCSharpFix(test, fixtest);
        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new PermissionAttributeCheckCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new PermissionAttributeCheckAnalyzer();
        }
    }
}