-- 渠道库存菜单 
INSERT INTO menu (SysId, MenuName, Action, Controller, ICons, ParentSysId, IsActive, SortSequence, GroupMenuController, AuthKey)
  VALUES (UUID(), '渠道库存报表', 'ChannelInventoryReport', 'Report', NULL, '2e806600-8ebd-11e6-8ed7-005056bd5942', 1, 25, 'ReportManage', 'Report_ChannelInventoryReport');

