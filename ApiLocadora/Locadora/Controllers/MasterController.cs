using System;
using System.Configuration;
using System.Web.Mvc;

namespace Locadora
{
    public class MasterController : Controller
    {
        /// <summary>retorna a string de conexão armazenada no WebConfig</summary>
        public string GetConString() { return ConfigurationManager.ConnectionStrings["locadora"].ConnectionString; }

        /// <summary>monta e retorna um objeto com a exceção</summary>
        public string GetException(Exception e)
        {
            string ex = e.Message;
            if (!ex.StartsWith("#")) ex = "F#" + ex;//falha de execução
            else ex = "V" + ex;//informações inconsistentes
            return ex.Replace("\n", "<br>");
        }
    }
}

