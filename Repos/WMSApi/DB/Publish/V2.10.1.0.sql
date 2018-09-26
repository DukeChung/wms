ALTER TABLE purchasedetail ADD COLUMN UpdateDate datetime NOT NULL DEFAULT NOW();

ALTER TABLE purchasedetail ADD COLUMN UpdateBy BIGINT NOT NULL DEFAULT 0;

ALTER TABLE purchasedetail ADD COLUMN UpdateUserName varchar(32) NULL DEFAULT '';

alter table invtrans modify column PackCode varchar(255);
alter table purchasedetail modify column PackCode varchar(255);