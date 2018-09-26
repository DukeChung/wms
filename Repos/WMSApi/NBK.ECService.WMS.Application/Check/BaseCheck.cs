using NBK.ECService.WMS.Application.Interface;
using NBK.ECService.WMS.DTO;

namespace NBK.ECService.WMS.Application.Check
{
    /// <summary>
    /// Check抽象类，已实现ICheck接口
    /// Before和After方法为虚方法，未实现，根据需要重写相应方法
    /// Check方法为抽象方法，具体check逻辑必须由子类重写
    /// Execute方法控制整体check流程
    /// </summary>
    public abstract class BaseCheck : ICheck
    {
        public virtual void BeforeCheck() { }

        public virtual void AfterCheck() { }

        public abstract CommonResponse Check();

        public CommonResponse Execute()
        {
            BeforeCheck();
            CommonResponse rsp = Check();
            AfterCheck();
            return rsp;
        }
    }
}
