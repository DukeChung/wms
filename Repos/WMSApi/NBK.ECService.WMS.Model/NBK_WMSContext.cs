using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Abp.EntityFramework;
using NBK.ECService.WMS.Model.Models.Mapping;

namespace NBK.ECService.WMS.Model.Models
{
    public partial class NBK_WMSContext : AbpDbContext
    {
        public NBK_WMSContext()
            : base("nbk_wmsContext")
        {

        }
        public void ChangeDB(string connectionString)
        {
            if (this.Database.Connection.ConnectionString != connectionString)
            {
                this.Database.Connection.ConnectionString = connectionString;
            }
        }

        public DbSet<outboundrule> outboundrules { get; set; }
        public DbSet<prepackrelation> prepackrelations { get; set; }
        public DbSet<adjustment> adjustments { get; set; }
        public DbSet<adjustmentdetail> adjustmentdetails { get; set; }
        public DbSet<carrier> carriers { get; set; }
        public DbSet<container> containers { get; set; }
        public DbSet<invlot> invlots { get; set; }
        public DbSet<invlotloclpn> invlotloclpns { get; set; }
        public DbSet<invskuloc> invskulocs { get; set; }
        public DbSet<invtran> invtrans { get; set; }
        public DbSet<location> locations { get; set; }
        public DbSet<lottemplate> lottemplates { get; set; }
        public DbSet<menu> menus { get; set; }
        public DbSet<nextnumbergen> nextnumbergens { get; set; }
        public DbSet<operationlog> operationlogs { get; set; }

        public DbSet<preorderrule> preorderrules { get; set; }
        public DbSet<outbound> outbounds { get; set; }
        public DbSet<outbounddetail> outbounddetails { get; set; }
        public DbSet<pack> packs { get; set; }
        public DbSet<pickdetail> pickdetails { get; set; }
        public DbSet<purchase> purchases { get; set; }
        public DbSet<picture> pictures { get; set; }
        public DbSet<transferinventory> transferinventorys { get; set; }
        public DbSet<transferinventorydetail> transferinventorydetails { get; set; }
        public DbSet<purchasedetail> purchasedetails { get; set; }
        public DbSet<receipt> receipts { get; set; }
        public DbSet<receiptdetail> receiptdetails { get; set; }
        public DbSet<receiptsn> receiptsns { get; set; }
        public DbSet<sku> skus { get; set; }
        public DbSet<skuclass> skuclasses { get; set; }
        public DbSet<stocktake> stocktakes { get; set; }
        public DbSet<stocktakedetail> stocktakedetails { get; set; }
        public DbSet<syscode> syscodes { get; set; }
        public DbSet<syscodedetail> syscodedetails { get; set; }
        public DbSet<uom> uoms { get; set; }
        public DbSet<vanning> vannings { get; set; }
        public DbSet<vanningdetail> vanningdetails { get; set; }
        public DbSet<vanningpickdetail> vanningpickdetails { get; set; }
        public DbSet<vendor> vendors { get; set; }
        public DbSet<warehouse> warehouses { get; set; }
        public DbSet<zone> zones { get; set; }
        public DbSet<stockmovement> stockmovements { get; set; }

        public DbSet<stocktransfer> stocktransfers { get; set; }

        public DbSet<userwarehousemapping> userwarehousemappings { get; set; }

        public DbSet<unitconversiontran> unitconversiontrans { get; set; }

        public DbSet<assembly> assemblies { get; set; }
        public DbSet<assemblydetail> assemblydetails { get; set; }

        public DbSet<component> components { get; set; }
        public DbSet<componentdetail> componentdetails { get; set; }
        public DbSet<prepack> prepacks { get; set; }
        public DbSet<prepackdetail> prepackdetails { get; set; }

        public DbSet<prebulkpack> prebulkpack { get; set; }

        public DbSet<prebulkpackdetail> prebulkpackdetail { get; set; }

        public DbSet<qualitycontrol> qualitycontrol { get; set; }

        public DbSet<qualitycontroldetail> qualitycontroldetail { get; set; }

        public DbSet<stockfrozen> stockfrozen { get; set; }

        public DbSet<skuborrow> skuborrow { get; set; }

        public DbSet<skuborrowdetail> skuborrowdetail { get; set; }

        public DbSet<work> works { get; set; }

        public DbSet<workuser> workusers { get; set; }

        public DbSet<picking> picking { get; set; }

        public DbSet<pickingrecords> pickingrecords { get; set; }

        public DbSet<outboundtransferorder> outboundtransferorder { get; set; }

        public DbSet<receiptdatarecord> receiptdatarecord { get; set; }

        public DbSet<workrule> workrule { get; set; }
        public DbSet<outboundtransferorderdetail> outboundtransferorderdetail { get; set; }

        public DbSet<purchaseextend> purchaseextend { get; set; }
        public DbSet<outboundexception> outboundexception { get; set; }
        public DbSet<assemblyrule> assemblyrule { get; set; }

