using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace TenantMNG.ADO.NET
{
    public class MeterCLS
    {

        SqlConnection cn = new SqlConnection(ConfigurationManager.AppSettings["DbConnection"]);

        public DataSet getTenantEnergy(string tablename, string s_date, string e_date, string str_peak_s_time_m, string str_peak_e_time_m, string str_inter_s_time_1_m,
            string str_inter_e_time_1_m,string str_inter_s_time_2_m,string str_inter_e_time_2_m,string str_base_s_time_m,string str_base_e_time_m,
            string str_base_s_time_sat,string str_base_e_time_sat,string str_inter_s_time_sat,string str_inter_e_time_sat,string str_base_s_time_sun,
            string str_base_e_time_sun,string  str_inter_s_time_sun,string str_inter_e_time_sun)
        {
            string query = "usp_get_meter_reading_new";
            SqlCommand cm = new SqlCommand(query, cn);
            cm.CommandType = CommandType.StoredProcedure;

            cm.Parameters.AddWithValue("@tb_name", tablename);
            cm.Parameters.AddWithValue("@start_date", s_date);
            cm.Parameters.AddWithValue("@end_date", e_date);
            cm.Parameters.AddWithValue("@str_peak_s_time_m", str_peak_s_time_m);
            cm.Parameters.AddWithValue("@str_peak_e_time_m", str_peak_e_time_m);

            cm.Parameters.AddWithValue("@str_inter_s_time_1_m", str_inter_s_time_1_m);
            cm.Parameters.AddWithValue("@str_inter_e_time_1_m", str_inter_e_time_1_m);

            cm.Parameters.AddWithValue("@str_inter_s_time_2_m", str_inter_s_time_2_m);
            cm.Parameters.AddWithValue("@str_inter_e_time_2_m", str_inter_e_time_2_m);

            cm.Parameters.AddWithValue("@str_base_s_time_m", str_base_s_time_m);
            cm.Parameters.AddWithValue("@str_base_e_time_m", str_base_e_time_m);

            cm.Parameters.AddWithValue("@str_base_s_time_sat", str_base_s_time_sat);
            cm.Parameters.AddWithValue("@str_base_e_time_sat", str_base_e_time_sat);

            cm.Parameters.AddWithValue("@str_inter_s_time_sat", str_inter_s_time_sat);
            cm.Parameters.AddWithValue("@str_inter_e_time_sat", str_inter_e_time_sat);

            cm.Parameters.AddWithValue("@str_base_s_time_sun", str_base_s_time_sun);
            cm.Parameters.AddWithValue("@str_base_e_time_sun", str_base_e_time_sun);

            cm.Parameters.AddWithValue("@str_inter_s_time_sun", str_inter_s_time_sun);
            cm.Parameters.AddWithValue("@str_inter_e_time_sun", str_inter_e_time_sun);



            SqlDataAdapter da = new SqlDataAdapter(cm);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return ds;

            //if (ds != null && ds.Tables[0].Rows.Count > 0)
            //    return ds.Tables[0].Rows[0][0].ToString() + "," + ds.Tables[1].Rows[0][0].ToString();
            //else
            //    return "0";
        }

        public DataSet getMeter()
        {
            string query = "select * from HISTORY_CONFIG";
            SqlCommand cm = new SqlCommand(query, cn);




            SqlDataAdapter da = new SqlDataAdapter(cm);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return ds;

        }

    }

    public class meter
    {
        public int ID { get; set; }

        public string name { get; set; }

        public int tenant_id { get; set; }
    }
}