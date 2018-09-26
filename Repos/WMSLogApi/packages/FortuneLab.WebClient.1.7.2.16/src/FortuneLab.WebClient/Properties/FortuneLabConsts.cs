namespace FortuneLab
{
    /// <summary>
    /// Used to define some constants for ABP.
    /// </summary>
    internal static class FortuneLabConsts
    {
        //这个通常确认里面文件是否有变化，如果无变化，则不用更新
        public const string FortuneLabVersion = "1.6.2.0";
        public const string FortuneLabAbpVersion = "1.6.2.1";
        public const string FortuneLabCoreService = "1.7.2.9";
        public const string FortuneLabWebClient = "1.7.2.16";
    }
}

/*
--1.2.0.7
1. 增加EfSimpleRepository类，TPrimaryKey用Guid的实现

--1.2.0.8   
修复ErrorCodes\ResourceHelper中的Bug

--1.2.0.9
 * BusinessException移除message对象
 * ApiResponse对象增加ToJsonResult方法
 * 

--1.3.0.0 
 * 全面支持AuthCenter登录认证
 * 
--1.3.2.0
 * Portal & API端开始共用相关组件, 比如: FortuneLab, FortuneLab.WebApiClient, 后面这样的扩充会加大

*/