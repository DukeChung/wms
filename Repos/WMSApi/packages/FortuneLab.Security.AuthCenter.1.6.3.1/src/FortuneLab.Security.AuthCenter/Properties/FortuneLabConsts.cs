namespace FortuneLab
{
    /// <summary>
    /// Used to define some constants for ABP.
    /// </summary>
    internal static class FortuneLabConsts
    {
        //���ͨ��ȷ�������ļ��Ƿ��б仯������ޱ仯�����ø���
        public const string FortuneLabVersion = "1.6.1.0";
        public const string FortuneLabAbpVersion = "1.6.1.0";
        public const string FortuneLabCoreService = "1.6.4.0";
        public const string FortuneLabWebClient = "1.6.2.0";
    }
}

/*
--1.2.0.7
1. ����EfSimpleRepository�࣬TPrimaryKey��Guid��ʵ��

--1.2.0.8   
�޸�ErrorCodes\ResourceHelper�е�Bug

--1.2.0.9
 * BusinessException�Ƴ�message����
 * ApiResponse��������ToJsonResult����
 * 

--1.3.0.0 
 * ȫ��֧��AuthCenter��¼��֤
 * 
--1.3.2.0
 * Portal & API�˿�ʼ����������, ����: FortuneLab, FortuneLab.WebApiClient, ���������������Ӵ�

*/