using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Locadora.Controllers
{
    public class ClienteController : MasterController
    {
        public string Salvar(string cliente)//o parâmetro cliente, é uma string que virá na url do navegador, ou em uma requisição via ajax com javascript
        {
            try
            {
                Cliente c = JsonConvert.DeserializeObject<Cliente>(cliente, G1.CfJson());//convertemos o parâmetro para um formato que equivale a um objeto da classe cliente

                bool jaExiste = new ClienteDal().CPFCadastrado(c, GetConString());
                if (jaExiste) throw new Exception("CPF já está cadastrado ou vazio!!!");
                new ClienteDal().Salvar(c,GetConString());//salva os dados, e traz o Id que foi gerado dentro do objeto que foi passado como parâmetro, caso seja um novo cadastro
                //return JsonConvert.SerializeObject(c);//retorna o objeto, depois que foi salvo, com todos os dados
                return JsonConvert.SerializeObject("Cliente cadastrado!");

            }
            catch (Exception e) { return JsonConvert.SerializeObject(GetException(e)); }
        }

        public async Task<string> Listar(string cliente = null)
        {
            try
            {
                Cliente c = !G1.Nada(cliente) ? JsonConvert.DeserializeObject<Cliente>(cliente, G1.CfJson()): new Cliente();//convertemos o parâmetro para um formato que equivale a um objeto da classe cliente
                List<Cliente> lista = await Task.FromResult(new ClienteDal().Listar(c, GetConString()));
                return JsonConvert.SerializeObject(lista);
            }
            catch (Exception e) { return JsonConvert.SerializeObject(GetException(e)); }
        }

        public ActionResult TesteApi() { return View("TesteApi"); }
        
    }
}