using System;
using System.Collections.Generic;
using System.Data;
using QuanLyQuanCaPhe.DTO;

namespace QuanLyQuanCaPhe.DAL
{
    /// <summary>Truy vấn ADO.NET cho bảng cấu hình của quán.</summary>
    public static class CaiDatDAL
    {
        public static List<CaiDatDTO> LayTatCa()
        {
            DataTable table = DatabaseHelper.Query("SELECT Khoa, GiaTri FROM dbo.CaiDat");
            var result = new List<CaiDatDTO>();
            foreach (DataRow row in table.Rows)
            {
                result.Add(new CaiDatDTO
                {
                    Khoa = Convert.ToString(row["Khoa"]),
                    GiaTri = Convert.ToString(row["GiaTri"])
                });
            }
            return result;
        }
    }
}
