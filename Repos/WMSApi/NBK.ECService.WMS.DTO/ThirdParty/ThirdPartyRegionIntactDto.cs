using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBK.ECService.WMS.DTO.ThirdParty
{
    public class ThirdPartyRegionIntactDto
    {
        public Guid SysId { get; set; }

        public int SysNo { get; set; }

        public string RegionID { get; set; }

        public string Name { get; set; }

        public int Level { get; set; }

        public int ParentSysNo { get; set; }

        public int Status { get; set; }

        public Guid ProvinceSysId { get; set; }

        public string ProvinceRegionID { get; set; }

        public string ProvinceName { get; set; }

        public Guid CitySysId { get; set; }

        public string CityRegionID { get; set; }

        public string CityName { get; set; }

        public Guid DistrictSysId { get; set; }

        public string DistrictRegionID { get; set; }

        public string DistrictName { get; set; }

        public Guid TownSysId { get; set; }

        public string TownRegionID { get; set; }

        public string TownName { get; set; }

        public Guid VillageSysId { get; set; }

        public string VillageRegionID { get; set; }

        public string VillageName { get; set; }
    }
}
