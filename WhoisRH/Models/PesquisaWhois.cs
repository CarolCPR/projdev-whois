using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace WhoisRH.Models{
    public class PesquisaWhois{
        public int Id { get; set; }
        public string Dominio { get; set; }
        public DateTime DataPesquisa { get; set; }
        public bool Registrado { get; set; }
        public DateTime? DataRegistro { get; set; }
        public DateTime? UltimaAlteracao { get; set; }
        public DateTime? Expiracao { get; set; }
        public string NomesServidores { get; set; }

        public PesquisaWhois() {
            DataPesquisa = DateTime.Now;
        }

        public static PesquisaWhois FromDomain(string dominio) {
            try {
                var requisicao = WebRequest.Create($"https://jsonwhoisapi.com/api/v1/whois?identifier={dominio}");
                requisicao.Method = "GET";
                requisicao.Timeout = 10000;
                requisicao.ContentType = "application/json";
                requisicao.Headers.Add("authorization", "Basic " +
                    Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes("479791909:jTMaA-xKCV2w_u0J6NJQfg")));
                return FromJSON(new StreamReader(requisicao.GetResponse().GetResponseStream()).ReadToEnd());
            } catch(WebException e) {
                if(e.Response == null)
                    switch (e.Status) {
                        case WebExceptionStatus.NameResolutionFailure:
                            throw new WebException("Não foi possível estabelecer uma conexão com o servidor.");
                        default:
                            throw e;
                    }
                    else
                    switch ((int)(e.Response as HttpWebResponse).StatusCode) {
                        case 422:
                            throw new Exception("Domínio inválido.");
                        default:
                            throw e;
                    }
                throw e;
            }
        }

        public static PesquisaWhois FromJSON(string json) {
            DateTime? DateFromJSON(string data) {
                if (string.IsNullOrEmpty(json)) return null;
                return DateTime.ParseExact(data, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            }
            dynamic raw = JsonConvert.DeserializeObject(json);
            if (raw.registered.Value) return new PesquisaWhois() {
                DataRegistro = DateFromJSON(raw.created),
                UltimaAlteracao = DateFromJSON(raw.changed),
                Expiracao = DateFromJSON(raw.expires),
                Dominio = raw.name,
                NomesServidores = String.Join("\n", raw.nameservers),
                Registrado = true
            };
            else return new PesquisaWhois() {
                Registrado = false,
                Dominio = raw.name
            };
        }
    }

    public class WhoisRHDbContext : DbContext {
        public DbSet<PesquisaWhois> Pesquisas { get; set; }
    }
}