        public DbSet<assemblyskuweight> assemblyskuweight { get; set; }

        public DbSet<transferinventoryreceiptextend> transferinventoryreceiptextend { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Configurations.Add(new assemblyruleMap());
            modelBuilder.Configurations.Add(new outboundexceptionMap());
            modelBuilder.Configurations.Add(new outboundtransferorderdetailMap());
            modelBuilder.Configurations.Add(new receiptdatarecordMap());
            modelBuilder.Configurations.Add(new outboundtransferorderMap());
            modelBuilder.Configurations.Add(new outboundruleMap());
            modelBuilder.Configurations.Add(new prepackrelationMap());
            modelBuilder.Configurations.Add(new assemblyMap());
            modelBuilder.Configurations.Add(new assemblydetailMap());
            modelBuilder.Configurations.Add(new componentMap());
            modelBuilder.Configurations.Add(new componentdetailMap());
            modelBuilder.Configurations.Add(new adjustmentMap());
            modelBuilder.Configurations.Add(new adjustmentdetailMap());
            modelBuilder.Configurations.Add(new carrierMap());
            modelBuilder.Configurations.Add(new containerMap());
            modelBuilder.Configurations.Add(new invlotMap());
            modelBuilder.Configurations.Add(new invlotloclpnMap());
            modelBuilder.Configurations.Add(new invskulocMap());
            modelBuilder.Configurations.Add(new invtranMap());
            modelBuilder.Configurations.Add(new locationMap());
            modelBuilder.Configurations.Add(new lottemplateMap());
            modelBuilder.Configurations.Add(new menuMap());
            modelBuilder.Configurations.Add(new nextnumbergenMap());
            modelBuilder.Configurations.Add(new preorderruleMap());
            modelBuilder.Configurations.Add(new operationlogMap());
            modelBuilder.Configurations.Add(new outboundMap());
            modelBuilder.Configurations.Add(new outbounddetailMap());
            modelBuilder.Configurations.Add(new userwarehousemappingMap());
            modelBuilder.Configurations.Add(new packMap());
            modelBuilder.Configurations.Add(new pickdetailMap());
            modelBuilder.Configurations.Add(new purchaseMap());
            modelBuilder.Configurations.Add(new purchasedetailMap());
            modelBuilder.Configurations.Add(new receiptMap());
            modelBuilder.Configurations.Add(new receiptdetailMap());
            modelBuilder.Configurations.Add(new receiptsnMap());
            modelBuilder.Configurations.Add(new skuMap());
            modelBuilder.Configurations.Add(new skuclassMap());
            modelBuilder.Configurations.Add(new stocktakeMap());
            modelBuilder.Configurations.Add(new stocktakedetailMap());
            modelBuilder.Configurations.Add(new syscodeMap());
            modelBuilder.Configurations.Add(new syscodedetailMap());
            modelBuilder.Configurations.Add(new uomMap());
            modelBuilder.Configurations.Add(new vanningMap());
            modelBuilder.Configurations.Add(new vanningdetailMap());
            modelBuilder.Configurations.Add(new vanningpickdetailMap());
            modelBuilder.Configurations.Add(new vendorMap());
            modelBuilder.Configurations.Add(new warehouseMap());
            modelBuilder.Configurations.Add(new zoneMap());
            modelBuilder.Configurations.Add(new stockmovementMap());
            modelBuilder.Configurations.Add(new unitconversiontranMap());
            modelBuilder.Configurations.Add(new stocktransferMap());
            modelBuilder.Configurations.Add(new transferinventoryMap());
            modelBuilder.Configurations.Add(new transferinventorydetailMap());
            modelBuilder.Configurations.Add(new pictureMap());
            modelBuilder.Configurations.Add(new prepackMap());
            modelBuilder.Configurations.Add(new prepackdetailMap());
            modelBuilder.Configurations.Add(new prebulkpackMap());
            modelBuilder.Configurations.Add(new prebulkpackdetailMap());
            modelBuilder.Configurations.Add(new qualitycontrolMap());
            modelBuilder.Configurations.Add(new qualitycontroldetailMap());
            modelBuilder.Configurations.Add(new stockfrozenMap());
            modelBuilder.Configurations.Add(new skuborrowMap());
            modelBuilder.Configurations.Add(new skuborrowdetailMap());
            modelBuilder.Configurations.Add(new workMap());
            modelBuilder.Configurations.Add(new workuserMap());
            modelBuilder.Configurations.Add(new pickingMap());
            modelBuilder.Configurations.Add(new pickingrecordsMap());
            modelBuilder.Configurations.Add(new workruleMap());
            modelBuilder.Configurations.Add(new purchaseextendMap());
            modelBuilder.Configurations.Add(new assemblyskuweightMap());
            modelBuilder.Configurations.Add(new transferinventoryreceiptextendMap());
        }
    }
}
