using System.Text;

namespace AppProyecto.Models
{
    public class utilidades
    {
        public static string convertirBase64(string ruta)
        {
            var messageBytes  = Encoding.UTF8.GetBytes(ruta);
            var encodedMessage = Convert.ToBase64String(messageBytes);
            return encodedMessage;
        }
    }
}
