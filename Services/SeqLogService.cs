using Serilog;

namespace SapGateway.Services
{
    
    public class SeqLogService
    {
        public void LogInfo(string message)
        {
            Log.Information(message);
        }

        // ✅ เพิ่ม overload รองรับ object detail
        public void LogInfo(string message, object detail)
        {
            // ใช้ structured log => Seq จะแสดง property ในรูปแบบ JSON
            Log.Information("{Message} {@Detail}", message, detail);
        }

        public void LogWarning(string message)
        {
            Log.Warning(message);
        }

        // ✅ เพิ่ม overload รองรับ object detail สำหรับ Warning
        public void LogWarning(string message, object detail)
        {
            // ใช้ structured log => Seq จะแสดง property ในรูปแบบ JSON
            Log.Warning("{Message} {@Detail}", message, detail);
        }

        public void LogError(string message, Exception? ex = null)
        {
            if (ex != null)
                Log.Error(ex, message);
            else
                Log.Error(message);
        }

        // ✅ เพิ่ม overload รองรับ object detail สำหรับ Error (ไม่มี Exception)
        public void LogError(string message, object detail)
        {
            // ใช้ structured log => Seq จะแสดง property ในรูปแบบ JSON
            Log.Error("{Message} {@Detail}", message, detail);
        }

        // ✅ เพิ่ม overload รองรับ Exception และ object detail สำหรับ Error
        public void LogError(string message, Exception ex, object detail)
        {
            // ใช้ structured log => Seq จะแสดง property ในรูปแบบ JSON พร้อม Exception
            Log.Error(ex, "{Message} {@Detail}", message, detail);
        }
    }
}



