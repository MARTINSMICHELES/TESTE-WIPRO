using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Locadora.Controllers
{
    public class ItensLocacaoController : MasterController
    {
        public string Salvar(string itenslocacao)//o parâmetro cliente, é uma string que virá na url do navegador, ou em uma requisição via ajax com javascript
        {
            try
            {
                ItensLocacao i = JsonConvert.DeserializeObject<ItensLocacao>(itenslocacao, G1.CfJson());//convertemos o parâmetro para um formato que equivale a um objeto da classe cliente

                bool jaExiste = new ItensLocacaoDal().FilmeDisponivel(i, GetConString());
                if (jaExiste) throw new Exception("Filme indisponível!!!");
                new ItensLocacaoDal().Salvar(i, GetConString());//salva os dados, e traz o Id que foi gerado dentro do objeto que foi passado como parâmetro, caso seja um novo cadastro
                //return JsonConvert.SerializeObject(i);//retorna o objeto, depois que foi salvo, com todos os dados
                return JsonConvert.SerializeObject("Itens da Locação Inseridos");

            }
            catch (Exception e) { return JsonConvert.SerializeObject(GetException(e)); }
        }

        public async Task<string> Listar(string itenslocacao = null)
        {
            try
            {
//                ItensLocacao i = G1.Nada(itenslocacao) ? JsonConvert.DeserializeObject<ItensLocacao>(itenslocacao, G1.CfJson()) : new ItensLocacao();//convertemos o parâmetro para um formato que equivale a um objeto da classe cliente
                ItensLocacao i = G1.Nada(itenslocacao) ? JsonConvert.DeserializeObject<ItensLocacao>(itenslocacao, G1.CfJson()) : new ItensLocacao();//convertemos o parâmetro para um formato que equivale a um objeto da classe cliente
                List<ItensLocacao> lista = await Task.FromResult(new ItensLocacaoDal().Listar(i, GetConString()));
                return JsonConvert.SerializeObject(lista);
            }
            catch (Exception e) { return JsonConvert.SerializeObject(GetException(e)); }
        }

        public ActionResult TesteApi() { return View("TesteApi"); }


    }
}
