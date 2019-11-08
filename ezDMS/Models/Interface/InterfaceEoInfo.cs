using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ezDMS.Models.Interface
{
    public class InterfaceEoInfo : ItfEoInfo
    {
        public int? temp_idx { get; set; }
        public List<interfacePartInfo> partList { get; set; } 
    }

    public class interfacePartInfo : ItfPartMaster
    {
        public int? temp_idx { get; set; }
        public int? parent_part_idx { get; set; }
        public string parent_part_no { get; set; }
        public string parent_part_rev_no { get; set; }
        public int? part_ord { get; set; }
        public float? qty { get; set; }
        public string find_no { get; set; }

        public List<ItfFileInfo> fileList { get; set; }

        public ItfPartMaster GetPartMaster()
        {
            ItfPartMaster mPart = new ItfPartMaster();
            mPart.part_idx = this.part_idx;
            mPart.eo_idx = this.part_idx;
            mPart.part_no = this.part_no;
            mPart.part_nm = this.part_nm;
            mPart.part_unit = this.part_unit;
            mPart.part_oid = this.part_oid;

            mPart.part_rev_no = this.part_rev_no;
            mPart.part_des = this.part_des;
            mPart.part_attr1 = this.part_attr1;
            mPart.part_attr2 = this.part_attr2;
            mPart.part_attr3 = this.part_attr3;
            mPart.part_attr4 = this.part_attr4;
            mPart.part_attr5 = this.part_attr5;

            return mPart;
        }

        public ItfBomInfo GetBomInfo()
        {
            ItfBomInfo mBom = new ItfBomInfo();
            mBom.part_idx = this.part_idx;
            mBom.parent_part_idx = this.parent_part_idx;
            mBom.part_ord = this.part_ord;
            mBom.qty = this.qty;
            mBom.find_no = this.find_no;

            return mBom;
        }
    }

    public class interfaceResult
    {
        public int? file_idx { get; set; }
        public string partNo { get; set; }
        public string partRevNo { get; set; }
        public string plmFIleName { get; set; }

        public string plmFIleRevNo { get; set; }

        public string ftpFilePath { get; set; }
        public string ftpFileConvName { get; set; }

        public string isNewFile { get; set; }
        
        public string plmFileOid { get; set; }

        public string file_format { get; set; }


    }
}
