using System.Collections.Generic;
using QuanLyQuanCaPhe.DAL;
using QuanLyQuanCaPhe.DTO;

namespace QuanLyQuanCaPhe.BUS
{
    /// <summary>Nghiệp vụ đọc cấu hình của quán (dùng khi in hóa đơn).</summary>
    public static class CaiDatBUS
    {
        public static List<CaiDatDTO> LayTatCa()
        {
            return CaiDatDAL.LayTatCa();
        }

        public static string Lay(string khoa, string macDinh)
        {
            List<CaiDatDTO> items = CaiDatDAL.LayTatCa();
            foreach (CaiDatDTO item in items)
            {
                if (item.Khoa == khoa && !string.IsNullOrEmpty(item.GiaTri))
                {
                    return item.GiaTri;
                }
            }
            return macDinh;
        }
    }
}
