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

        public void LogError(string message, Exception? ex = null)
        {
            if (ex != null)
                Log.Error(ex, message);
            else
                Log.Error(message);
        }
    }
}



