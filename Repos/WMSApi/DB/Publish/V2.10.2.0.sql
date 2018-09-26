ALTER TABLE purchaseextend ADD COLUMN ServiceStationCode varchar(64) NULL;

ALTER TABLE purchaseextend ADD COLUMN ServiceStationName varchar(128) NULL;

ALTER TABLE pack ADD COLUMN UpdateDate datetime NOT NULL DEFAULT NOW();

ALTER TABLE pack ADD COLUMN UpdateBy BIGINT NOT NULL DEFAULT 0;

ALTER TABLE pack ADD COLUMN UpdateUserName varchar(32) NULL DEFAULT '';

ALTER TABLE pack ADD COLUMN CreateDate datetime NOT NULL DEFAULT NOW();

ALTER TABLE pack ADD COLUMN CreateBy BIGINT NOT NULL DEFAULT 0;

ALTER TABLE pack ADD COLUMN CreateUserName varchar(32) NULL DEFAULT '';

ALTER TABLE outbounddetail ADD COLUMN Memo varchar(256) NULL;

-- 清空移仓收货表 收获数量，初始化为0
UPDATE transferinventoryreceiptextend t
SET t.ReceivedQty = 0;