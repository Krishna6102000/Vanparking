using Microsoft.Data.SqlClient;

public class DbHelper
{
    private readonly string cs;

    public DbHelper(IConfiguration config)
    {
        cs = config.GetConnectionString("DefaultConnection");
    }

    // Get Slots
    public List<ParkingSlot> GetSlots()
    {
        List<ParkingSlot> list = new List<ParkingSlot>();

        using (SqlConnection con = new SqlConnection(cs))
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM ParkingSlots", con);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new ParkingSlot
                {
                    SlotId = Convert.ToInt32(dr["SlotId"]),
                    SlotNumber = dr["SlotNumber"].ToString(),
                    IsOccupied = Convert.ToBoolean(dr["IsOccupied"])
                });
            }
        }
        return list;
    }

    // Get Active Vans
    public List<Van> GetActiveVans()
    {
        List<Van> list = new List<Van>();

        using (SqlConnection con = new SqlConnection(cs))
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Vans WHERE ExitTime IS NULL", con);
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Van
                {
                    VanId = Convert.ToInt32(dr["VanId"]),
                    VehicleNumber = dr["VehicleNumber"].ToString(),
                    SlotId = Convert.ToInt32(dr["SlotId"]),
                    EntryTime = Convert.ToDateTime(dr["EntryTime"])
                });
            }
        }
        return list;
    }

    // Park Van
    public void ParkVan(string vehicleNumber, int slotId)
    {
        using (SqlConnection con = new SqlConnection(cs))
        {
            con.Open();

            SqlCommand cmd = new SqlCommand(
                "INSERT INTO Vans(VehicleNumber, SlotId, EntryTime) VALUES(@v,@s,GETDATE())", con);
            cmd.Parameters.AddWithValue("@v", vehicleNumber);
            cmd.Parameters.AddWithValue("@s", slotId);
            cmd.ExecuteNonQuery();

            SqlCommand cmd2 = new SqlCommand(
                "UPDATE ParkingSlots SET IsOccupied=1 WHERE SlotId=@id", con);
            cmd2.Parameters.AddWithValue("@id", slotId);
            cmd2.ExecuteNonQuery();
        }
    }

    // Exit Van
    public void ExitVan(int vanId, int slotId)
    {
        using (SqlConnection con = new SqlConnection(cs))
        {
            con.Open();

            SqlCommand cmd = new SqlCommand(
                "UPDATE Vans SET ExitTime=GETDATE() WHERE VanId=@id", con);
            cmd.Parameters.AddWithValue("@id", vanId);
            cmd.ExecuteNonQuery();

            SqlCommand cmd2 = new SqlCommand(
                "UPDATE ParkingSlots SET IsOccupied=0 WHERE SlotId=@sid", con);
            cmd2.Parameters.AddWithValue("@sid", slotId);
            cmd2.ExecuteNonQuery();
        }
    }
}