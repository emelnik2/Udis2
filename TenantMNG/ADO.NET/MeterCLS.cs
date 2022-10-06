using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace TenantMNG.ADO.NET
{
    public class MeterCLS
    {

        SqlConnection cn = new SqlConnection(ConfigurationManager.AppSettings["DbConnection"]);

        public DataSet getTenantEnergy(string tablename, string s_date, string e_date)
        {
            string query = "usp_get_meter_reading_new";
            SqlCommand cm = new SqlCommand(query, cn);
            cm.CommandType = CommandType.StoredProcedure;

            cm.Parameters.AddWithValue("@tb_name", tablename);
            cm.Parameters.AddWithValue("@start_date", s_date);
            cm.Parameters.AddWithValue("@end_date", e_date);

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
            string query = "SELECT meters.M AS str_meter_id, ISNULL(tbl_tenant_meter.multiplicador, 1 ) AS multiplicador FROM " +
                            "(SELECT DISTINCT M FROM( " +
                            "    SELECT  CFE_MeterID as M FROM UDIS " +
                            "    UNION " + 
                            "    SELECT str_meter_id as M FROM tbl_tenant_meter " +
                            ")a) AS meters " +
                            "LEFT JOIN tbl_tenant_meter " +
                            "on meters.M = tbl_tenant_meter.str_meter_id " +
                            "ORDER BY tbl_tenant_meter.str_meter_id ";
            //string query = "SELECT DISTINCT CFE_MeterID FROM udis.summation_sources";
            SqlCommand cm = new SqlCommand(query, cn);


            SqlDataAdapter da = new SqlDataAdapter(cm);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return ds;

        }

    }

    public class meter
    {
        public string name { get; set; }

        public int tenant_id { get; set; }

        public int multiplier { get; set; }


    }
}